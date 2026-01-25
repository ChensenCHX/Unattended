using CodeExecutor;
using GlobalSettings;
using UnityEngine;

public class Test : MonoBehaviour
{
    private LuaVM luaVM;
    void Start()
    {
        luaVM = new LuaVM(new LuaVMConfigurer(8,
            vm => {
                LuaVMAdaptorLib.AtomicCAS(vm); LuaVMAdaptorLib.CheckThread(vm);
                LuaVMAdaptorLib.GetCurrentThreadID(vm); LuaVMAdaptorLib.HangupCurrentThread(vm);
                LuaVMAdaptorLib.NewThread(vm); LuaVMAdaptorLib.Move(vm);
                LuaVMAdaptorLib.CanHarvest(vm); LuaVMAdaptorLib.Harvest(vm);
                LuaVMAdaptorLib.TrySetFacility(vm); LuaVMAdaptorLib.InteractWith(vm);
                vm.GetLuaVM().Options.DebugPrint = Debug.Log;
            }, vm => { }, LuaVMAdaptorLib.CheckCurrentBotIsBusy), 
            "TestScript", @"
print('Thread id:', get_current_thread())
build(2)
build(8)
print(interact_with('get_tone'))
while true do
    move(1)
    if can_harvest() then 
        harvest()
        build(2)
    else
        hangup_current_thread()
    end
end
build(1)
            ");
    }

    void Update()
    {
        if (luaVM.CouldResume())
            luaVM.ResumeUntilLimit(LuaVMConfigurer.MaxInstructionPerResume);
        else
        {
            Debug.Log(luaVM.State);
            if (luaVM.State == RunningState.Faulted) Debug.Log(luaVM.ExceptionWhat);
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        luaVM.Dispose();
    }
}

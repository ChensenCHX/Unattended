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
                vm.GetLuaVM().Options.DebugPrint = Debug.Log;
            }, LuaVMAdaptorLib.DestroyAllBots, LuaVMAdaptorLib.CheckCurrentBotIsBusy), 
            "TestScript", @"
function f()
    print('Thread id:', get_current_thread())
    move(2) move(2) move(2) move(2) move(2) move(2) move(2) move(2)
    move(2) move(2) move(2) move(2) move(2) move(2) move(2) move(2)
    while true do end
end

print('Thread id:', get_current_thread())
print('New thread:', new_thread(f))
print(can_harvest())
print(harvest())
harvest() harvest() harvest() harvest() harvest()
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
    }
}

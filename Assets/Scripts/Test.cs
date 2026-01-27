using System;
using CodeExecutor;
using GlobalSettings;
using UnityEngine;

public class Test : MonoBehaviour
{
    private LuaVM luaVM;
    private double lastFrameCount = GlobalInfos.Instance.MelodiaCount;
    void Start()
    {
        luaVM = new LuaVM(new LuaVMConfigurer(8,
            vm => {
                LuaVMAdaptorLib.AtomicCAS(vm); LuaVMAdaptorLib.CheckThread(vm);
                LuaVMAdaptorLib.GetCurrentThreadID(vm); LuaVMAdaptorLib.HangupCurrentThread(vm);
                LuaVMAdaptorLib.NewThread(vm); LuaVMAdaptorLib.GetCurrentFrameCount(vm);
                LuaVMAdaptorLib.Move(vm); LuaVMAdaptorLib.UseItem(vm);
                LuaVMAdaptorLib.CanHarvest(vm); LuaVMAdaptorLib.Harvest(vm);
                LuaVMAdaptorLib.TrySetFacility(vm); LuaVMAdaptorLib.InteractWith(vm);
                vm.GetLuaVM().Options.DebugPrint = Debug.Log;
            }, vm => { }, LuaVMAdaptorLib.CheckCurrentBotIsBusy), 
            "TestScript", @"
print('Thread id:', get_current_thread())
build(2)
build(4)
build(16)

print(interact_with('check'))
itemType, targetTime = interact_with('start', 114514)
print(itemType, targetTime, get_current_frame_count())
while(targetTime ~= get_current_frame_count()+3) do hangup_current_thread() end
use_item(itemType)

while(not can_harvest()) do hangup_current_thread() end
harvest()
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

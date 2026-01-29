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

function test()
    build(2)
    build(4)
    build(32)
    --print(('x=%d, y=%d;   height:%d, strength=%d;'):format(i, j, interact_with('get_height'), interact_with('get_strength')))
end

function for_all(f)
	function row()
		for i=1, 31 do
			f()
			move(1)
		end
		f()
    end
	for i=1, 32 do
		if not new_thread(row) then row() end
		move(2)
    end
end

for_all(test)
move(1)


height = interact_with('get_height') print(height) move(1)

t = {0}
for i=1, 31 do
    val = interact_with('get_height') print(val)
    if val <= t[#t] or val >= height then interact_with('detach') harvest() else table.insert(t, val) end
    move(1)
end
for i=1, 300 do hangup_current_thread() end
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
        luaVM?.Dispose();
    }
}

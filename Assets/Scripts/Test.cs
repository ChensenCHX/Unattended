using System;
using CodeExecutor;
using EditorUIAdaptor;
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
                LuaVMAdaptorLib.GetPosition(vm);
                LuaVMAdaptorLib.Move(vm); LuaVMAdaptorLib.UseItem(vm);
                LuaVMAdaptorLib.CanHarvest(vm); LuaVMAdaptorLib.Harvest(vm);
                LuaVMAdaptorLib.TrySetFacility(vm); LuaVMAdaptorLib.InteractWith(vm);
                vm.GetLuaVM().Options.DebugPrint = Debug.Log;
            }, vm => { }, LuaVMAdaptorLib.CheckCurrentBotIsBusy), 
            "TestScript", @"
for i=0, 5000 do
    for i=1, 8 do
        build(2)
        move(1)
    end
    move(2)
end
            ");
        
        EditorWindowManager.Instance.CreateEditorWindow();
    }

    private bool printed = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            var screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                EditorWindowManager.Instance.RectTransform,
                screenCenter,
                null,
                out var localPos
                );
            EditorWindowManager.Instance.CreateEditorWindow(null, null, (int)localPos.x, (int)localPos.y);
        }
        
        if (luaVM.CouldResume())
            luaVM.ResumeUntilLimit(LuaVMConfigurer.MaxInstructionPerResume * 10);
        else if (!printed)
        {
            printed = true;
            Debug.Log(luaVM.State);
            if (luaVM.State == RunningState.Faulted) Debug.Log(luaVM.ExceptionWhat);
        }
        else
        {
            ;   // trap here
        }
    }

    void OnDestroy()
    {
        luaVM?.Dispose();
    }
}

using System;
using EditorUIAdaptor;
using UnityEngine;
using Utils;

namespace CodeExecutor
{
    public class CodeService : SingletonMono<CodeService>
    {
        public enum WorkingState
        {
            Running,
            Stepping,
            Paused,
            Stopped,
        }
        
        private int fileOpCount = 0;
        private LuaVM luaVM;
        
        public WorkingState CurrentState { get; private set; } = WorkingState.Stopped;
        
        public string GetSafeFileName() { fileOpCount++; return fileOpCount.ToString(); }
        public bool CheckScriptExist(string scriptName) => EditorWindowManager.Instance.ProcessedWindowNames.Contains(scriptName.ToUpperInvariant());
        public void RenameExistScript(string oldName, string newName)
        {
            var nameCache = EditorWindowManager.Instance.ProcessedWindowNames;
            nameCache.Remove(oldName.ToUpperInvariant());
            nameCache.Add(newName.ToUpperInvariant());
            
            if (LuaVMLoader.Instance.LoadedScripts.Contains(EditorWindowManager.Instance.FindWindow(newName.ToUpperInvariant()))) StopExecute();
        }
        
        public void StartExecute(string scriptName)
        {
            if (CurrentState != WorkingState.Stopped) StopExecute();
            var window = EditorWindowManager.Instance.FindWindow(scriptName.ToUpperInvariant());
            if (window == null) throw new AccessViolationException("This should never happen!");
            
            window.SetRunningState(WorkingState.Running);
            var script = window.GetScript();

            // TODO:: create vm here with right status, this is only for test
            luaVM = new LuaVM(new LuaVMConfigurer(8,
                    vm =>
                    {
                        LuaVMAdaptorLib.AtomicCAS(vm);
                        LuaVMAdaptorLib.CheckThread(vm);
                        LuaVMAdaptorLib.GetCurrentThreadID(vm);
                        LuaVMAdaptorLib.HangupCurrentThread(vm);
                        LuaVMAdaptorLib.NewThread(vm);
                        LuaVMAdaptorLib.GetCurrentFrameCount(vm);
                        LuaVMAdaptorLib.GetPosition(vm);
                        LuaVMAdaptorLib.Move(vm);
                        LuaVMAdaptorLib.UseItem(vm);
                        LuaVMAdaptorLib.CanHarvest(vm);
                        LuaVMAdaptorLib.Harvest(vm);
                        LuaVMAdaptorLib.TrySetFacility(vm);
                        LuaVMAdaptorLib.InteractWith(vm);
                        vm.GetLuaVM().Options.DebugPrint = Debug.Log;
                    }, vm => { }, LuaVMAdaptorLib.CheckCurrentBotIsBusy),
                scriptName, script);
            LuaVMLoader.Instance.LoadedScripts.Add(window);
            CurrentState = WorkingState.Running;
        }
        public void StopExecute()
        {
            foreach (var editor in EditorWindowManager.Instance.GetAllWindows())
            {
                editor.SetRunningState(WorkingState.Stopped);
                editor.StopHighlightZone();
            }
            luaVM?.Dispose();
            LuaVMLoader.Instance.Clear();
            CurrentState = WorkingState.Stopped;
        }
        public void StepExecute(string scriptName)
        {
            if (CurrentState != WorkingState.Paused) StopExecute();
            if (CurrentState == WorkingState.Stopped) StartExecute(scriptName);
            CurrentState = WorkingState.Stepping;
        }
        public void PauseExecute()
        {
            if (CurrentState is not (WorkingState.Running or WorkingState.Stepping)) throw new AccessViolationException("This should never happen!");
            foreach (var editor in LuaVMLoader.Instance.LoadedScripts) editor.SetRunningState(WorkingState.Paused);
            CurrentState = WorkingState.Paused;
        }
        public void ResumeExecute()
        {
            if (CurrentState != WorkingState.Paused) throw new AccessViolationException("This should never happen!");
            foreach (var editor in LuaVMLoader.Instance.LoadedScripts) editor.SetRunningState(WorkingState.Running);
            CurrentState = WorkingState.Running;
        }
        
        private void Update()
        {
            switch (CurrentState)
            {
                case WorkingState.Running: 
                    if (luaVM.CouldResume())
                    {
                        var matchedBreakpoint = luaVM.ResumeUntilBreakPoint();
                        if (matchedBreakpoint) PauseExecute();
                    }
                    break;
                case WorkingState.Stepping: 
                    if (luaVM.CouldResume())
                    {
                        var matchedLineChange = luaVM.ResumeUntilNextStmt(LuaVMConfigurer.MaxInstructionPerResume);
                        if (matchedLineChange) PauseExecute();
                    }
                    break;
                case WorkingState.Paused:
                case WorkingState.Stopped: 
                    return;
                default: throw new AccessViolationException("This should never happen!");
            }

            foreach (var rtInfo in luaVM.RuntimeInfos)
            {
                var window = EditorWindowManager.Instance.FindWindow(rtInfo.ScriptName);
                window?.HighlightZone(rtInfo.CurrentLineStart-1, rtInfo.CurrentCharStart, rtInfo.CurrentLineEnd-1, rtInfo.CurrentCharEnd);
            }
            if (luaVM.CouldResume()) return;
            if (luaVM.State == RunningState.Faulted) Debug.Log(luaVM.ExceptionWhat.DecoratedMessage);    
            StopExecute();
            // TODO:: replace this later. need a better way to print exception
        }
    }
}
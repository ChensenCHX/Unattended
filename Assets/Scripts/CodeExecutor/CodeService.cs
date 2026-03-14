using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EditorUIAdaptor;
using EditorUIAdaptor.Behaviours;
using GlobalSettings;
using InGameTextEditor;
using Michsky.MUIP;
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
        public NotificationManager exceptionNotification;
        public NotificationManager printNotification;
        
        private int fileOpCount = 0;
        private string scriptDirPath;
        private LuaVM luaVM;
        private ScriptWatcher scriptWatcher;

        private void Start()
        {
            scriptDirPath = Path.Combine(Application.persistentDataPath, "Scripts");
            scriptWatcher = new ScriptWatcher(scriptDirPath);
        }

        public WorkingState CurrentState { get; private set; } = WorkingState.Stopped;
        
        public string GetSafeFileName() { fileOpCount++; return fileOpCount.ToString(); }
        public bool CheckScriptExist(string scriptName) => EditorWindowManager.Instance.FindWindow(scriptName) != null;
        public void RenameExistScript(string oldName, string newName)
        {
            if (LuaVMLoader.Instance.LoadedScripts.Contains(EditorWindowManager.Instance.FindWindow(newName))) StopExecute();
        }
        public void RemoveExistScript(EditorWindowHandler handler)
        {
            StopListeningOutsideChange(handler);
            if (LuaVMLoader.Instance.LoadedScripts.Contains(handler)) StopExecute();
        }
        
        public void AddBreakpoint(EditorWindowHandler windowHandler, int lineAt)
        {
            if (luaVM is null || !luaVM.CouldResume()) return;
            luaVM.AddBreakPoint(windowHandler.GetWindowName(), lineAt);
        }
        public void RemoveBreakpoint(EditorWindowHandler windowHandler, int lineAt)
        {
            if (luaVM is null || !luaVM.CouldResume()) return;
            luaVM.RemoveBreakPoint(windowHandler.GetWindowName(), lineAt);
        }
        public void ResetBreakpoint(EditorWindowHandler windowHandler) 
        {
            if (luaVM is null || !luaVM.CouldResume()) return;
            luaVM.SetBreakPoints(windowHandler.GetWindowName(), windowHandler.GetBreakpoints());
        }

        private bool printedBefore = true;
        private string printMessage = "";
        private void SendPrintMessage(string message)
        {
            printedBefore = false;
            printMessage = message;
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
                        vm.GetLuaVM().Options.DebugPrint = SendPrintMessage;
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
        
        public string GetScriptFilePath(EditorWindowHandler handler)
        {
            var scriptName = handler.GetWindowName();
            var filePath = Path.Combine(scriptDirPath, scriptName + ".lua");
            return filePath;
        }
        public void SaveScriptFile(EditorWindowHandler handler)
        {
            var filePath = GetScriptFilePath(handler);
            File.WriteAllText(filePath, handler.GetScript(), Encoding.UTF8);
        }
        
        public void StartListeningOutsideChange(EditorWindowHandler handler)
        {
            SaveScriptFile(handler);
            scriptWatcher.TargetScripts.Add(handler.GetWindowName());
        }
        public void StopListeningOutsideChange(EditorWindowHandler handler) => scriptWatcher.TargetScripts.Remove(handler.GetWindowName());
        
        private void Update()
        {
            scriptWatcher.RunTasks();
            if (!printedBefore)
            {
                printNotification.Close();
                var cutted = false;
                var str = printMessage;
                var pos = str.IndexOfNth('\n', 32);
                if (pos != -1) { cutted = true; pos = str.Length; }
                if (pos > 1024) { cutted = true; pos = 1024; }

                var msg = cutted ? str.Substring(0, pos) + "\n...(too long, truncated)" : str;
                printNotification.description = msg;
                printNotification.UpdateUI();
                printNotification.Open();
                printedBefore = true;
            }
            
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
                        var matchedLineChange = luaVM.ResumeUntilNextStmt(GlobalInfos.Instance.MaxVMInstructionPerResume);
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
            if (luaVM.State == RunningState.Faulted)
            {
                exceptionNotification.Close();
                var cutted = false;
                var str = luaVM.ExceptionWhat.DecoratedMessage;
                var pos = str.IndexOfNth('\n', 32);
                if (pos != -1) { cutted = true; pos = str.Length; }
                if (pos > 1024) { cutted = true; pos = 1024; }

                var msg = cutted ? str.Substring(0, pos) + "\n...(too long, truncated)" : str;
                exceptionNotification.description = msg;
                exceptionNotification.UpdateUI();
                exceptionNotification.Open();
            }
            StopExecute();
        }
    }
}

public class ScriptWatcher
{
    private enum EventType { Change, Create, Delete, Rename, Error }
    private struct EventStruct { public EventType EventType; public string Name; public Action Action; }
    
    private FileSystemWatcher watcher;
    private readonly ConcurrentQueue<EventStruct> mainThreadQueue = new();
    public readonly HashSet<string> TargetScripts = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
    
    public ScriptWatcher(string path)
    {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        
        watcher = new FileSystemWatcher(path)
        {
            Filter = "*.lua",                                                   // 监听所有lua脚本
            IncludeSubdirectories = false,                                      // 不考虑子目录
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite     // 只关心修改和文件名变化
        };

        watcher.Changed += OnChanged;
        watcher.Created += OnCreated;
        watcher.Deleted += OnDeleted;
        watcher.Renamed += OnRenamed;
        watcher.Error += OnError;

        watcher.EnableRaisingEvents = true;
    }

    public void RunTasks()
    {
        if (!mainThreadQueue.TryDequeue(out var lastResult)) return;
        while (mainThreadQueue.TryDequeue(out var thisResult))
        {
            if (thisResult.EventType == lastResult.EventType && thisResult.Name == lastResult.Name) { lastResult =  thisResult; return; }
            lastResult.Action.Invoke();
            lastResult = thisResult;
        }
        lastResult.Action.Invoke();
    }
    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        if (e.ChangeType != WatcherChangeTypes.Changed) return; // 只处理纯change
        var nameWithoutExt = Path.GetFileNameWithoutExtension(e.Name);
        if (!TargetScripts.Contains(nameWithoutExt)) return;
        mainThreadQueue.Enqueue(new EventStruct { 
            EventType = EventType.Change,
            Name = nameWithoutExt,
            Action = () =>
            {
                var handler = EditorWindowManager.Instance.FindWindow(nameWithoutExt);
                if (handler == null) throw new AccessViolationException("This should never happen!");
                var textEditor = handler.GetTextEditor();
                textEditor.CaretPosition = new TextPosition(0, 0);
                textEditor.Text = File.ReadAllText(e.FullPath);
            }
        });
    }
    private void OnCreated(object sender, FileSystemEventArgs e)
    {
        if (IsDirectory(e.FullPath)) return;                                                                // 过滤文件夹
        var nameWithoutExt = Path.GetFileNameWithoutExtension(e.Name);
        if (!IsValidLuaFileName(nameWithoutExt)) return;
        mainThreadQueue.Enqueue(new EventStruct {
            EventType = EventType.Create,
            Name = nameWithoutExt,
            Action = () =>
            {
                if (EditorWindowManager.Instance.FindWindow(nameWithoutExt) != null) return;
                EditorWindowManager.Instance.CreateEditorWindow(nameWithoutExt);
            }
        });
    }
    private void OnRenamed(object sender, RenamedEventArgs e)
    {
        if (IsDirectory(e.FullPath)) return;                                                                // 过滤文件夹
        var nameWithoutExt = Path.GetFileNameWithoutExtension(e.Name);
        var oldNameWithoutExt = Path.GetFileNameWithoutExtension(e.OldName);
        if (!TargetScripts.Contains(oldNameWithoutExt)) return;
        mainThreadQueue.Enqueue(new EventStruct {
            EventType = EventType.Rename,
            Name = oldNameWithoutExt,
            Action = () =>
            {
                var handler = EditorWindowManager.Instance.FindWindow(oldNameWithoutExt);
                if (handler == null) throw new AccessViolationException("This should never happen!");
                handler.GetScriptNameAdjustor().ScriptName = nameWithoutExt;
                TargetScripts.Remove(oldNameWithoutExt);
                TargetScripts.Add(nameWithoutExt);
            }
        });
    }
    private void OnDeleted(object sender, FileSystemEventArgs e)
    {
        var nameWithoutExt = Path.GetFileNameWithoutExtension(e.Name);
        if (!TargetScripts.Contains(nameWithoutExt)) return;
        mainThreadQueue.Enqueue(new EventStruct {
            EventType = EventType.Delete,
            Name = nameWithoutExt,
            Action = () =>
            {
                TargetScripts.Remove(nameWithoutExt);
                EditorWindowManager.Instance.RemoveEditorWindow(nameWithoutExt);
            }
        });
    }
    private void OnError(object sender, ErrorEventArgs e) => mainThreadQueue.Enqueue(new EventStruct {
        EventType = EventType.Error,
        Name = e.ToString(),
        Action = () => Debug.LogError($"外部变更监听错误: {e.GetException().Message}")
    });
    
    private static bool IsDirectory(string path)
    {
        try { return (File.GetAttributes(path) & FileAttributes.Directory) != 0; }
        catch { return true; }  // 默认视为文件夹 不执行事件
    }
    private static bool IsValidLuaFileName(string fileNameWithoutExtension)
    {
        foreach (var c in fileNameWithoutExtension)
            if (c is not (>= 'a' and <= 'z' or >= 'A' and <= 'Z' or >= '0' and <= '9' or '_')) return false;
        return true;
    }
    
    public void Dispose()
    {
        if (watcher == null) return;
        watcher.EnableRaisingEvents = false;
        watcher.Dispose();
        watcher = null;
    }
    ~ScriptWatcher() => Dispose();
}
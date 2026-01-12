using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Debugging;

// Warning: for init accessor
namespace System.Runtime.CompilerServices { internal static class IsExternalInit { } }

namespace CodeExecutor
{
    internal readonly struct LuaVMRuntimeInfo : IEquatable<LuaVMRuntimeInfo>
    {
        # region 属性
        public int ThreadID { get; init; }
        public int CurrentLineStart { get; init; }
        public int CurrentLineEnd { get; init; }
        public int CurrentCharStart { get; init; }
        public int CurrentCharEnd { get; init; }
        # endregion
        
        # region 方法
        public override bool Equals(object obj)
        {
            if (obj is not LuaVMRuntimeInfo info) return false; 
            return ThreadID == info.ThreadID;
        }
        public bool Equals(LuaVMRuntimeInfo other) => ThreadID == other.ThreadID;
        public override int GetHashCode() => ThreadID.GetHashCode();
        # endregion
    }
    internal class LuaVMInfoHook : IDebugger
    {
        # region 内部量
        private readonly Dictionary<string, Tuple<SourceCode, HashSet<int>>> codes = new();
        private DebugService dbgSvc;
        private readonly DebuggerAction dbgAction = new DebuggerAction { Action = DebuggerAction.ActionType.StepIn };
        private readonly List<DynamicExpression> _dummyList = new List<DynamicExpression>();
        private readonly HashSet<LuaVMRuntimeInfo> runtimeInfos;
        # endregion
        
        # region 外部状态
        public Coroutine CurrentThread { get; set; }
        public InfoHookMode Mode { get; set; } = InfoHookMode.Normal;
        public bool MatchBreakpoint { get; private set; }
        # endregion

        # region IDebugger接口方法
        public DebuggerCaps GetDebuggerCaps() => DebuggerCaps.CanDebugSourceCode;
        public void SetDebugService(DebugService debugService) => dbgSvc = debugService;
        public void SetSourceCode(SourceCode src) => codes[src.Name] = Tuple.Create(src, new HashSet<int>());
        public void SetByteCode(string[] byteCode) { }
        public bool IsPauseRequested() => false;
        public bool SignalRuntimeException(ScriptRuntimeException ex) => false;
        public DebuggerAction GetAction(int ip, SourceRef sourceref)
        {
            if (sourceref is null) return dbgAction;

            runtimeInfos.Add(new LuaVMRuntimeInfo() {
                ThreadID = CurrentThread.ReferenceID,
                CurrentLineStart = sourceref.FromLine,
                CurrentLineEnd = sourceref.ToLine,
                CurrentCharStart = sourceref.FromChar,
                CurrentCharEnd = sourceref.ToChar,
            });
            
            MatchBreakpoint = sourceref.Breakpoint;
            if (MatchBreakpoint && Mode == InfoHookMode.LineBreakPoint) CurrentThread.AutoYieldCounter = 0;
            if (Mode == InfoHookMode.NextLine) CurrentThread.AutoYieldCounter = 0;
            
            return dbgAction;
        }
        public void SignalExecutionEnded() { }
        public void Update(WatchType watchType, IEnumerable<WatchItem> items) { }
        public List<DynamicExpression> GetWatchItems() => _dummyList;
        public void RefreshBreakpoints(IEnumerable<SourceRef> refs) { }
        # endregion
        
        # region 自定义方法及构造
        public void AddLineBreakpoint(string srcName, int line)
        {
            if (!codes.TryGetValue(srcName, out var srcTuple)) return;
            srcTuple.Item2.Add(line);
            dbgSvc.ResetBreakPoints(srcTuple.Item1, srcTuple.Item2);
        }
        public void RemoveLineBreakpoint(string srcName, int line, bool all=false)
        {
            if (!codes.TryGetValue(srcName, out var srcTuple)) return;
            if (all) srcTuple.Item2.Clear(); else srcTuple.Item2.Remove(line);
            dbgSvc.ResetBreakPoints(srcTuple.Item1, srcTuple.Item2);
        }
        public void ResetLineBreakpoints(string srcName, HashSet<int> lines)
        {
            if (!codes.TryGetValue(srcName, out var srcTuple)) return;
            srcTuple.Item2.Clear();
            srcTuple.Item2.UnionWith(lines);
            dbgSvc.ResetBreakPoints(srcTuple.Item1, srcTuple.Item2);
        }
        public LuaVMInfoHook(HashSet<LuaVMRuntimeInfo> rtInfos) => runtimeInfos = rtInfos;
        # endregion
    }
}
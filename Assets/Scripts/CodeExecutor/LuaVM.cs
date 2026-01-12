using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace CodeExecutor
{
    public class LuaVM
    {
        # region 内部状态量
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable : for keeping refrence
        private readonly Script luaVM;
        private readonly LuaVMInfoHook luaVMInfoHook;
        private readonly LuaVMConfigurer luaVMConfigurer;
        private readonly HashSet<Coroutine> userThreads = new();
        private readonly HashSet<LuaVMRuntimeInfo> runtimeInfos = new();
        # endregion
        
        # region 外部状态量
        public RunningState State { get; private set; }
        public InterpreterException ExceptionWhat { get; private set; }
        public IReadOnlyCollection<LuaVMRuntimeInfo> RuntimeInfos => runtimeInfos;
        # endregion
        
        # region 执行指令
        /// 是否仍可恢复执行
        public bool CouldResume()
        {
            switch (State)
            {
                case RunningState.Ready:
                case RunningState.Waiting:
                    return true;
                case RunningState.Finished:
                case RunningState.Faulted:
                case RunningState.Terminated:
                default:
                    return false;
            }
        }
        /// 执行指定步长 (虚拟机指令数)
        private void ResumeWithStep(Coroutine userThread, long maxInstructionCount=1)
        {
            if (!CouldResume())
                throw new InvalidOperationException($"Invalid state: try to resume with state: {State.ToString()}");
            
            userThread.AutoYieldCounter = maxInstructionCount;
            luaVMInfoHook.RefreshState(userThread);
            try
            {
                while (true)
                {
                    var result = userThread.Resume();
                    if (result.Type != DataType.YieldRequest)
                    {
                        userThreads.Remove(userThread);
                        runtimeInfos.Remove(new LuaVMRuntimeInfo() { ThreadID = userThread.ReferenceID });
                        if (userThreads.Count == 0) State = RunningState.Finished;
                        return;
                    }

                    if (result.YieldRequest.Forced) break;
                }
            }
            catch (ScriptRuntimeException se)
            {
                ExceptionWhat = se;
                State = RunningState.Faulted;
                return;
            }
            
            State = RunningState.Waiting;
        }
        public void ResumeUntilLimit(int maxInstructionCount)
        {
            foreach (var userThread in userThreads) ResumeWithStep(userThread, maxInstructionCount);
        }
        /// 执行到下一语句 (最多执行maxStep条指令) <returns>是否到达下一语句</returns>
        public bool ResumeUntilNextStmt(int maxInstructionCount)
        {
            var statementChanged = false;
            luaVMInfoHook.Mode = InfoHookMode.NextLine;
            foreach (var userThread in userThreads)
            {
                ResumeWithStep(userThread, maxInstructionCount);
                statementChanged |= luaVMInfoHook.StatementChanged;
            }
            luaVMInfoHook.Mode = InfoHookMode.Normal;
            
            return statementChanged;
        }
        /// 执行到断点 <returns>达到虚拟机单次指令上限前是否到达任意有效断点行(不包括当前行!)</returns>
        public bool ResumeUntilBreakPoint()
        {
            var matchBreakPoint = false;
            luaVMInfoHook.Mode = InfoHookMode.LineBreakPoint;
            foreach (var userThread in userThreads)
            {
                ResumeWithStep(userThread, LuaVMConfigurer.MaxInstructionPerResume);
                matchBreakPoint |= luaVMInfoHook.MatchedBreakpoint;
            }
            luaVMInfoHook.Mode = InfoHookMode.Normal;

            return matchBreakPoint;
        }
        # endregion
        
        # region 断点设置
        public void AddBreakPoint(string scriptName, int line) 
            => luaVMInfoHook.AddLineBreakpoint(scriptName, line);
        public void RemoveBreakPoint(string scriptName, int line) 
            => luaVMInfoHook.RemoveLineBreakpoint(scriptName, line);
        public void SetBreakPoints(string scriptName, HashSet<int> lines)
            => luaVMInfoHook.ResetLineBreakpoints(scriptName, lines);
        # endregion
        
        # region 构造与析构
        public LuaVM(LuaVMConfigurer configurer, string scriptName, string scriptCode)
        {
            luaVM = new Script(configurer.GetCoreModules());
            configurer.CustomModify(luaVM);
            
            DynValue userCode = null;
            try { userCode = luaVM.LoadString(scriptCode, luaVM.Globals, scriptName); }
            catch(SyntaxErrorException e) { ExceptionWhat = e; State = RunningState.Faulted; }

            if (userCode is null) return;
            if (userCode.Type != DataType.Function)
            {
                ExceptionWhat = new LuaVMException("Fatal error: can't compile user code, vm internal error.");
                State = RunningState.Faulted; 
                return;
            }
            
            DynValue coroutine = luaVM.CreateCoroutine(userCode);
            if (coroutine.Type != DataType.Thread)
            {
                ExceptionWhat = new LuaVMException("Fatal error: can't create user thread, vm internal error.");
                State = RunningState.Faulted;
                return;
            }
            
            userThreads.Add(coroutine.Coroutine);
            luaVMInfoHook = new LuaVMInfoHook(runtimeInfos);
            luaVMConfigurer = configurer;
            
            luaVM.AttachDebugger(luaVMInfoHook);
            luaVM.DebuggerEnabled = true;
            State = RunningState.Ready;
        }
        public void Dispose() { State = RunningState.Terminated; luaVMConfigurer.CustomDispose(luaVM); }
        ~LuaVM() { if (State != RunningState.Terminated) Dispose(); }
        # endregion
    }
}
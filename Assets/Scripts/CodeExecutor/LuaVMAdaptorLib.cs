using MoonSharp.Interpreter;

namespace CodeExecutor
{
    public class LuaVMAdaptorLib
    {
        public static void NewThread(LuaVM luaVM){
            var vm = luaVM.GetLuaVM();
            vm.Globals.Set("new_thread", DynValue.NewCallback((ctx, args) =>
            {
                var func = args.AsType(0, "new_thread", DataType.Function);
                var thread = vm.CreateCoroutine(func).Coroutine;
                var success = luaVM.AttachThread(thread);
                return success ? DynValue.NewNumber(thread.ReferenceID) : DynValue.False;
            }));
        }
        public static void CheckThread(LuaVM luaVM){
            var vm = luaVM.GetLuaVM();
            vm.Globals.Set("check_thread", DynValue.NewCallback((ctx, args) =>
            {
                var threadID = args.AsInt(0, "check_thread");
                var state = luaVM.GetThreadState(threadID);
                return state == CoroutineState.Dead ? DynValue.False : DynValue.True;
            }));
        }
        public static void HangupCurrentThread(LuaVM luaVM){
            var vm = luaVM.GetLuaVM();
            vm.Globals.Set("hangup_current_thread", DynValue.NewCallback((ctx, args) =>
            {
                ctx.GetCallingCoroutine().AutoYieldCounter = 0;
                return DynValue.Void;
            }));
        }
        public static void AtomicCAS(LuaVM luaVM)
        {
            var vm = luaVM.GetLuaVM();
            vm.Globals.Set("atomic_compare_and_swap_at", DynValue.NewCallback((ctx, args) =>
            {
                var table = args.AsType(0, "atomic_compare_and_swap_at", DataType.Table);
                var key = args[1];
                if (key.Type == DataType.Nil)
                    throw ScriptRuntimeException.BadArgument(1, "atomic_compare_and_swap_at", "any value", "nil", false);
                var oldValue = args[2];
                if (oldValue.Type == DataType.Void) oldValue = DynValue.NewNil();
                var newValue = args[3];
                if (newValue.Type == DataType.Void) newValue = DynValue.NewNil();
                if (table.Table.Get(key).Equals(oldValue)) table.Table.Set(key, newValue);
                
                return oldValue;
            }));
        }
        public static void RefreshThreadID(LuaVM luaVM, Coroutine currThread)
        {
            var vm = luaVM.GetLuaVM();
            vm.Globals.Set("__ThreadID__", DynValue.NewNumber(currThread.ReferenceID));
        }
    }
}
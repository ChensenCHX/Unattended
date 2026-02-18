using System;
using Bots;
using Items;
using MoonSharp.Interpreter;
using UnityEngine;
using Workspace.Facilities;
using Coroutine = MoonSharp.Interpreter.Coroutine;

namespace CodeExecutor
{
    public static class LuaVMAdaptorLib
    {
        public static void NewThread(LuaVM luaVM){
            var vm = luaVM.GetLuaVM();
            vm.Globals.Set("new_thread", DynValue.NewCallback((ctx, args) =>
            {
                var func = args.AsType(0, "new_thread", DataType.Function);
                var thread = vm.CreateCoroutine(func).Coroutine;
                var haveBot = BotManager.Instance.GetBotByID(ctx.GetCallingCoroutine().ReferenceID, out var bot);
                if (!haveBot) throw new ScriptRuntimeException("Fatal error: try alloc new bot but have no known father.");
                
                var success = luaVM.AttachThread(thread, bot.X, bot.Y);
                return success ? DynValue.NewNumber(thread.ReferenceID) : DynValue.False;
            }));
        }
        public static void CheckThread(LuaVM luaVM){
            var vm = luaVM.GetLuaVM();
            vm.Globals.Set("check_thread", DynValue.NewCallback((_, args) =>
            {
                var threadID = args.AsInt(0, "check_thread");
                var state = luaVM.GetThreadState(threadID);
                return state == CoroutineState.Dead ? DynValue.False : DynValue.True;
            }));
        }
        public static void HangupCurrentThread(LuaVM luaVM){
            var vm = luaVM.GetLuaVM();
            vm.Globals.Set("hangup_current_thread", DynValue.NewCallback((ctx, _) =>
            {
                ctx.GetCallingCoroutine().AutoYieldCounter = 0;
                return DynValue.Void;
            }));
        }
        public static void AtomicCAS(LuaVM luaVM)
        {
            var vm = luaVM.GetLuaVM();
            vm.Globals.Set("atomic_compare_and_swap_at", DynValue.NewCallback((_, args) =>
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
        public static void GetCurrentThreadID(LuaVM luaVM)
        {
            var vm = luaVM.GetLuaVM();
            vm.Globals.Set("get_current_thread", DynValue.NewCallback((ctx, _)
                => DynValue.NewNumber(ctx.GetCallingCoroutine().ReferenceID))
            );
        }
        public static void GetCurrentFrameCount(LuaVM luaVM)
        {
            var vm = luaVM.GetLuaVM();
            vm.Globals.Set("get_current_frame_count", DynValue.NewCallback((_, _) => DynValue.NewNumber(Time.frameCount)));
        }
        public static void GetPosition(LuaVM luaVM)
        {
            var vm = luaVM.GetLuaVM();
            vm.Globals.Set("get_x_pos", DynValue.NewCallback((ctx, _) =>
                {
                    var haveBot = BotManager.Instance.GetBotByID(ctx.GetCallingCoroutine().ReferenceID, out var bot);
                    if (!haveBot) throw new ScriptRuntimeException("Fatal error: cannot find this thread's bot.");

                    return DynValue.NewNumber(bot.X);
                }
            ));
            vm.Globals.Set("get_y_pos", DynValue.NewCallback((ctx, _) =>
                {
                    var haveBot = BotManager.Instance.GetBotByID(ctx.GetCallingCoroutine().ReferenceID, out var bot);
                    if (!haveBot) throw new ScriptRuntimeException("Fatal error: cannot find this thread's bot.");

                    return DynValue.NewNumber(bot.Y);
                }
            ));
        }
        
        public static void Move(LuaVM luaVM)
        {
            var vm = luaVM.GetLuaVM();
            vm.Globals.Set("move", DynValue.NewCallback((ctx, args) =>
                {
                    var haveBot = BotManager.Instance.GetBotByID(ctx.GetCallingCoroutine().ReferenceID, out var bot);
                    if (!haveBot) throw new ScriptRuntimeException("Fatal error: cannot find this thread's bot.");
                    
                    var direction = args.AsInt(0, "move");
                    ctx.GetCallingCoroutine().AutoYieldCounter = 0;     // 涉及Bot移动的操作都需要立即让出当前执行
                    switch (direction)
                    {
                        case 1:
                            bot.Move(Vector3.right); break;
                        case 2:
                            bot.Move(Vector3.forward); break;
                        case 3:
                            bot.Move(Vector3.left); break;
                        case 4:
                            bot.Move(Vector3.back); break;
                        default:
                            return DynValue.False;
                    }
                    return DynValue.True;
                }
            ));
        }
        public static void UseItem(LuaVM luaVM)
        {
            var vm = luaVM.GetLuaVM();
            vm.Globals.Set("use_item", DynValue.NewCallback((ctx, args) =>
                {
                    var haveBot = BotManager.Instance.GetBotByID(ctx.GetCallingCoroutine().ReferenceID, out var bot);
                    if (!haveBot) throw new ScriptRuntimeException("Fatal error: cannot find this thread's bot.");
                    
                    var item = args.AsInt(0, "use_item");
                    if (!Enum.IsDefined(typeof(ItemType), item)) throw new ScriptRuntimeException($"Error: type '{item}' is not a valid item type.");

                    return bot.TryAddItem((ItemType)item);
                }
            ));
        }
        public static void CanHarvest(LuaVM luaVM)
        {
            var vm = luaVM.GetLuaVM();
            vm.Globals.Set("can_harvest", DynValue.NewCallback((ctx, _) =>
                {
                    var haveBot = BotManager.Instance.GetBotByID(ctx.GetCallingCoroutine().ReferenceID, out var bot);
                    if (!haveBot) throw new ScriptRuntimeException("Fatal error: cannot find this thread's bot.");
                    
                    return bot.CanHarvest();
                }
            ));
        }
        public static void Harvest(LuaVM luaVM)
        {
            var vm = luaVM.GetLuaVM();
            vm.Globals.Set("harvest", DynValue.NewCallback((ctx, _) =>
                {
                    var haveBot = BotManager.Instance.GetBotByID(ctx.GetCallingCoroutine().ReferenceID, out var bot);
                    if (!haveBot) throw new ScriptRuntimeException("Fatal error: cannot find this thread's bot.");
                    bot.Harvest();
                    ctx.GetCallingCoroutine().AutoYieldCounter = 0;     // 涉及Bot移动的操作都需要立即让出当前执行
                    return DynValue.Nil;
                }
            ));
        }
        public static void TrySetFacility(LuaVM luaVM)
        {
            var vm = luaVM.GetLuaVM();
            vm.Globals.Set("build", DynValue.NewCallback((ctx, args) =>
                {
                    var haveBot = BotManager.Instance.GetBotByID(ctx.GetCallingCoroutine().ReferenceID, out var bot);
                    if (!haveBot) throw new ScriptRuntimeException("Fatal error: cannot find this thread's bot.");
                    
                    var type = args.AsInt(0, "build");
                    ctx.GetCallingCoroutine().AutoYieldCounter = 0;     // 涉及Bot移动的操作都需要立即让出当前执行
                    if (!Enum.IsDefined(typeof(FacilityType), type)) throw new ScriptRuntimeException($"Error: type '{type}' is not a valid type.");
                    return bot.TrySetFacility((FacilityType)type);
                }
            ));
        }
        public static void InteractWith(LuaVM luaVM)
        {
            var vm = luaVM.GetLuaVM();
            vm.Globals.Set("interact_with", DynValue.NewCallback((ctx, args) => 
                {
                    var haveBot = BotManager.Instance.GetBotByID(ctx.GetCallingCoroutine().ReferenceID, out var bot);
                    if (!haveBot) throw new ScriptRuntimeException("Fatal error: cannot find this thread's bot.");
                    
                    return bot.InteractWith(args);
                }
            ));
        }
        
        public static bool CheckCurrentBotIsBusy(LuaVM luaVM, Coroutine thread)
        {
            if (!BotManager.Instance.GetBotByID(thread.ReferenceID, out var bot)) 
                throw new ScriptRuntimeException($"Fatal error: thread id {thread.ReferenceID} have no bot!");

            return bot.BotIsWorking;
        }
    }
}
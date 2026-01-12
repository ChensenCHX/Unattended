using System;
using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter;

namespace CodeExecutor
{
    public class LuaVMConfigurer
    {
        private static readonly IReadOnlyList<CoreModules> STDList = new List<CoreModules>() {
            CoreModules.Basic,
            CoreModules.GlobalConsts | CoreModules.ErrorHandling,
            CoreModules.TableIterators,
            CoreModules.Bit32 | CoreModules.Math,
            CoreModules.String,
            CoreModules.Table,
            CoreModules.LoadMethods,
            CoreModules.Metatables,
        };
        private readonly int coreModulesLevel;

        public static readonly int OriginalMaxInstructionPerResume = 100;
        public static int MaxInstructionPerResume = 100;
        public static readonly int MaxThreadCount = 32;
        public static int CurrentThreadCount = 0;
        
        public Action<LuaVM> OnStartVM { get; private set; }
        public Action<LuaVM> OnDispose { get; private set; }
        public Action<LuaVM, Coroutine> OnThreadSwitch { get; private set; }

        public CoreModules GetCoreModules() => STDList
            .Take(Math.Min(8, Math.Max(0, coreModulesLevel)))
            .Aggregate((all, self) => all | self);
        public LuaVMConfigurer(int coreModulesLevel, 
            Action<LuaVM> onStartVM, Action<LuaVM> onDispose,
            Action<LuaVM, Coroutine> onThreadSwitch)
        {
            this.coreModulesLevel = coreModulesLevel;
            OnStartVM = onStartVM;
            OnDispose = onDispose;
            OnThreadSwitch = onThreadSwitch;
        }
    }
}
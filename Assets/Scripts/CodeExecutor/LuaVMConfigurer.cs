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
        public static readonly int OriginalMaxInstructionPerResume = 100;
        public static int MaxInstructionPerResume = 100;
        
        private readonly int coreModulesLevel;
        public Action<Script> OnStartVM { get; private set; }
        public Action<Script> OnDispose { get; private set; }
        public Action<Script, Coroutine> OnThreadSwitch { get; private set; }

        public CoreModules GetCoreModules() => STDList
            .Take(Math.Min(8, Math.Max(0, coreModulesLevel)))
            .Aggregate((all, self) => all | self);
        public LuaVMConfigurer(int coreModulesLevel, 
            Action<Script> onStartVM, Action<Script> onDispose,
            Action<Script, Coroutine> onThreadSwitch)
        {
            this.coreModulesLevel = coreModulesLevel;
            OnStartVM = onStartVM;
            OnDispose = onDispose;
            OnThreadSwitch = onThreadSwitch;
        }
    }
}
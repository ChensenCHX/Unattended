using System.Collections.Generic;
using EditorUIAdaptor;
using EditorUIAdaptor.Behaviours;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using Utils;

namespace CodeExecutor
{
    public class LuaVMLoader : Singleton<LuaVMLoader>, IScriptLoader
    {
        public readonly HashSet<EditorWindowHandler> LoadedScripts = new();
        public void Clear() => LoadedScripts.Clear();
        public object LoadFile(string file, Table globalContext)
        {
            var window = EditorWindowManager.Instance.FindWindow(file.ToUpperInvariant());
            if (window == null) throw new ScriptRuntimeException($"Couldn't find script '{file}'.");
            LoadedScripts.Add(window);
            window.SetRunningState(CodeService.WorkingState.Running);
            return window.GetScript();
        }

        public string ResolveFileName(string filename, Table globalContext) => filename;
        public string ResolveModuleName(string modname, Table globalContext) => modname;
    }
}
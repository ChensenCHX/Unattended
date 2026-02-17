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
        private readonly Dictionary<string, string> scriptCache = new();
        public void Clear() => LoadedScripts.Clear();
        public object LoadFile(string file, Table globalContext)
        {
            if (scriptCache.TryGetValue(file, out var loadedFile)) return loadedFile;
            var window = EditorWindowManager.Instance.FindWindow(file.ToLowerInvariant());
            if (window == null) throw new LuaVMException($"Couldn't find script '{file}'.");
            LoadedScripts.Add(window);
            window.SetRunningState(CodeService.WorkingState.Running);
            scriptCache[file] = window.GetScript();
            return scriptCache;
        }

        public string ResolveFileName(string filename, Table globalContext) => filename;
        public string ResolveModuleName(string modname, Table globalContext) => modname;
    }
}
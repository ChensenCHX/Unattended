using System;
using Utils;

namespace CodeExecutor
{
    public class CodeService : SingletonMono<CodeService>
    {
        private int fileOpCount = 0;
        
        public string GetSafeFileName() { fileOpCount++; return fileOpCount.ToString(); }
        public bool CheckScriptExist(string scriptName)
        {
            // TODO:: impl it later
            // tips: all script name should compared as ToUpperInvariant()
            return false;
            throw new NotImplementedException();
        }
        public bool RenameExistScript(string oldName, string newName)
        {
            // TODO:: impl it later
            return true;
            throw new NotImplementedException();
        }
    }
}
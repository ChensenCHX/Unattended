using System;
using MoonSharp.Interpreter;

namespace CodeExecutor
{
    internal class LuaVMException : InterpreterException
    {
        public LuaVMException(Exception ex) : base(ex) { }       
        public LuaVMException(string message) : base(message) { }
    }
}
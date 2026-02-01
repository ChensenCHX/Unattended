using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace GlobalSettings
{
    public static class DynValueCache
    {
        private static readonly Dictionary<int, DynValue> intCache = new();
        private static readonly Dictionary<string, DynValue> strCache = new();
        public static DynValue NewNumber(int value) => intCache.ContainsKey(value) ? intCache[value] : intCache[value] = DynValue.NewNumber(value);
        public static DynValue NewString(string value) => strCache.ContainsKey(value) ? strCache[value] : strCache[value] = DynValue.NewString(value);
    }
}
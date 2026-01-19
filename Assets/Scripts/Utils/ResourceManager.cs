using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils
{
    public static class ResourceManager<T> where T : Object
    {
        private static readonly Dictionary<string, T> ResourcesTable = new();
        private static readonly Dictionary<string, bool> Paths = new();
        
        public static void AddSearchPath(string path) => Paths.TryAdd(path.Replace('\\', '/'), false); 
        public static void RemoveSearchPath(string path) => Paths.Remove(path.Replace('\\', '/'));
        public static void ClearSearchPath() => Paths.Clear();
        public static IReadOnlyList<string> GetSearchPaths() => Paths.Keys.ToList();

        public static void UnloadAll(bool clearPaths=false)
        {
            ResourcesTable
                .Select(pair => pair.Value)
                .ToList()
                .ForEach(Resources.UnloadAsset);
            ResourcesTable.Clear();

            if (clearPaths) { ClearSearchPath(); return; }
            foreach (var (path, _) in Paths) Paths[path] = false;
        }
        public static void LoadAll(bool reload=false)
        {
            if (reload) UnloadAll();

            Paths
                .Where(pair => !pair.Value)
                .Select(pair => pair.Key)
                .ToList()
                .ConvertAll(Resources.LoadAll<T>)
                .SelectMany(value => value)
                .ToList()
                .ForEach(resource => ResourcesTable[resource.name] = resource);
        }

        public static T GetResource(string resourceName) => ResourcesTable.GetValueOrDefault(resourceName);
    }
}
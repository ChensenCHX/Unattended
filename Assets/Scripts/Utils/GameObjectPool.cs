using System.Collections.Generic;
using UnityEngine;
using Workspace;
using Workspace.Facilities.Impl;

namespace Utils
{
    public interface IPoolable<T> where T : MonoBehaviour
    {
        void FreeThis();
        void OnAlloc();
    }
    
    public static class GameObjectPool<T> where T : MonoBehaviour, IPoolable<T>
    {
        private static readonly Stack<T> pool = new();
        public static T Alloc(GameObject prefab)
        {
            if (pool.Count == 0)
            {
                var obj = Object.Instantiate(prefab, WorkspaceManager.Instance.transform);
                var objMono = obj.GetComponent<T>();
                objMono.OnAlloc();
                return objMono;
            }
            
            var curr = pool.Pop();
            curr.gameObject.SetActive(true);
            curr.OnAlloc();
            return curr;
        }

        public static void Free(T obj)
        {
            obj.gameObject.SetActive(false);
            pool.Push(obj);
        }
    }
}
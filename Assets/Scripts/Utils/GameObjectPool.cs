using System.Collections.Generic;
using UnityEngine;
using Workspace;
using Workspace.Facilities.Impl;

namespace Utils
{
    public interface IPoolAble<T> where T : MonoBehaviour
    {
        void OnFree();
        void OnAlloc();
    }
    
    public static class GameObjectPool<T> where T : MonoBehaviour, IPoolAble<T>
    {
        private static readonly List<T> pool = new();
        private static int currPtr = 0;
        public static T Alloc(GameObject prefab)
        {
            if (currPtr < 0)
            {
                var obj = Object.Instantiate(prefab, WorkspaceManager.Instance.transform);
                var objMono = obj.GetComponent<T>();
                objMono.OnAlloc();
                return objMono;
            }
            
            var curr = pool[currPtr]; currPtr--;
            curr.gameObject.SetActive(true);
            curr.OnAlloc();
            return curr;
        }

        public static void Free(T obj)
        {
            obj.OnFree();
            obj.transform.SetParent(GameObjectPoolHolder.Instance.Transform);
            obj.gameObject.SetActive(false);
            pool.Add(obj);
            currPtr++;
        }
    }
}
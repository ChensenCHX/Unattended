using System;
using UnityEngine;

namespace Utils
{
    public class Singleton<T> where T : new()
    { 
        private static readonly object _lock = new object();
        public static T Instance { get; } = new T();
        public Singleton() { lock(_lock) if(Instance is not null) throw new InvalidOperationException($"Cannot new '{Instance.GetType()}' (a singleton class)."); }
    }

    // 我放弃了 除非用反射否则无法阻拦用户搞出someObj.gameObject.AddComponent<T>(),其中T是一个SingletonMono<T>的情况
    public class SingletonMonoAuto<T> : MonoBehaviour where T : SingletonMonoAuto<T>
    {
        private static readonly object _lock = new object();
        private static T _instance;

        private static T __CreateInstance()
        {
            return new GameObject(nameof(T) + "(Singleton)").AddComponent<T>();
        }
        
        public static T Instance { 
        get {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null) _instance = __CreateInstance();
                }
            }
            return _instance;
        } }
    }
    
    public class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
    {
        private static readonly object _lock = new();
        private static T _instance;
        protected virtual bool DontDestroy => false;

        protected virtual void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            
            if (!DontDestroy) return;
            
            DontDestroyOnLoad(gameObject);
        }

        public static T Instance { get { lock (_lock) return _instance == null ? _instance = FindObjectOfType<T>() : _instance; } }
    }
}
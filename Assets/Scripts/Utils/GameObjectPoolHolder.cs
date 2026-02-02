using UnityEngine;

namespace Utils
{
    public class GameObjectPoolHolder : SingletonMono<GameObjectPoolHolder>
    {
        public GameObject GameObject => gameObject;
        public Transform Transform => transform;
    }
}
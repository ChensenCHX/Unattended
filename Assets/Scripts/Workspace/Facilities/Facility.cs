using DG.Tweening;
using MoonSharp.Interpreter;
using Items;
using UnityEngine;
using Utils;

namespace Workspace.Facilities
{
    public abstract class Facility : MonoBehaviour, IPoolable<Facility>
    {
        # region 属性
        public abstract FacilityType Type { get; }
        public abstract double Progress { get; }
        public abstract int X { get; }
        public abstract int Y { get; }
        # endregion
        
        # region 玩家方法
        public abstract DynValue InteractWith(CallbackArguments args);
        public abstract DynValue TryAddItem(ItemType item);
        public abstract DynValue CanHarvest();
        public abstract void Harvest();
        # endregion

        private void OnDestroy() => transform.DOKill();

        public virtual void FreeThis() { }
        public virtual void OnAlloc() { }
    }
}

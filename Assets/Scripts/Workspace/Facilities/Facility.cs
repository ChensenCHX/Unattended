using DG.Tweening;
using MoonSharp.Interpreter;
using Items;
using UnityEngine;

namespace Workspace.Facilities
{
    public abstract class Facility : MonoBehaviour
    {
        # region 属性
        public abstract FacilityType Type { get; }
        public abstract double Progress { get; }
        public abstract int X { get; }
        public abstract int Y { get; }
        # endregion
        
        # region 玩家方法
        public abstract DynValue GetUniqueState();
        public abstract DynValue InteractWith(DynValue argTuple);
        public abstract DynValue TryAddItem(ItemType item, int count);
        public abstract DynValue CanHarvest();
        public abstract void Harvest();
        # endregion

        private void OnDestroy() => transform.DOKill();
    }
}

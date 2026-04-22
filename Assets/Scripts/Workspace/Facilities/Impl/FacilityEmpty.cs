using DG.Tweening;
using MoonSharp.Interpreter;
using UnityEngine;
using Items;
using Utils;

namespace Workspace.Facilities.Impl
{
    public class FacilityEmpty : Facility, IPoolable<FacilityEmpty>
    {
        public override FacilityType Type { get; } = FacilityType.Empty;
        public override Tween GrowthTween { get; } = null;
        public override double Progress { get; } = 1;
        public override int X => Mathf.RoundToInt(transform.position.x);
        public override int Y => Mathf.RoundToInt(transform.position.z);

        public override DynValue InteractWith(CallbackArguments args) => DynValue.Nil;
        public override DynValue TryAddItem(ItemType item)
        {
            return DynValue.False;
        }
        public override void Harvest() { }
        public override DynValue CanHarvest() => DynValue.True;
        public void Init(int x, int y) => transform.position = new Vector3(x, 0, y);

        public override void FreeThis() => GameObjectPool<FacilityEmpty>.Free(this);
        public override void OnAlloc() { }
    }
}
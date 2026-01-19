using System;
using MoonSharp.Interpreter;
using UnityEngine;
using Items;

namespace Workspace.Facilities.Impl
{
    public class FacilityEmpty : Facility
    {
        public override FacilityType Type { get; } = FacilityType.Empty;
        public override double Progress { get; } = 1;
        public override int X => Mathf.RoundToInt(transform.position.x);
        public override int Y => Mathf.RoundToInt(transform.position.z);

        public override bool CanBuildOn(FacilityType type) => true;

        public override DynValue GetUniqueState() => DynValue.Nil;
        public override DynValue InteractWith(DynValue argTuple) => DynValue.Nil;
        public override DynValue TryAddItem(ItemType item, int count)
        {
            // TODO:: maybe some item have effect
            throw new System.NotImplementedException();
        }
        public override DynValue Harvest() => DynValue.False;
        public override DynValue CanHarvest() => DynValue.False;
        public void Init(int x, int y) => transform.position = new Vector3(x, 0, y);
    }
}
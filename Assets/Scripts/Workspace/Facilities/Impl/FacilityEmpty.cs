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

        public override DynValue InteractWith(CallbackArguments args) => DynValue.Nil;
        public override DynValue TryAddItem(ItemType item)
        {
            // TODO:: maybe some item have effect
            throw new System.NotImplementedException();
        }
        public override void Harvest() { }
        public override DynValue CanHarvest() => DynValue.True;
        public void Init(int x, int y) => transform.position = new Vector3(x, 0, y);
    }
}
using System;
using MoonSharp.Interpreter;
using UnityEngine;
using Workspace.Items;

namespace Workspace.Facilities.Impl
{
    public class FacilityEmpty : Facility
    {
        // TODO:: add prefab here
        // private GameObject obj = GameObject.Instantiate(??getPrefab, new Vector3Int(X, 0, Y), Quaternion.identity);
        
        public override FacilityType Type { get; } = FacilityType.Empty;
        public override double Progress { get; } = 1;
        public override int X { get; }
        public override int Y { get; }

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

        public FacilityEmpty(int x, int y) { X = x; Y = y; }
        // TODO:: enable after add prefab
        // ~FacilityEmpty() { GameObject.Destroy(obj); }
    }
}
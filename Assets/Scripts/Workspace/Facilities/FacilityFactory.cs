using UnityEngine;
using Utils;
using Workspace.Facilities.Impl;

namespace Workspace.Facilities
{
    public static class FacilityFactory
    {
        public static Facility CreateEmpty(int x, int y)
        {
            var obj = Object.Instantiate(
                ResourceManager<GameObject>.GetResource("FacilityEmpty"), 
                WorkspaceManager.Instance.transform);
            var facility = obj.GetComponent<FacilityEmpty>();
            facility.Init(x, y);
            return facility;
        }
        public static Facility CreateMana(int x, int y)
        {
            var obj = Object.Instantiate(
                ResourceManager<GameObject>.GetResource("FacilityMana"), 
                WorkspaceManager.Instance.transform);
            var facility = obj.GetComponent<FacilityMana>();
            facility.Init(x, y);
            return facility;
        }
        
        public static bool CanBuildOn(FacilityType typeNew, FacilityType typeOld) => typeNew switch
        {
            FacilityType.Empty => true,
            FacilityType.Mana => true,
            _ => throw new System.NotImplementedException(),
        };
        public static Facility GetInstanceByType(FacilityType type, int x, int y) => type switch
        {
            FacilityType.Empty => FacilityFactory.CreateEmpty(x, y),
            FacilityType.Mana  => FacilityFactory.CreateMana(x, y),
            _ => throw new System.NotImplementedException(),
        };
    }
}
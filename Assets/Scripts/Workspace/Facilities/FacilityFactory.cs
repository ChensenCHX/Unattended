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
        public static Facility CreateEther(int x, int y)
        {
            var obj = Object.Instantiate(
                ResourceManager<GameObject>.GetResource("FacilityEther"), 
                WorkspaceManager.Instance.transform);
            var facility = obj.GetComponent<FacilityEther>();
            facility.Init(x, y);
            return facility;
        }
        public static Facility CreateMelodia(int x, int y)
        {
            var obj = Object.Instantiate(
                ResourceManager<GameObject>.GetResource("FacilityMelodia"), 
                WorkspaceManager.Instance.transform);
            var facility = obj.GetComponent<FacilityMelodia>();
            facility.Init(x, y);
            return facility;
        }
        public static Facility CreateChronos(int x, int y)
        {
            var obj = Object.Instantiate(
                ResourceManager<GameObject>.GetResource("FacilityChronos"), 
                WorkspaceManager.Instance.transform);
            var facility = obj.GetComponent<FacilityChronos>();
            facility.Init(x, y);
            return facility;
        }
        public static Facility CreateSignum(int x, int y)
        {
            var obj = Object.Instantiate(
                ResourceManager<GameObject>.GetResource("FacilitySignum"), 
                WorkspaceManager.Instance.transform);
            var facility = obj.GetComponent<FacilitySignum>();
            facility.Init(x, y);
            return facility;
        }
        public static Facility CreateIter(int x, int y)
        {
            var obj = Object.Instantiate(
                ResourceManager<GameObject>.GetResource("FacilityIter"), 
                WorkspaceManager.Instance.transform);
            var facility = obj.GetComponent<FacilityIter>();
            facility.Init(x, y);
            return facility;
        }
        public static Facility CreateOpus(int x, int y)
        {
            var obj = Object.Instantiate(
                ResourceManager<GameObject>.GetResource("FacilityOpus"), 
                WorkspaceManager.Instance.transform);
            var facility = obj.GetComponent<FacilityOpus>();
            facility.Init(x, y);
            return facility;
        }
        
        public static bool CanBuildOn(FacilityType typeNew, FacilityType typeOld) => typeNew switch
        {
            FacilityType.Empty => true,
            FacilityType.Mana => true,
            FacilityType.Ether => (typeOld & FacilityType.EtherCanBuild) != 0,
            FacilityType.Melodia => (typeOld & FacilityType.MelodiaCanBuild) != 0,
            FacilityType.Chronos => (typeOld & FacilityType.ChronosCanBuild) != 0,
            FacilityType.Signum => (typeOld & FacilityType.SignumCanBuild) != 0,
            FacilityType.Iter => (typeOld & FacilityType.IterCanBuild) != 0,
            FacilityType.Opus => (typeOld & FacilityType.OpusCanBuild) != 0,
            _ => throw new System.NotImplementedException(),
        };
        public static Facility GetInstanceByType(FacilityType type, int x, int y) => type switch
        {
            FacilityType.Empty      => FacilityFactory.CreateEmpty(x, y),
            FacilityType.Mana       => FacilityFactory.CreateMana(x, y),
            FacilityType.Ether      => FacilityFactory.CreateEther(x, y),
            FacilityType.Melodia    => FacilityFactory.CreateMelodia(x, y),
            FacilityType.Chronos    => FacilityFactory.CreateChronos(x, y),
            FacilityType.Signum     => FacilityFactory.CreateSignum(x, y),
            FacilityType.Iter       => FacilityFactory.CreateIter(x, y),
            FacilityType.Opus       => FacilityFactory.CreateOpus(x, y),
            _ => throw new System.NotImplementedException(),
        };
    }
}
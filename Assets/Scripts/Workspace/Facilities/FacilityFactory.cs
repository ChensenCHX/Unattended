using UnityEngine;
using Utils;
using Workspace.Facilities.Impl;

namespace Workspace.Facilities
{
    public static class FacilityFactory
    {
        public static Facility CreateEmpty(int x, int y)
        {
            var facility = GameObjectPool<FacilityEmpty>.Alloc(ResourceManager<GameObject>.GetResource("FacilityEmpty"));
            facility.Init(x, y);
            return facility;
        }
        public static Facility CreateMana(int x, int y)
        {
            var facility = GameObjectPool<FacilityMana>.Alloc(ResourceManager<GameObject>.GetResource("FacilityMana"));
            facility.Init(x, y);
            return facility;
        }
        public static Facility CreateEther(int x, int y)
        {
            var facility = GameObjectPool<FacilityEther>.Alloc(ResourceManager<GameObject>.GetResource("FacilityEther"));
            facility.Init(x, y);
            return facility;
        }
        public static Facility CreateMelodia(int x, int y)
        {
            var facility = GameObjectPool<FacilityMelodia>.Alloc(ResourceManager<GameObject>.GetResource("FacilityMelodia"));
            facility.Init(x, y);
            return facility;
        }
        public static Facility CreateChronos(int x, int y)
        {
            var facility = GameObjectPool<FacilityChronos>.Alloc(ResourceManager<GameObject>.GetResource("FacilityChronos"));
            facility.Init(x, y);
            return facility;
        }
        public static Facility CreateSignum(int x, int y)
        {
            var facility = GameObjectPool<FacilitySignum>.Alloc(ResourceManager<GameObject>.GetResource("FacilitySignum"));
            facility.Init(x, y);
            return facility;
        }
        public static Facility CreateIter(int x, int y)
        {
            var facility = GameObjectPool<FacilityIter>.Alloc(ResourceManager<GameObject>.GetResource("FacilityIter"));
            facility.Init(x, y);
            return facility;
        }
        public static Facility CreateOpus(int x, int y)
        {
            if (FacilityOpus.OpusOnWorkplace)
            {
                var facilityEmpty = GameObjectPool<FacilityEmpty>.Alloc(ResourceManager<GameObject>.GetResource("FacilityEmpty"));
                facilityEmpty.Init(x, y);
                return facilityEmpty;
            }
            
            var facilityOpus = GameObjectPool<FacilityOpus>.Alloc(ResourceManager<GameObject>.GetResource("FacilityOpus"));
            facilityOpus.Init(x, y);
            return facilityOpus;
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
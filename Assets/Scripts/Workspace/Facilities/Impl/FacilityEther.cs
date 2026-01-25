using System.Collections;
using GlobalSettings;
using Items;
using MoonSharp.Interpreter;
using UnityEngine;

namespace Workspace.Facilities.Impl
{
    public class FacilityEther : Facility
    {
        public override FacilityType Type { get; } = FacilityType.Ether;
        public override double Progress => progress;
        public override int X => Mathf.RoundToInt(transform.position.x);
        public override int Y => Mathf.RoundToInt(transform.position.z);
        
        private double progress = 0.0f;
        
        public override DynValue GetUniqueState() => DynValue.Nil;
        public override DynValue InteractWith(DynValue argTuple) => DynValue.Nil;
        public override DynValue TryAddItem(ItemType item, int count)
        {
            // TODO:: maybe some item have effect
            throw new System.NotImplementedException();
        }

        private bool _CanHarvest() => progress >= 1.0f;
        public override DynValue CanHarvest() => _CanHarvest() ? DynValue.True : DynValue.False;
        private int GetNearFacilityCount()
        {
            var sameCount = 0;
            if (WorkspaceManager.Instance.GetFacility(X-1 + GlobalInfos.Instance.WorkspaceEdgeLength, Y).Type == FacilityType.Ether) sameCount++;
            if (WorkspaceManager.Instance.GetFacility(X+1, Y).Type == FacilityType.Ether) sameCount++;
            if (WorkspaceManager.Instance.GetFacility(X, Y-1 + GlobalInfos.Instance.WorkspaceEdgeLength).Type == FacilityType.Ether) sameCount++;
            if (WorkspaceManager.Instance.GetFacility(X, Y+1).Type == FacilityType.Ether) sameCount++;
            
            return sameCount;
        }
        public override void Harvest()
        {
            if (_CanHarvest()) GlobalInfos.Instance.EtherCount += GlobalInfos.Instance.EtherBaseYield * (0b00010000 >> GetNearFacilityCount());
            WorkspaceManager.Instance.TrySetFacility(X, Y, FacilityType.Mana);
        }
        public void Init(int x, int y)
        {
            transform.position = new Vector3(x, 0, y);
            var time = Random.Range(GlobalConsts.EtherGrowTimeLowerBound, GlobalConsts.EtherGrowTimeUpperBound);

            StartCoroutine(InitCoroutine(time));
        }
        private IEnumerator InitCoroutine(float basicGrowthTime)
        {
            var objTransform = transform.Find("Main").transform;

            while (progress < 1.0f)
            {
                yield return null;
                var timeMul = 1 << GetNearFacilityCount();
                
                progress += Time.deltaTime / (timeMul * basicGrowthTime);
                objTransform.localScale = Vector3.one * (float)progress;
            }
            progress = 1.0f;
            objTransform.localScale = Vector3.one;
        }
    }
}
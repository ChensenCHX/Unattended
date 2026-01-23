using DG.Tweening;
using GlobalSettings;
using Items;
using MoonSharp.Interpreter;
using UnityEngine;

namespace Workspace.Facilities.Impl
{
    public class FacilityMana : Facility
    {
        public override FacilityType Type { get; } = FacilityType.Mana;
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
        public override void Harvest()
        {
            if (_CanHarvest()) GlobalInfos.Instance.ManaCount += GlobalInfos.Instance.ManaBaseYield;
            WorkspaceManager.Instance.TrySetFacility(X, Y, FacilityType.Empty);
        }
        public void Init(int x, int y)
        {
            transform.position = new Vector3(x, 0, y);
            var time = Random.Range(GlobalConsts.ManaGrowTimeLowerBound, GlobalConsts.ManaGrowTimeUpperBound);
            var objTransform = transform.Find("Main").transform;
            objTransform.DOScale(Vector3.one, time)
                .SetEase(Ease.Linear)
                .OnUpdate(() => progress = objTransform.localScale.x)
                .OnComplete(() => progress = 1.0f);
        }
    }
}
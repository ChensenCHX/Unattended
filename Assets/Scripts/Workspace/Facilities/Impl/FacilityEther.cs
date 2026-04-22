using System.Collections;
using DG.Tweening;
using GlobalSettings;
using Items;
using MoonSharp.Interpreter;
using UnityEngine;
using Utils;

namespace Workspace.Facilities.Impl
{
    public class FacilityEther : Facility, IPoolable<FacilityEther>
    {
        public override FacilityType Type { get; } = FacilityType.Ether;
        public override Tween GrowthTween => growthTween;
        public override double Progress => progress;
        public override int X => Mathf.RoundToInt(transform.position.x);
        public override int Y => Mathf.RoundToInt(transform.position.z);
        
        private Tween growthTween;
        private double progress = 0.0f;
        private Transform objTransform;

        public override DynValue InteractWith(CallbackArguments args) => DynValue.Nil;
        public override DynValue TryAddItem(ItemType item)
        {
            return DefaultTryAddItem(item, objTransform);
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

            growthTween = objTransform.DOScale(Vector3.one, time)
                .SetEase(Ease.Linear)
                .OnUpdate(() =>
                {
                    // 每帧根据设施数量计算当前速度因子 timeMul
                    int timeMul = 1 << GetNearFacilityCount();
                    // 调整播放速度：timeMul 越大，timeScale 越小，动画越慢
                    growthTween.timeScale = 1.0f / timeMul;
                    progress = objTransform.localScale.x;
                })
                .OnComplete(() =>
                {
                    // 确保最终缩放精确为 1
                    objTransform.localScale = Vector3.one;
                    progress = 1.0f;
                });
        }

        
        private void OnDestroy() => objTransform.DOKill();
        
        public override void FreeThis() => GameObjectPool<FacilityEther>.Free(this);
        public override void OnAlloc()
        {
            progress = 0.0f;
            objTransform ??= transform.Find("Main").transform;
            objTransform.DOKill();
            objTransform.localScale = Vector3.zero;
        }
    }
}
using DG.Tweening;
using GlobalSettings;
using MoonSharp.Interpreter;
using Items;
using UnityEngine;
using Utils;

namespace Workspace.Facilities
{
    public abstract class Facility : MonoBehaviour, IPoolable<Facility>
    {
        # region 属性
        public abstract FacilityType Type { get; }
        public abstract Tween GrowthTween { get; }
        public abstract double Progress { get; }
        public abstract int X { get; }
        public abstract int Y { get; }
        # endregion
        
        # region 玩家方法
        public abstract DynValue InteractWith(CallbackArguments args);
        public abstract DynValue TryAddItem(ItemType item);
        public abstract DynValue CanHarvest();
        public abstract void Harvest();
        # endregion

        protected DynValue DefaultTryAddItem(ItemType item, Transform objTransform)
        {
            if (item is ItemType.Chronos && GlobalInfos.Instance.TryConsumeItem(item, 1))
            {
                if (GrowthTween is not null && (Transform)GrowthTween.target == objTransform && GrowthTween.IsPlaying())
                {
                    GrowthTween.Goto(GrowthTween.Duration() + 1f);
                    return DynValue.True;
                }
            }

            if (item is ItemType.Opus && GlobalInfos.Instance.TryConsumeItem(item, 1))
            {
                if (GrowthTween is not null && (Transform)GrowthTween.target == objTransform && GrowthTween.IsPlaying())
                {
                    GrowthTween.Complete();
                    return DynValue.True;
                }
            }

            return DynValue.False;
        }
        private void OnDestroy() => transform.DOKill();

        public virtual void FreeThis() { }
        public virtual void OnAlloc() { }
    }
}

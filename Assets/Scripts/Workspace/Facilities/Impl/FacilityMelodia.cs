using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GlobalSettings;
using Items;
using MoonSharp.Interpreter;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Workspace.Facilities.Impl
{
    public class FacilityMelodia : Facility, IPoolable<FacilityMelodia>
    {
        public override FacilityType Type { get; } = FacilityType.Melodia;
        public override Tween GrowthTween => growthTween;
        public override double Progress => progress;
        public override int X => Mathf.RoundToInt(transform.position.x);
        public override int Y => Mathf.RoundToInt(transform.position.z);
        
        private Tween growthTween;
        private double progress = 0.0f;
        private static Queue<int> toneQueue = new();
        private Transform objTransform;
        private int tone;

        public override DynValue InteractWith(CallbackArguments args)
        {
            var funcName = args.AsType(0, "InteractWith", DataType.String);
            return funcName.String != "get_tone" ? DynValue.Nil : DynValueCache.NewNumber(tone);
        }
        public override DynValue TryAddItem(ItemType item)
        {
            return DefaultTryAddItem(item, objTransform);
        }
        private bool _CanHarvest() => progress >= 1.0f;
        public override DynValue CanHarvest() => _CanHarvest() ? DynValue.True : DynValue.False;
        public override void Harvest()
        {
            if (_CanHarvest())
            {
                while (toneQueue.Count >= 32) toneQueue.Dequeue();
                
                if (toneQueue.Any(toneBefore => tone == toneBefore)) toneQueue.Clear();
                toneQueue.Enqueue(tone);
                GlobalInfos.Instance.MelodiaCount += GlobalInfos.Instance.MelodiaBaseYield * Math.Min(16, toneQueue.Count);
            }
            WorkspaceManager.Instance.TrySetFacility(X, Y, FacilityType.Mana);
        }
        public void Init(int x, int y)
        {
            transform.position = new Vector3(x, 0, y);
            tone = Random.Range(0, 32);
            var time = Random.Range(GlobalConsts.MelodiaGrowTimeLowerBound, GlobalConsts.MelodiaGrowTimeUpperBound);
            growthTween = objTransform.DOScale(Vector3.one, time)
                .SetEase(Ease.Linear)
                .OnUpdate(() => progress = objTransform.localScale.x)
                .OnComplete(() => progress = 1.0f);
        }

        private void OnDestroy() => objTransform.DOKill();

        public override void FreeThis() => GameObjectPool<FacilityMelodia>.Free(this);
        public override void OnAlloc()
        {
            progress = 0.0f;
            objTransform ??= transform.Find("Main").transform;
            objTransform.DOKill();
            objTransform.localScale = Vector3.zero;
        }
    }
}
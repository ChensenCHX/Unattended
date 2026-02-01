using System;
using DG.Tweening;
using GlobalSettings;
using Items;
using MoonSharp.Interpreter;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Workspace.Facilities.Impl
{
    public class FacilityChronos : Facility
    {
        public override FacilityType Type { get; } = FacilityType.Chronos;
        public override double Progress => progress;
        public override int X => Mathf.RoundToInt(transform.position.x);
        public override int Y => Mathf.RoundToInt(transform.position.z);
        
        private double progress = 0.0f;
        private Transform objTransform;
        private ItemType requestItemType;
        private bool startedBefore = false;
        private bool finished = false;
        private bool success = false;
        private int targetTime;
        private int tolerance;

        
        public override DynValue InteractWith(CallbackArguments args)
        {
            var funcName = args.AsType(0, "InteractWith", DataType.String);
            switch (funcName.String)
            {
                case "check":
                    if (!startedBefore) return DynValueCache.NewString("init");
                    if (Math.Abs(Time.frameCount - targetTime) > tolerance) finished = true; 
                    return finished ? DynValueCache.NewString(success ? "success" : "fail") : DynValueCache.NewString("waiting");
                case "start":
                    var arg = args.AsInt(1, "Chronos.InteractWith.start");
                    tolerance = Math.Abs(arg);
                    
                    if (!startedBefore) startedBefore = true; else return DynValue.Nil;
                    targetTime = Time.frameCount + Random.Range(GlobalConsts.ChronosSyncFrameLowerBound, GlobalConsts.ChronosSyncFrameUpperBound);
                    return DynValue.NewTuple(DynValueCache.NewNumber((int)requestItemType), DynValueCache.NewNumber(targetTime));
                default:
                    return DynValue.Nil;
            }
        }
        public override DynValue TryAddItem(ItemType item)
        {
            if (item == requestItemType)
            {
                if (!GlobalInfos.Instance.TryConsumeItem(item, GlobalInfos.Instance.ChronosBaseYield)) return DynValue.False;
                if (!startedBefore || finished) return DynValue.True;

                finished = true;
                if (Math.Abs(Time.frameCount - targetTime) <= tolerance) success = true;
                return DynValue.True;
            }
            
            // TODO:: maybe some usage
            throw new System.NotImplementedException();
        }
        private bool _CanHarvest() => progress >= 1.0f;
        public override DynValue CanHarvest() => _CanHarvest() ? DynValue.True : DynValue.False;
        public override void Harvest()
        {
            if (_CanHarvest())
            {
                var before = GlobalInfos.Instance.ChronosCount;
                if (success) 
                    GlobalInfos.Instance.ChronosCount += Math.Floor(GlobalInfos.Instance.ChronosBaseYield * (16.0 / Math.Min(16.0, Math.Sqrt(tolerance + 1))));
                else
                    GlobalInfos.Instance.ChronosCount += GlobalInfos.Instance.ChronosBaseYield;
            }
            WorkspaceManager.Instance.TrySetFacility(X, Y, FacilityType.Ether);
        }
        public void Init(int x, int y)
        {
            transform.position = new Vector3(x, 0, y);
            requestItemType = (ItemType)Random.Range((int)ItemType.Mana, (int)ItemType.Signum);
            var time = Random.Range(GlobalConsts.ChronosGrowTimeLowerBound, GlobalConsts.ChronosGrowTimeUpperBound);
            objTransform = transform.Find("Main").transform;
            objTransform.DOScale(Vector3.one, time)
                .SetEase(Ease.Linear)
                .OnUpdate(() => progress = objTransform.localScale.x)
                .OnComplete(() => progress = 1.0f);
        }
        
        private void OnDestroy() => objTransform.DOKill();
    }
}
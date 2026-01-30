using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GlobalSettings;
using Items;
using MoonSharp.Interpreter;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Workspace.Facilities.Impl
{
    public class FacilitySignum : Facility
    {
        public override FacilityType Type { get; } = FacilityType.Signum;
        public override double Progress => progress;
        public override int X => Mathf.RoundToInt(transform.position.x);
        public override int Y => Mathf.RoundToInt(transform.position.z);
        
        private double progress = 0.0f;
        private Transform objTransform;
        private int height;
        private int strength;
        private bool detached = false;
        
        public override DynValue InteractWith(CallbackArguments args)
        {
            var funcName = args.AsType(0, "InteractWith", DataType.String);
            switch(funcName.String)
            {
                case "get_height":
                    return DynValue.NewNumber(height);
                case "get_strength":
                    return DynValue.NewNumber(strength);
                case "detach":
                    detached = true;
                    return DynValue.True;
                default:
                    return DynValue.Nil;
            };
        }
        public override DynValue TryAddItem(ItemType item)
        {
            // TODO:: maybe some usage
            throw new System.NotImplementedException();
        }
        private bool _CanHarvest() => progress >= 1.0f;
        public override DynValue CanHarvest() => _CanHarvest() ? DynValue.True : DynValue.False;
        private bool RawHarvest()
        {
            WorkspaceManager.Instance.TrySetFacility(X, Y, FacilityType.Ether); 
            return _CanHarvest();
        }
        public override void Harvest()
        {
            if (detached)
                if (_CanHarvest()) GlobalInfos.Instance.SignumCount += GlobalInfos.Instance.SignumBaseYield;
            else
            {
                var totalStrength = strength;
                var totalCount = _CanHarvest() ? 1 : 0;
                var canTransferFrom = GetSignumsCanBeLinked();
                canTransferFrom.ForEach(signum =>
                {
                    if (!signum.RawHarvest()) return;
                    totalCount++; totalStrength += signum.strength;
                });
                GlobalInfos.Instance.SignumCount += GlobalInfos.Instance.SignumBaseYield * totalCount * totalCount * totalStrength;
            }
            RawHarvest();
        }
        public void Init(int x, int y)
        {
            transform.position = new Vector3(x, 0, y);
            height = Random.Range(GlobalConsts.SignumHeightLowerBound, GlobalConsts.SignumHeightUpperBound);
            strength = Random.Range(GlobalConsts.SignumStrengthLowerBound, GlobalConsts.SignumStrengthUpperBound);
            var time = Random.Range(GlobalConsts.SignumGrowTimeLowerBound, GlobalConsts.SignumGrowTimeUpperBound);
            objTransform = transform.Find("Main").transform;
            objTransform.DOScale(Vector3.one, time)
                .SetEase(Ease.Linear)
                .OnUpdate(() => progress = objTransform.localScale.x)
                .OnComplete(() => progress = 1.0f);
        }
        
        private void OnDestroy() => objTransform.DOKill();

        private List<FacilitySignum> GetSignumsCanBeLinked()
        {
            var edgeLength = GlobalInfos.Instance.WorkspaceEdgeLength;
            var canTransferFrom = new List<FacilitySignum>();
            var currentHighest = 0;
            for (var objX = X - 1; objX >= 0; objX--)
            {
                var facility = WorkspaceManager.Instance.GetFacility(objX, Y);
                if (facility.Type != FacilityType.Signum) continue;
                var signum = (FacilitySignum)facility;
                if (currentHighest >= signum.height) continue; currentHighest = signum.height;
                if (currentHighest > height) continue; canTransferFrom.Add(signum);
            } currentHighest = 0;
            for (var objX = X + 1; objX < edgeLength; objX++)
            {
                var facility = WorkspaceManager.Instance.GetFacility(objX, Y);
                if (facility.Type != FacilityType.Signum) continue;
                var signum = (FacilitySignum)facility;
                if (currentHighest >= signum.height) continue; currentHighest = signum.height;
                if (currentHighest > height) continue; canTransferFrom.Add(signum);
            } currentHighest = 0;
            for (var objY = Y - 1; objY >= 0; objY--)
            {
                var facility = WorkspaceManager.Instance.GetFacility(X, objY);
                if (facility.Type != FacilityType.Signum) continue;
                var signum = (FacilitySignum)facility;
                if (currentHighest >= signum.height) continue; currentHighest = signum.height;
                if (currentHighest > height) continue; canTransferFrom.Add(signum);
            } currentHighest = 0;
            for (var objY = Y + 1; objY < edgeLength; objY++)
            {
                var facility = WorkspaceManager.Instance.GetFacility(X, objY);
                if (facility.Type != FacilityType.Signum) continue;
                var signum = (FacilitySignum)facility;
                if (currentHighest >= signum.height) continue; currentHighest = signum.height;
                if (currentHighest > height) continue; canTransferFrom.Add(signum);
            } currentHighest = 0;
            return canTransferFrom;
        }
    }
}
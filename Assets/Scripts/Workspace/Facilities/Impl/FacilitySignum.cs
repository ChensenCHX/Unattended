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
        private HashSet<FacilitySignum> canTransferFrom = new();
        
        public override DynValue InteractWith(CallbackArguments args)
        {
            var funcName = args.AsType(0, "InteractWith", DataType.String);
            return funcName.String switch
            {
                "get_height" => DynValue.NewNumber(height),
                "get_strength" => DynValue.NewNumber(strength),
                _ => DynValue.Nil
            };
        }
        public override DynValue TryAddItem(ItemType item)
        {
            // TODO:: maybe some usage
            throw new System.NotImplementedException();
        }
        private bool _CanHarvest() => progress >= 1.0f;
        public override DynValue CanHarvest() => _CanHarvest() ? DynValue.True : DynValue.False;
        private bool RawHarvest() { WorkspaceManager.Instance.TrySetFacility(X, Y, FacilityType.Ether); return _CanHarvest(); }
        public override void Harvest()
        {
            if (_CanHarvest())
            {
                var totalStrength = 0;
                var totalCount = 0;
                canTransferFrom
                    .ToList()
                    .ForEach(facility => {
                        if (!facility.RawHarvest()) return;
                        totalCount++; totalStrength += facility.strength;
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

            var edgeLength = GlobalInfos.Instance.WorkspaceEdgeLength;

            var luLowerList = new List<FacilitySignum>(32);
            var luHigherList = new List<FacilitySignum>(32);
            var rdLowerList = new List<FacilitySignum>(32);
            var rdHigherList = new List<FacilitySignum>(32);

            for (var objX = x - 1; objX >= 0; objX--)
            {
                var facility = WorkspaceManager.Instance.GetFacility(objX, y);
                if (facility.Type != FacilityType.Signum) continue;
                var signum = (FacilitySignum)facility;
                if (signum.height <= height) luLowerList.Add(signum); else luHigherList.Add(signum);
            }
            for (var objX = x + 1; objX < edgeLength; objX++)
            {
                var facility = WorkspaceManager.Instance.GetFacility(objX, y);
                if (facility.Type != FacilityType.Signum) continue;
                var signum = (FacilitySignum)facility;
                if (signum.height <= height) rdLowerList.Add(signum); else rdHigherList.Add(signum);
            }
            luLowerList.ForEach(signum => signum.canTransferFrom.ExceptWith(rdLowerList));
            luHigherList.ForEach(signum => signum.canTransferFrom.ExceptWith(rdLowerList));
            rdLowerList.ForEach(signum => signum.canTransferFrom.ExceptWith(luLowerList));
            rdHigherList.ForEach(signum => signum.canTransferFrom.ExceptWith(luLowerList));
            
            luLowerList.Clear(); luHigherList.Clear(); rdLowerList.Clear(); rdHigherList.Clear();
            for (var objY = y + 1; objY < edgeLength; objY++)
            {
                var facility = WorkspaceManager.Instance.GetFacility(x, objY);
                if (facility.Type != FacilityType.Signum) continue;
                var signum = (FacilitySignum)facility;
                if (signum.height <= height) luLowerList.Add(signum); else luHigherList.Add(signum);
            }
            for (var objY = y - 1; objY >= 0; objY--)
            {
                var facility = WorkspaceManager.Instance.GetFacility(x, objY);
                if (facility.Type != FacilityType.Signum) continue;
                var signum = (FacilitySignum)facility;
                if (signum.height <= height) rdLowerList.Add(signum); else rdHigherList.Add(signum);
            }
            luLowerList.ForEach(signum => signum.canTransferFrom.ExceptWith(rdLowerList));
            luHigherList.ForEach(signum => signum.canTransferFrom.ExceptWith(rdLowerList));
            rdLowerList.ForEach(signum => signum.canTransferFrom.ExceptWith(luLowerList));
            rdHigherList.ForEach(signum => signum.canTransferFrom.ExceptWith(luLowerList));
            
            var time = Random.Range(GlobalConsts.SignumGrowTimeLowerBound, GlobalConsts.SignumGrowTimeUpperBound);
            objTransform = transform.Find("Main").transform;
            objTransform.DOScale(Vector3.one, time)
                .SetEase(Ease.Linear)
                .OnUpdate(() => progress = objTransform.localScale.x)
                .OnComplete(() => progress = 1.0f);
        }
        
        private void OnDestroy()
        {
            objTransform.DOKill();
            var edgeLength = GlobalInfos.Instance.WorkspaceEdgeLength;
            for (var x = 0; x < edgeLength; x++)
            {
                var facility = WorkspaceManager.Instance.GetFacility(x, Y);
                if (facility.Type != FacilityType.Signum) continue;
                var signum = (FacilitySignum)facility;
                signum.canTransferFrom.Remove(this);
            }
            for(var y = 0; y < edgeLength; y++)
            {
                var facility = WorkspaceManager.Instance.GetFacility(X, y);
                if (facility.Type != FacilityType.Signum) continue;
                var signum = (FacilitySignum)facility;
                signum.canTransferFrom.Remove(this);
            }
        }
    }
}
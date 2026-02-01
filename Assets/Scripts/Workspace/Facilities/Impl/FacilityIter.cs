using System;
using System.Collections.Generic;
using System.Linq;
using CodeExecutor;
using DG.Tweening;
using GlobalSettings;
using Items;
using MoonSharp.Interpreter;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Workspace.Facilities.Impl
{
    public class FacilityIter : Facility
    {
        public override FacilityType Type { get; } = FacilityType.Iter;
        public override double Progress => progress;
        public override int X => Mathf.RoundToInt(transform.position.x);
        public override int Y => Mathf.RoundToInt(transform.position.z);
        
        private double progress = 0.0f;
        private Transform objTransform;
        private Dictionary<FacilityIter, bool> edgeList = new();
        private Dictionary<FacilityIter, int> edgeWeight = new();
        
        public override DynValue InteractWith(CallbackArguments args)
        {
            var funcName = args.AsType(0, "InteractWith", DataType.String);
            switch(funcName.String)
            {
                case "get_edges":
                {
                    var tableObj = DynValue.NewPrimeTable();
                    var table = tableObj.Table;
                    
                    var xTableObj = DynValue.NewPrimeTable(); var xTable = xTableObj.Table;
                    var yTableObj = DynValue.NewPrimeTable(); var yTable = yTableObj.Table;
                    var weightTableObj = DynValue.NewPrimeTable(); var weightTable = weightTableObj.Table;
                    var stateTableObj = DynValue.NewPrimeTable(); var stateTable = stateTableObj.Table;

                    table.Set("x", xTableObj);
                    table.Set("y", yTableObj);
                    table.Set("weight", weightTableObj);
                    table.Set("state", stateTableObj);

                    foreach (var (target, state) in edgeList)
                    {
                        xTable.Append(DynValue.NewNumber(target.X));
                        yTable.Append(DynValue.NewNumber(target.Y));
                        weightTable.Append(DynValue.NewNumber(edgeWeight[target]));
                        stateTable.Append(DynValue.NewString(state ? "connected" : "disconnected"));
                    }

                    return tableObj;
                }
                case "connect":
                {
                    var x = args.AsInt(1, "Iter.InteractWith.connect");
                    var y = args.AsInt(2, "Iter.InteractWith.connect");
                    var facility = WorkspaceManager.Instance.GetFacility(x, y);
                    if (facility.Type != FacilityType.Iter) return DynValue.False;

                    var iter = (FacilityIter)facility;
                    if (!(edgeList.ContainsKey(iter) && iter.edgeList.ContainsKey(this))) return DynValue.False;
                    edgeList[iter] = true;
                    iter.edgeList[this] = true;
                    return DynValue.True;
                }
                case "disconnect":
                {
                    var x = args.AsInt(1, "Iter.InteractWith.connect");
                    var y = args.AsInt(2, "Iter.InteractWith.connect");
                    var facility = WorkspaceManager.Instance.GetFacility(x, y);
                    if (facility.Type != FacilityType.Iter) return DynValue.False;

                    var iter = (FacilityIter)facility;
                    if (!(edgeList.ContainsKey(iter) && iter.edgeList.ContainsKey(this))) return DynValue.False;
                    edgeList[iter] = false;
                    iter.edgeList[this] = false;
                    return DynValue.True;
                }
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
            foreach (var (other, _) in edgeList) { other.edgeList.Remove(this); }
            WorkspaceManager.Instance.TrySetFacility(X, Y, FacilityType.Signum); 
            return _CanHarvest();
        }
        public override void Harvest()
        {
            var iters = FindAllLinkedItersFromThis();
            var edgeWeightSum = iters.Sum(iter => iter.edgeList.Sum(kvPair => kvPair.Value ? kvPair.Key.edgeWeight[iter] : 0));
            var validNodeCount = iters.Sum(iter => iter.RawHarvest() ? 1 : 0);


            edgeWeightSum >>= 1;    //这里每条边实际被计算了两次 (a->b + b->a) 所以总权重要除以2
            GlobalInfos.Instance.IterCount += 1.0 * GlobalInfos.Instance.IterBaseYield * 
                validNodeCount * validNodeCount * validNodeCount / Math.Max(1, edgeWeightSum);
        }
        public void Init(int x, int y)
        {
            transform.position = new Vector3(x, 0, y);
            var time = Random.Range(GlobalConsts.IterGrowTimeLowerBound, GlobalConsts.IterGrowTimeUpperBound);
            var others = GetOtherIters()
                .FindAll(iter => iter.edgeList.Count < GlobalConsts.IterEdgeCountUpperBound)
                .Shuffle()
                .Take(Random.Range(GlobalConsts.IterEdgeCountLowerBound, GlobalConsts.IterEdgeCountUpperBound));
            foreach (var other in others)
            {
                var weight = Random.Range(GlobalConsts.IterEdgeWeightLowerBound, GlobalConsts.IterEdgeWeightUpperBound);
                edgeList.Add(other, false); 
                edgeWeight.Add(other, weight);
                other.edgeList.Add(this, false);
                other.edgeWeight.Add(this, weight);
            }
            objTransform = transform.Find("Main").transform;
            objTransform.DOScale(Vector3.one, time)
                .SetEase(Ease.Linear)
                .OnUpdate(() => progress = objTransform.localScale.x)
                .OnComplete(() => progress = 1.0f);
        }
        
        private void OnDestroy() => objTransform.DOKill();
        private List<FacilityIter> GetOtherIters()
        {
            var edgeLength = GlobalInfos.Instance.WorkspaceEdgeLength;
            var iters = new List<FacilityIter>();
            for (var i = 0; i < edgeLength; i++)
            for (var j = 0; j < edgeLength; j++)
            {
                var facility = WorkspaceManager.Instance.GetFacility(i, j);
                if (facility.Type == FacilityType.Iter && facility != this) iters.Add((FacilityIter)facility);
            }
            return iters;
        }
        private HashSet<FacilityIter> FindAllLinkedItersFromThis(bool notIncludeSelf = false)
        {
            var visited = new HashSet<FacilityIter>(64);
            var queue = new Queue<FacilityIter>(64);
            queue.Enqueue(this);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (!visited.Add(current)) continue;
                foreach (var (target, linked) in current.edgeList)
                    if (linked) queue.Enqueue(target);
            }

            if (notIncludeSelf) visited.Remove(this);
            return visited;
        }

        // private void OnDrawGizmos()
        // {
        //     var from = new Vector3(X, 1, Y);
        //     Gizmos.color = Color.green;
        //     edgeList
        //         .Where(kvPair => kvPair.Value)
        //         .Select(iter => new Vector3(iter.Key.X, 1, iter.Key.Y))
        //         .ToList()
        //         .ForEach(to => Gizmos.DrawLine(from, to));
        //
        //     Gizmos.color = Color.red;
        //     edgeList
        //         .Where(kvPair => !kvPair.Value)
        //         .Select(iter => new Vector3(iter.Key.X, 1, iter.Key.Y))
        //         .ToList()
        //         .ForEach(to => Gizmos.DrawLine(from, to));
        // }
    }
}
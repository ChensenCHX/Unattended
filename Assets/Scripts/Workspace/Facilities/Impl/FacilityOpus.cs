using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DG.Tweening;
using GlobalSettings;
using Items;
using MoonSharp.Interpreter;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Workspace.Facilities.Impl
{
    public class FacilityOpus : Facility, IPoolable<FacilityOpus>
    {
        public override FacilityType Type { get; } = FacilityType.Opus;
        public override double Progress => progress;
        public override int X => Mathf.RoundToInt(transform.position.x);
        public override int Y => Mathf.RoundToInt(transform.position.z);
        
        private double progress = 0.0f;
        private Transform objTransform;
        private uint[,] lifegameMap;     // some cursed bit operate used here: bit0~29 as uint30, bit30 as last life state, bit31 as current life state
        private int lifegameMapSize;
        private bool started = false;
        private bool halted = false;
        private int removeCost = 0;
        public static bool OpusOnWorkplace { get; private set; } = false;
        private List<ValueTuple<int, int>> advPositions = new();
        
        private uint GetLifeTime(int x, int y) => lifegameMap[x, y] >> 2;
        private void SetLifeTime(int x, int y, uint value) => lifegameMap[x, y] = (value << 2) | (lifegameMap[x, y] & 0b11);
        private bool AliveNow(int x, int y) => (lifegameMap[x, y] & 0b01) != 0;
        private bool AliveBefore(int x, int y) => (lifegameMap[x, y] & 0b10) != 0;
        private void ShiftLifeState(int x, int y) => lifegameMap[x, y] &= ((lifegameMap[x, y] & 0b11) << 1) | 0xFFFFFFFC;
        private void UpdateLifeState(int x, int y, bool alive, int avgLifetime)
        {
            var aliveBefore = AliveBefore(x, y);
            if (alive)
            {
                if (aliveBefore) SetLifeTime(x, y, GetLifeTime(x, y) + 1); else SetLifeTime(x, y, (uint)avgLifetime);
                lifegameMap[x, y] |= 0b01;
            }
            else
                SetLifeTime(x, y, 0);
        } 
        private int ClampPos(int x)
        {
            if (x < 0) x += lifegameMapSize;
            if (x >= lifegameMapSize) x -= lifegameMapSize;
            return x;
        }
        private void UpdateLifeMap()
        {
            for (var i = 0; i < lifegameMapSize; i++)
            for (var j = 0; j < lifegameMapSize; j++)
                ShiftLifeState(i, j);
            
            for (var i = 0; i < lifegameMapSize; i++)
            for (var j = 0; j < lifegameMapSize; j++)
            {
                var aliveCount = 0;
                var totalLifetime = 0;
                for (var ii = i - 1; ii <= i + 1; ii++) { 
                    var iPos = ClampPos(ii);
                    for (var jj = j - 1; jj <= j + 1; jj++)
                    {
                        var jPos = ClampPos(jj);
                        if ((iPos == i && jPos == j) || !AliveNow(iPos, jPos)) continue;
                        aliveCount++;
                        totalLifetime += (int)GetLifeTime(iPos, jPos);
                    }
                }

                switch (aliveCount)
                {
                    case 2:
                    case 3:
                        UpdateLifeState(i, j, true, totalLifetime / aliveCount); break;
                    default:
                        UpdateLifeState(i, j, false, 0); break;
                }
            }
        }
        
        public override DynValue InteractWith(CallbackArguments args)
        {
            var funcName = args.AsType(0, "InteractWith", DataType.String);
            switch(funcName.String)
            {
                case "start":
                {
                    if (halted || started) return DynValue.False; started = true;
                    var infoTableObj = DynValue.NewPrimeTable(); var infoTable = infoTableObj.Table;
                    var xTableObj = DynValue.NewPrimeTable(); var xTable = xTableObj.Table;
                    var yTableObj = DynValue.NewPrimeTable(); var yTable = yTableObj.Table;

                    infoTable.Set("x", xTableObj);
                    infoTable.Set("y", yTableObj);

                    var xn1 = ClampPos(X - 1); var xn2 = X; var xn3 = ClampPos(X + 1);
                    var yn1 = ClampPos(Y - 1); var yn2 = Y; var yn3 = ClampPos(Y + 1);
                    var count = Random.Range(GlobalConsts.OpusGenerateCountLowerBound, GlobalConsts.OpusGenerateCountUpperBound);
                    count = Math.Min(count, lifegameMapSize * lifegameMapSize - 9);
                    while (count > 0)
                    {
                        var x = Random.Range(0, lifegameMapSize);
                        var y = Random.Range(0, lifegameMapSize);
                        if (x == xn1 || x == xn2 || x == xn3 || y == yn1 || y == yn2 || y == yn3) continue;
                        count--;
                        xTable.Append(DynValueCache.NewNumber(x));
                        yTable.Append(DynValueCache.NewNumber(y));
                        lifegameMap[x, y] = 0b01;
                    }
                    return infoTableObj;
                }
                case "eval":
                {
                    if (halted || !started) return DynValue.False;
                    var workspaceManager = WorkspaceManager.Instance;
                    var x = X;
                    var y = Y;
                    for (var i = 0; i < lifegameMapSize; i++)
                    for (var j = 0; j < lifegameMapSize; j++)
                    {
                        var isAlive = workspaceManager.GetFacility(i, j).Type != FacilityType.Empty;
                        if (isAlive == AliveNow(i, j) || (i == x && j == y)) continue;
                        halted = true;
                        return DynValue.False;
                    }

                    UpdateLifeMap();
                    
                    for (var i = x - 1; i < x + 1; i++) { 
                        var iPos = ClampPos(i);
                        for (var j = y - 1; j < y + 1; j++)
                        {
                            var jPos = ClampPos(j);
                            if (!AliveNow(iPos, jPos)) continue;
                            halted = true; advPositions.Add(new ValueTuple<int, int>(i, j));
                        }
                    }
                    return DynValue.True;
                }
                case "add":
                {
                    if (halted || !started || !GlobalInfos.Instance.TryConsumeItem(ItemType.Opus, 1))
                        return DynValue.False;

                    var x = args.AsInt(1, "Opus.InteractWith.add");
                    var y = args.AsInt(2, "Opus.InteractWith.add");
                    x = ((x % lifegameMapSize) + lifegameMapSize) % lifegameMapSize;
                    y = ((y % lifegameMapSize) + lifegameMapSize) % lifegameMapSize;
                    lifegameMap[x, y] = 0b01; // refresh lifetime & state
                    return DynValue.True;
                }
                case "remove":
                {
                    if (halted || !started || !GlobalInfos.Instance.TryConsumeItem(ItemType.Opus, removeCost))
                        return DynValue.False;
                    
                    var x = args.AsInt(1, "Opus.InteractWith.remove");
                    var y = args.AsInt(2, "Opus.InteractWith.remove");
                    x = ((x % lifegameMapSize) + lifegameMapSize) % lifegameMapSize;
                    y = ((y % lifegameMapSize) + lifegameMapSize) % lifegameMapSize;
                    lifegameMap[x, y] = 0b00; // refresh lifetime & state
                    removeCost++;
                    return DynValue.True;
                }
                default:
                    return DynValue.Nil;
            };
        }
        public override DynValue TryAddItem(ItemType item)
        {
            // TODO:: maybe some item have effect
            throw new System.NotImplementedException();
        }

        private bool _CanHarvest() => progress >= 1.0f;
        public override DynValue CanHarvest() => _CanHarvest() ? DynValue.True : DynValue.False;
        public override void Harvest()
        {
            var globalInfo = GlobalInfos.Instance;
            if (_CanHarvest())
            {
                if (halted)
                {  
                    advPositions.ForEach(pos =>
                    {
                        var x = pos.Item1; var y = pos.Item2;
                        var facility = WorkspaceManager.Instance.GetFacility(x, y);
                        var itemType = GlobalInfos.FacilityTypeToItemType(facility.Type);
                        var oldVal = globalInfo.GetItemCountByType(itemType);
                        facility.Harvest();
                        var newVal = globalInfo.GetItemCountByType(itemType);
                        var addon = (newVal - oldVal) * GetLifeTime(x, y);
                        globalInfo.SetItemCountByType(itemType, newVal + addon);
                    });
                    globalInfo.OpusCount += globalInfo.OpusBaseYield * advPositions.Count;
                }
                
                globalInfo.OpusCount += globalInfo.OpusBaseYield;
            }
            WorkspaceManager.Instance.TrySetFacility(X, Y, FacilityType.Empty);
        }
        public void Init(int x, int y)
        {
            transform.position = new Vector3(x, 0, y);
            var time = Random.Range(GlobalConsts.OpusGrowTimeLowerBound, GlobalConsts.OpusGrowTimeUpperBound);
            var edgeLength = GlobalInfos.Instance.WorkspaceEdgeLength;
            lifegameMap = new uint[edgeLength, edgeLength];
            lifegameMapSize = edgeLength;
            objTransform.DOScale(Vector3.one, time)
                .SetEase(Ease.Linear)
                .OnUpdate(() => progress = objTransform.localScale.x)
                .OnComplete(() => progress = 1.0f);
        }
        
        private void OnDestroy()
        {
            objTransform.DOKill();
            OpusOnWorkplace = false;
        }
        
        public override void FreeThis()
        {
            OpusOnWorkplace = false;
            GameObjectPool<FacilityOpus>.Free(this);
        }
        public override void OnAlloc()
        {
            progress = 0.0f;
            OpusOnWorkplace = true;
            objTransform ??= transform.Find("Main").transform;
            objTransform.DOKill();
            objTransform.localScale = Vector3.zero;
        }
    }
}
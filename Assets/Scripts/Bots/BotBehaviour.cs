using System;
using DG.Tweening;
using GlobalSettings;
using MoonSharp.Interpreter;
using UnityEngine;
using Workspace;

namespace Bots
{
    public class BotBehaviour : MonoBehaviour
    {
        public bool BotIsWorking { get; private set; } = false;

        public void GetPosition(out int x, out int y)
        {
            x = Mathf.RoundToInt(this.transform.position.x);
            y = Mathf.RoundToInt(this.transform.position.z);
        }
        public void Move(Vector3 direction)
        {
            if (BotIsWorking) return; BotIsWorking = true;
            var finalPosition = this.transform.position + direction;
            if (finalPosition.x < 0) finalPosition.x = GlobalInfos.Instance.WorkspaceEdgeLength;
            if (finalPosition.x >= GlobalInfos.Instance.WorkspaceEdgeLength) finalPosition.x = 0;
            if (finalPosition.z < 0) finalPosition.z = GlobalInfos.Instance.WorkspaceEdgeLength;
            if (finalPosition.z >= GlobalInfos.Instance.WorkspaceEdgeLength) finalPosition.z = 0;
            
            transform
                .DOMove(finalPosition, GlobalInfos.Instance.MoveTime)
                .SetEase(Ease.OutBack)
                .OnComplete(() => BotIsWorking = false);
        }
        public DynValue CanHarvest()
        {
            if (BotIsWorking) return DynValue.False;
            GetPosition(out var x, out var y);
            return WorkspaceManager.Instance.GetFacility(x, y).CanHarvest();
        }
        public void Harvest()
        {
            if (BotIsWorking) return; BotIsWorking = true;
            var seq =  DOTween.Sequence();
            
            seq.Append(transform.DOMove(Vector3.down, GlobalInfos.Instance.MoveTime / 2))
                .AppendCallback(() => {
                    GetPosition(out var x, out var y);
                    WorkspaceManager.Instance.GetFacility(x, y).Harvest();
                })
                .Append(transform.DOMove(Vector3.up, GlobalInfos.Instance.MoveTime / 2))
                .OnComplete(() => BotIsWorking = false);
        }
    }
}
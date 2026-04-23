using DG.Tweening;
using GlobalSettings;
using Items;
using MoonSharp.Interpreter;
using UnityEngine;
using Workspace;
using Workspace.Facilities;

namespace Bots
{
    public class BotBehaviour : MonoBehaviour
    {
        public bool BotIsWorking { get; private set; } = false;
        public int X => Mathf.RoundToInt(transform.position.x);
        public int Y => Mathf.RoundToInt(transform.position.z);
        
        public void FadeIn()
        {
            if (BotIsWorking) return; BotIsWorking = true;
            transform.localScale = Vector3.zero;
            transform
                .DOScale(Vector3.one, GlobalInfos.Instance.MoveTime)
                .SetEase(Ease.OutQuint)
                .OnComplete(() => BotIsWorking = false);
            transform
                .DORotate(new Vector3(0, 1080, 0), GlobalInfos.Instance.MoveTime, RotateMode.FastBeyond360)
                .SetEase(Ease.OutQuint)
                .OnComplete(
                    () => {
                        Tween tween = null;
                        tween = transform
                            .DORotate(new Vector3(0, 360, 0), GlobalInfos.Instance.MoveTime, RotateMode.FastBeyond360)
                            .SetEase(Ease.Linear)
                            .SetLoops(-1, LoopType.Incremental) // 无限循环
                            .OnUpdate(() => tween.timeScale = GlobalInfos.Instance.TryConsumeItem(ItemType.Iter, 1) ? 1.0f : GlobalConsts.IterBotSpeedBoost);
                    });
        }
        public void FadeOut()
        {
            transform.DOKill(true);
            transform
                .DOScale(Vector3.zero, GlobalInfos.Instance.MoveTime)
                .SetEase(Ease.InQuint)
                .OnComplete(() => Destroy(gameObject));
            transform
                .DORotate(new Vector3(0, 1080, 0), GlobalInfos.Instance.MoveTime, RotateMode.FastBeyond360)
                .SetEase(Ease.InQuint);
        }
        
        public void Move(Vector3 direction)
        {
            if (BotIsWorking) return; BotIsWorking = true;
            var finalPosition = this.transform.position + direction;
            if (finalPosition.x < 0) finalPosition.x = GlobalInfos.Instance.WorkspaceEdgeLength;
            if (finalPosition.x >= GlobalInfos.Instance.WorkspaceEdgeLength) finalPosition.x = 0;
            if (finalPosition.z < 0) finalPosition.z = GlobalInfos.Instance.WorkspaceEdgeLength;
            if (finalPosition.z >= GlobalInfos.Instance.WorkspaceEdgeLength) finalPosition.z = 0;
            
            Tween tween = null;
            tween = transform
                .DOMove(finalPosition, GlobalInfos.Instance.MoveTime)
                .SetEase(Ease.Linear)
                .OnComplete(() => BotIsWorking = false)
                .OnUpdate(() => tween.timeScale = GlobalInfos.Instance.TryConsumeItem(ItemType.Iter, 1) ? 1.0f : GlobalConsts.IterBotSpeedBoost);
        }
        public DynValue CanHarvest() => BotIsWorking ? DynValue.False : WorkspaceManager.Instance.GetFacility(X, Y).CanHarvest();
        public void Harvest()
        {
            if (BotIsWorking) return; BotIsWorking = true;
            
            Sequence sequence = null;
            sequence = DOTween.Sequence(transform)
                .Append(transform.DOMove(Vector3.down, GlobalInfos.Instance.MoveTime / 2).SetRelative(true))
                .AppendCallback(() => WorkspaceManager.Instance.GetFacility(X, Y).Harvest())
                .Append(transform.DOMove(Vector3.up, GlobalInfos.Instance.MoveTime / 2).SetRelative(true))
                .OnComplete(() => BotIsWorking = false)
                .OnUpdate(() => sequence.timeScale = GlobalInfos.Instance.TryConsumeItem(ItemType.Iter, 1) ? 1.0f : GlobalConsts.IterBotSpeedBoost);
        }
        public DynValue TrySetFacility(FacilityType type)
        {
            if (BotIsWorking) return DynValue.False; BotIsWorking = true;
            var couldTryDo = FacilityFactory.CanBuildOn(type, WorkspaceManager.Instance.GetFacility(X, Y).Type);
            if (!couldTryDo) { BotIsWorking = false; return DynValue.False; }
            
            Sequence sequence = null;
            sequence = DOTween.Sequence(transform)
                .Append(transform.DOMove(Vector3.down, GlobalInfos.Instance.MoveTime / 2).SetRelative(true))
                .AppendCallback(() => WorkspaceManager.Instance.TrySetFacility(X, Y, type))
                .Append(transform.DOMove(Vector3.up, GlobalInfos.Instance.MoveTime / 2).SetRelative(true))
                .OnComplete(() => BotIsWorking = false)
                .OnUpdate(() => sequence.timeScale = GlobalInfos.Instance.TryConsumeItem(ItemType.Iter, 1) ? 1.0f : GlobalConsts.IterBotSpeedBoost);
            
            return DynValue.True;
        }
        public DynValue InteractWith(CallbackArguments ctx) => WorkspaceManager.Instance.GetFacility(X, Y).InteractWith(ctx);
        public DynValue TryAddItem(ItemType item) => WorkspaceManager.Instance.GetFacility(X, Y).TryAddItem(item);
        
        private void OnDestroy() => transform.DOKill();
    }
}
using System;
using EditorUIAdaptor.Behaviours;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.InGameUI.InfoWindow
{
    public enum DraggerSide
    {
        Right,
        Bottom,
        Corner,
    }
    
    public class DraggerAdjustor : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        [SerializeField] private RectTransform windowRectTransform;
        [SerializeField] private DraggerSide side;
        
        public void OnDrag(PointerEventData eventData)
        {
            var newSize = side switch
            {
                DraggerSide.Right   => new Vector2(eventData.delta.x, 0) + windowRectTransform.sizeDelta,
                DraggerSide.Bottom  => new Vector2(0, -eventData.delta.y) + windowRectTransform.sizeDelta,
                DraggerSide.Corner  => new Vector2(eventData.delta.x, -eventData.delta.y) + windowRectTransform.sizeDelta,
                _ => throw new ArgumentOutOfRangeException()
            };
            newSize = new Vector2(Mathf.Max(newSize.x, 480f), Mathf.Max(newSize.y, 270f));
            if (windowRectTransform.sizeDelta == newSize) return;
            windowRectTransform.sizeDelta = newSize;
        }
        public void OnEndDrag(PointerEventData eventData) => EventSystem.current.SetSelectedGameObject(UIBGMouseListener.Instance.gameObject);
    }
}


using System;
using Riten.Native.Cursors;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

namespace EditorUIAdaptor.Behaviours
{
    public enum DraggerSide
    {
        Right,
        Bottom,
        Corner,
    }
    
    public class DraggerAdjustor : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        [SerializeField] private WindowAdjustor windowAdjustor;
        [SerializeField] private DraggerSide side;
        
        public void OnDrag(PointerEventData eventData)
        {
            var newSize = side switch
            {
                DraggerSide.Right   => new Vector2(eventData.delta.x, 0) + windowAdjustor.mainWindow.sizeDelta,
                DraggerSide.Bottom  => new Vector2(0, -eventData.delta.y) + windowAdjustor.mainWindow.sizeDelta,
                DraggerSide.Corner  => new Vector2(eventData.delta.x, -eventData.delta.y) + windowAdjustor.mainWindow.sizeDelta,
                _ => throw new ArgumentOutOfRangeException()
            };
            windowAdjustor.TryResizeWindow(newSize.x, newSize.y);
        }
        public void OnEndDrag(PointerEventData eventData) => EventSystem.current.SetSelectedGameObject(UIBGMouseListener.Instance.gameObject);
    }
}

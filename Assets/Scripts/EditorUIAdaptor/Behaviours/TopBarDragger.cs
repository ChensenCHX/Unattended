using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EditorUIAdaptor.Behaviours
{
    public class TopBarDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public RectTransform editorHolder;

        private Vector2 offset;
        public void OnBeginDrag(PointerEventData eventData)
        {
            CameraController.Lock();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                EditorWindowManager.Instance.RectTransform,
                eventData.pressPosition, 
                WindowCamera.Instance.Camera, 
                out var position
            );
            offset = editorHolder.anchoredPosition - position;
        }
        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                EditorWindowManager.Instance.RectTransform,
                
                eventData.position, 
                WindowCamera.Instance.Camera, 
                out var position
            );
            editorHolder.anchoredPosition = offset + position;
        }
        public void OnEndDrag(PointerEventData eventData) => EventSystem.current.SetSelectedGameObject(UIBGMouseListener.Instance.gameObject);
    }
}

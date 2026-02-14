using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EditorUIAdaptor
{
    public class FocusDetector : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerDownHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private void Start() => EventSystem.current.SetSelectedGameObject(gameObject);

        public void OnSelect(BaseEventData eventData) => CameraController.Unlock();
        public void OnDeselect(BaseEventData eventData) => CameraController.Lock();
        public void OnPointerDown(PointerEventData eventData) => EventSystem.current.SetSelectedGameObject(gameObject);

        private static Plane _groundPlane = new Plane(Vector3.up, 0);
        private Vector3 lastPoint;
        public void OnDrag(PointerEventData eventData)
        {
            var ray = CameraController.Instance.Camera.ScreenPointToRay(eventData.position);
            if (!_groundPlane.Raycast(ray, out var enter)) throw new InvalidOperationException("This should never happen!");
            var thisPoint = ray.GetPoint(enter);
            var offset = thisPoint - lastPoint;
            CameraController.Instance.AddExtraMovement(new Vector3(-offset.x, 0, -offset.z));
            lastPoint = thisPoint;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            var ray = CameraController.Instance.Camera.ScreenPointToRay(eventData.position);
            if (!_groundPlane.Raycast(ray, out var enter)) throw new InvalidOperationException("This should never happen!");
            lastPoint = ray.GetPoint(enter);
        }
        public void OnEndDrag(PointerEventData eventData) { }
    }
}

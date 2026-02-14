using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EditorUIAdaptor
{
    public class UIBGMouseListener : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerDownHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private void Start() => EventSystem.current.SetSelectedGameObject(gameObject);

        public void OnSelect(BaseEventData eventData) => CameraController.Unlock();
        public void OnDeselect(BaseEventData eventData) => CameraController.Lock();
        public void OnPointerDown(PointerEventData eventData) => EventSystem.current.SetSelectedGameObject(gameObject);

        private static Plane _groundPlane = new Plane(Vector3.up, 0);
        public void OnDrag(PointerEventData eventData)
        {
            var ray1 = CameraController.Instance.Camera.ScreenPointToRay(eventData.delta);
            var ray2 = CameraController.Instance.Camera.ScreenPointToRay(Vector3.zero);
            if (!_groundPlane.Raycast(ray1, out var enter1) || !_groundPlane.Raycast(ray2, out var enter2)) 
                throw new InvalidOperationException("This should never happen!");
            
            var movPoint = ray1.GetPoint(enter1);
            var refPoint = ray2.GetPoint(enter2);
            var offset = refPoint - movPoint;
            CameraController.Instance.AddExtraMovement(new Vector3(offset.x, 0, offset.z));
        }

        public void OnBeginDrag(PointerEventData eventData) { }
        public void OnEndDrag(PointerEventData eventData) { }
    }
}

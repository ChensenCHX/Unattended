using UnityEngine;
using UnityEngine.EventSystems;

namespace EditorUIAdaptor
{
    public class FocusDetector : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerDownHandler
    {
        private void Start() => EventSystem.current.SetSelectedGameObject(gameObject);

        public void OnSelect(BaseEventData eventData) => CameraController.Unlock();
        public void OnDeselect(BaseEventData eventData) => CameraController.Lock();
        public void OnPointerDown(PointerEventData eventData) => EventSystem.current.SetSelectedGameObject(gameObject);
    }
}

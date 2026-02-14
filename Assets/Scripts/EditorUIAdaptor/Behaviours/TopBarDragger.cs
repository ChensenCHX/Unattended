using UnityEngine;
using UnityEngine.EventSystems;

namespace EditorUIAdaptor.Behaviours
{
    public class TopBarDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public RectTransform editorHolder;

        public void OnBeginDrag(PointerEventData eventData) => CameraController.Lock();
        public void OnDrag(PointerEventData eventData) => editorHolder.anchoredPosition += eventData.delta / AnchoredToWorld.Instance.EditorScale;
        public void OnEndDrag(PointerEventData eventData) => EventSystem.current.SetSelectedGameObject(UIBGMouseListener.Instance.gameObject);
    }
}

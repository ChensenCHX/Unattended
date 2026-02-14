using UnityEngine;
using UnityEngine.EventSystems;

namespace EditorUIAdaptor
{
    public class TopBarDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public RectTransform editorHolder;

        public void OnBeginDrag(PointerEventData eventData) { }
        public void OnDrag(PointerEventData eventData) => editorHolder.anchoredPosition += eventData.delta;
        public void OnEndDrag(PointerEventData eventData) { }
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace EditorUIAdaptor.Behaviours
{
    public class AnchoredToWorld : SingletonMono<AnchoredToWorld>
    {
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private RectTransform canvasRectTransform;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private float defaultY = 5;

        public Vector3 EditorScale => rectTransform.localScale;
        private readonly List<RaycastResult> raycastResults = new();
        private void Update()
        {
            // change render order
            
            if (Input.GetMouseButtonDown(0))
            {
                var pointerData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
                raycastResults.Clear();
                EventSystem.current.RaycastAll(pointerData, raycastResults);

                foreach (var result in raycastResults)
                {
                    var current = result.gameObject.transform;
                    while (current != null && current.parent != transform) current = current.parent;

                    if (current == null || current.parent != transform || current.GetComponent<EditorWindowHandler>().GetTextEditor().DisableInput) continue;
                    current.SetAsLastSibling(); break;
                }
            }
            
            // change offsets
            var screenPos = CameraController.Instance.Camera.WorldToScreenPoint(Vector3.zero);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRectTransform,
                screenPos,
                null,
                out var localPos
            );
            
            rectTransform.anchoredPosition = localPos;
            rectTransform.localScale = Vector3.one * (defaultY / screenPos.z);
        }
    }
}

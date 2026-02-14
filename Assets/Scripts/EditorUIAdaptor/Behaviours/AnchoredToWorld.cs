using UnityEngine;
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
        private void Update()
        {
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

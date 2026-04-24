using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.InGameUI.UpgradePanel
{
    public class ViewportClamper : MonoBehaviour, IScrollHandler, IDragHandler, IBeginDragHandler
    {
        [SerializeField] private RectTransform viewport;
        [SerializeField] private RectTransform contentRect;
        [SerializeField] private float minScale = 0.5f;
        [SerializeField] private float maxScale = 4.0f;
        [SerializeField] private float zoomSpeed = 0.1f;

        private Vector2? _lastPointerPosition;
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    viewport,
                    eventData.position,
                    eventData.pressEventCamera,
                    out var localPointer))
            {
                _lastPointerPosition = localPointer;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    viewport,
                    eventData.position,
                    eventData.pressEventCamera,
                    out var currentPointer)) return;

            if (_lastPointerPosition.HasValue)
            {
                var delta = currentPointer - _lastPointerPosition.Value;
                contentRect.anchoredPosition += delta;
            }
            
            _lastPointerPosition = currentPointer;
            
            ClampContentToViewport();
        }

        public void OnScroll(PointerEventData eventData)
        {
            // 以鼠标位置为中心缩放（体验更好）
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                contentRect, 
                eventData.position, 
                eventData.pressEventCamera, 
                out var mousePos);

            var delta = eventData.scrollDelta.y;
            var scaleFactor = 1f + delta * zoomSpeed;
            var newScale = contentRect.localScale * scaleFactor;
            newScale.x = Mathf.Clamp(newScale.x, minScale, maxScale);
            newScale.y = Mathf.Clamp(newScale.y, minScale, maxScale);
            newScale.z = 1f;

            // 调整位置以保持鼠标指向的点不动
            Vector2 oldScale = contentRect.localScale;
            var scaleChange = newScale / oldScale;
            contentRect.localScale = newScale;
            contentRect.anchoredPosition += mousePos * (Vector2.one - scaleChange);

            ClampContentToViewport();
        }

        private void ClampContentToViewport()
        {
            if (viewport == null || contentRect.childCount == 0) return;

            // 获取视口的世界坐标边界
            var viewCorners = new Vector3[4];
            viewport.GetWorldCorners(viewCorners);
            var viewMin = new Vector2(viewCorners[0].x, viewCorners[0].y); // 左下
            var viewMax = new Vector2(viewCorners[2].x, viewCorners[2].y); // 右上

            // 获取 content 的世界坐标边界
            var contentCorners = new Vector3[4];
            contentRect.GetWorldCorners(contentCorners);
            var contentWorldMin = new Vector2(contentCorners[0].x, contentCorners[0].y);
            var contentWorldMax = new Vector2(contentCorners[2].x, contentCorners[2].y);

            var clampedPos = contentRect.anchoredPosition;

            // 计算需要调整的偏移量
            // 左边缘子物体不能超出视口左边
            var leftEdge = GetLeftmostEdgeInWorldSpace();
            if (leftEdge.HasValue)
            {
                var offset = viewMin.x - leftEdge.Value;
                if (offset > 0)
                    clampedPos.x += offset;
            }
            // 右边缘子物体不能超出视口右边
            var rightEdge = GetRightmostEdgeInWorldSpace();
            if (rightEdge.HasValue)
            {
                var offset = viewMax.x - rightEdge.Value;
                if (offset < 0)
                    clampedPos.x += offset;
            }
            // 下边缘子物体不能超出视口下边
            var bottomEdge = GetBottommostEdgeInWorldSpace();
            if (bottomEdge.HasValue)
            {
                var offset = viewMin.y - bottomEdge.Value;
                if (offset > 0)
                    clampedPos.y += offset;
            }
            // 上边缘子物体不能超出视口上边
            var topEdge = GetTopmostEdgeInWorldSpace();
            if (topEdge.HasValue)
            {
                var offset = viewMax.y - topEdge.Value;
                if (offset < 0)
                    clampedPos.y += offset;
            }
            
            contentRect.anchoredPosition = clampedPos;
        }

        private float? GetLeftmostEdgeInWorldSpace()
        {
            float? minX = null;
            var childCount = contentRect.childCount;
            for (var i = 0; i < childCount; i++)
            {
                var child = contentRect.GetChild(i) as RectTransform;
                if (child == null) continue;

                var corners = new Vector3[4];
                child.GetWorldCorners(corners);
                for (var j = 0; j < 4; j++)
                {
                    if (!minX.HasValue || corners[j].x < minX.Value)
                        minX = corners[j].x;
                }
            }
            return minX;
        }

        private float? GetRightmostEdgeInWorldSpace()
        {
            float? maxX = null;
            var childCount = contentRect.childCount;
            for (var i = 0; i < childCount; i++)
            {
                var child = contentRect.GetChild(i) as RectTransform;
                if (child == null) continue;

                var corners = new Vector3[4];
                child.GetWorldCorners(corners);
                for (var j = 0; j < 4; j++)
                {
                    if (!maxX.HasValue || corners[j].x > maxX.Value)
                        maxX = corners[j].x;
                }
            }
            return maxX;
        }

        private float? GetBottommostEdgeInWorldSpace()
        {
            float? minY = null;
            var childCount = contentRect.childCount;
            for (var i = 0; i < childCount; i++)
            {
                var child = contentRect.GetChild(i) as RectTransform;
                if (child == null) continue;

                var corners = new Vector3[4];
                child.GetWorldCorners(corners);
                for (var j = 0; j < 4; j++)
                {
                    if (!minY.HasValue || corners[j].y < minY.Value)
                        minY = corners[j].y;
                }
            }
            return minY;
        }

        private float? GetTopmostEdgeInWorldSpace()
        {
            float? maxY = null;
            var childCount = contentRect.childCount;
            for (var i = 0; i < childCount; i++)
            {
                var child = contentRect.GetChild(i) as RectTransform;
                if (child == null) continue;

                var corners = new Vector3[4];
                child.GetWorldCorners(corners);
                for (var j = 0; j < 4; j++)
                {
                    if (!maxY.HasValue || corners[j].y > maxY.Value)
                        maxY = corners[j].y;
                }
            }
            return maxY;
        }

        private (float? left, float? right, float? bottom, float? top) FindEdgeChildren()
        {
            var childCount = contentRect.childCount;
            if (childCount == 0) return (null, null, null, null);

            float? leftMin = null;
            float? rightMax = null;
            float? bottomMin = null;
            float? topMax = null;

            for (var i = 0; i < childCount; i++)
            {
                var child = contentRect.GetChild(i) as RectTransform;
                if (child == null) continue;

                // 获取子物体在 contentRect 本地空间中的边界
                var childBounds = GetChildBoundsInContentSpace(child);

                // 找最左边的子物体
                if (!leftMin.HasValue || childBounds.min.x < leftMin.Value)
                    leftMin = childBounds.min.x;
                // 找最右边的子物体
                if (!rightMax.HasValue || childBounds.max.x > rightMax.Value)
                    rightMax = childBounds.max.x;
                // 找最下边的子物体
                if (!bottomMin.HasValue || childBounds.min.y < bottomMin.Value)
                    bottomMin = childBounds.min.y;
                // 找最上边的子物体
                if (!topMax.HasValue || childBounds.max.y > topMax.Value)
                    topMax = childBounds.max.y;
            }

            return (leftMin, rightMax, bottomMin, topMax);
        }

        private Bounds GetChildBoundsInContentSpace(RectTransform child)
        {
            var bounds = new Bounds();
            var first = true;

            var corners = new Vector3[4];
            child.GetWorldCorners(corners);
            for (var j = 0; j < 4; j++)
            {
                var localPoint = contentRect.InverseTransformPoint(corners[j]);
                if (first)
                {
                    bounds = new Bounds(localPoint, Vector3.zero);
                    first = false;
                }
                else
                {
                    bounds.Encapsulate(localPoint);
                }
            }

            return bounds;
        }

        private Bounds CalculateContentBoundsRelativeToContent()
        {
            // 只获取直接子物体
            var childCount = contentRect.childCount;
            if (childCount == 0) return new Bounds(Vector3.zero, Vector3.zero);

            var bounds = new Bounds();
            var first = true;
            for (var i = 0; i < childCount; i++)
            {
                var child = contentRect.GetChild(i) as RectTransform;
                if (child == null) continue;

                // 将子物体的角点转换到 contentRect 的本地空间
                // 这样得到的是固定边界，不随 anchoredPosition 变化
                var corners = new Vector3[4];
                child.GetWorldCorners(corners);
                for (var j = 0; j < 4; j++)
                {
                    var localPoint = contentRect.InverseTransformPoint(corners[j]);
                    if (first)
                    {
                        bounds = new Bounds(localPoint, Vector3.zero);
                        first = false;
                    }
                    else
                    {
                        bounds.Encapsulate(localPoint);
                    }
                }
            }
            return bounds;
        }
    }
}
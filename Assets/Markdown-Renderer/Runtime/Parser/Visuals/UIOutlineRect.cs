using UnityEngine;
using UnityEngine.UI;

namespace MarkdownToUnity.Runtime
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class UIOutlineRect : MaskableGraphic
    {
        public float thickness = .5f;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            Rect rect = rectTransform.rect;

            float xMin = rect.xMin;
            float xMax = rect.xMax;
            float yMin = rect.yMin;
            float yMax = rect.yMax;

            float t = thickness;

            // Vier Seiten zeichnen
            DrawQuad(vh, new Vector2(xMin, yMin), new Vector2(xMax, yMin + t)); // Bottom
            DrawQuad(vh, new Vector2(xMin, yMax - t), new Vector2(xMax, yMax)); // Top
            DrawQuad(vh, new Vector2(xMin, yMin + t), new Vector2(xMin + t, yMax - t)); // Left
            DrawQuad(vh, new Vector2(xMax - t, yMin + t), new Vector2(xMax, yMax - t)); // Right
        }

        private void DrawQuad(VertexHelper vh, Vector2 corner1, Vector2 corner2)
        {
            int idx = vh.currentVertCount;

            UIVertex v = UIVertex.simpleVert;
            v.color = color;

            v.position = new Vector3(corner1.x, corner1.y); vh.AddVert(v);
            v.position = new Vector3(corner1.x, corner2.y); vh.AddVert(v);
            v.position = new Vector3(corner2.x, corner2.y); vh.AddVert(v);
            v.position = new Vector3(corner2.x, corner1.y); vh.AddVert(v);

            vh.AddTriangle(idx + 0, idx + 1, idx + 2);
            vh.AddTriangle(idx + 2, idx + 3, idx + 0);
        }
    }
}

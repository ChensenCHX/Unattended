#define UI_ELEMENTS_UXML_ATTRIBUTES
using UnityEngine;
using UnityEngine.UIElements;

namespace MusiciMarkdownToUnity.Editor
{
    public enum GradientDirection
    {
        Horizontal,
        Vertical,
        DiagonalTopLeftToBottomRight,
        DiagonalBottomLeftToTopRight,
        Radial
    }
#if UNITY_6000_0_OR_NEWER
    [UxmlElement]
#endif
    public partial class GradientElement : VisualElement
    {
#if !UNITY_6000_0_OR_NEWER
        // 那没办法 手工注册吧
        public new class UxmlFactory : UxmlFactory<GradientElement, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlEnumAttributeDescription<GradientDirection> m_Direction = new UxmlEnumAttributeDescription<GradientDirection> { name = "gradient-direction", defaultValue = GradientDirection.Horizontal };
            UxmlColorAttributeDescription m_From = new UxmlColorAttributeDescription { name = "gradient-from", defaultValue = new Color(0.2f, 0.4f, 0.8f) };
            UxmlColorAttributeDescription m_To = new UxmlColorAttributeDescription { name = "gradient-to", defaultValue = new Color(0.1f, 0.1f, 0.1f) };
            UxmlBoolAttributeDescription m_Gamma = new UxmlBoolAttributeDescription { name = "use-gamma-correction", defaultValue = true };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var element = ve as GradientElement;
                element.GradientDirection = m_Direction.GetValueFromBag(bag, cc);
                element.GradientFrom = m_From.GetValueFromBag(bag, cc);
                element.GradientTo = m_To.GetValueFromBag(bag, cc);
                element.UseGammaCorrection = m_Gamma.GetValueFromBag(bag, cc);
            }
        }
#endif
        
        static readonly Vertex[] _vertices = new Vertex[4];
        static readonly ushort[] _indices = { 0, 1, 2, 2, 3, 0 };
        private VisualElement _gradient;

#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute]
#endif
        public GradientDirection GradientDirection { get; set; } = GradientDirection.Horizontal;

#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute]
#endif
        public Color GradientFrom { get; set; } = new Color(0.2f, 0.4f, 0.8f);

#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute]
#endif
        public Color GradientTo { get; set; } = new Color(0.1f, 0.1f, 0.1f);

#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute]
#endif
        public bool UseGammaCorrection { get; set; } = true;

        public GradientElement()
        {
            style.overflow = Overflow.Hidden;

            _gradient = new VisualElement { name = "Gradient" };
            _gradient.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            _gradient.style.height = new StyleLength(new Length(100, LengthUnit.Percent));

            _gradient.generateVisualContent += GenerateVisualContent;
            hierarchy.Add(_gradient);

            RegisterCallback<GeometryChangedEvent>(_ => _gradient.MarkDirtyRepaint());
        }

        private void GenerateVisualContent(MeshGenerationContext ctx)
        {
            var rect = _gradient.contentRect;
            if (rect.width < 0.1f || rect.height < 0.1f)
                return;

            UpdateVerticesPosition(rect);
            UpdateVerticesTint(rect);

            var meshWriteData = ctx.Allocate(_vertices.Length, _indices.Length);
            meshWriteData.SetAllVertices(_vertices);
            meshWriteData.SetAllIndices(_indices);
        }

        private static void UpdateVerticesPosition(Rect rect)
        {
            float left = 0f;
            float right = rect.width;
            float top = 0f;
            float bottom = rect.height;

            _vertices[0].position = new Vector3(left, bottom, Vertex.nearZ);
            _vertices[1].position = new Vector3(left, top, Vertex.nearZ);
            _vertices[2].position = new Vector3(right, top, Vertex.nearZ);
            _vertices[3].position = new Vector3(right, bottom, Vertex.nearZ);
        }

        private void UpdateVerticesTint(Rect rect)
        {
            Color from = UseGammaCorrection ? GradientFrom.gamma : GradientFrom.linear;
            Color to = UseGammaCorrection ? GradientTo.gamma : GradientTo.linear;

            switch (GradientDirection)
            {
                case GradientDirection.Horizontal:
                    _vertices[0].tint = from;
                    _vertices[1].tint = from;
                    _vertices[2].tint = to;
                    _vertices[3].tint = to;
                    break;

                case GradientDirection.Vertical:
                    _vertices[0].tint = to;
                    _vertices[1].tint = from;
                    _vertices[2].tint = from;
                    _vertices[3].tint = to;
                    break;

                case GradientDirection.DiagonalTopLeftToBottomRight:
                    _vertices[0].tint = from;
                    _vertices[1].tint = from;
                    _vertices[2].tint = to;
                    _vertices[3].tint = to;
                    break;

                case GradientDirection.DiagonalBottomLeftToTopRight:
                    _vertices[0].tint = to;
                    _vertices[1].tint = from;
                    _vertices[2].tint = from;
                    _vertices[3].tint = to;
                    break;

                case GradientDirection.Radial:
                    ApplyRadialTint(rect, from, to);
                    break;
            }
        }

        private static void ApplyRadialTint(Rect rect, Color from, Color to)
        {
            Color mid = Color.Lerp(from, to, 0.5f);
            _vertices[0].tint = mid;
            _vertices[1].tint = from;
            _vertices[2].tint = mid;
            _vertices[3].tint = to;
        }

        public void SetColors(Color from, Color to)
        {
            GradientFrom = from;
            GradientTo = to;
            _gradient.MarkDirtyRepaint();
        }

        public void SetDirection(GradientDirection dir)
        {
            GradientDirection = dir;
            _gradient.MarkDirtyRepaint();
        }
    }
   
}
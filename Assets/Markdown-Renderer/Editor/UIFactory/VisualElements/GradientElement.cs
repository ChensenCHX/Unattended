using UnityEngine;
using UnityEngine.UIElements;

namespace MarkdownToUnity.Editor
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
        readonly Vertex[] _vertices = new Vertex[4];
        static readonly ushort[] _indices = { 0, 1, 2, 2, 3, 0 };
        private GradientDirection _direction = GradientDirection.Horizontal;
        private Color _from = new Color(0.2f, 0.4f, 0.8f);
        private Color _to = new Color(0.1f, 0.1f, 0.1f);
        private bool _gamma = true;

#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute]
#endif
        public GradientDirection GradientDirection
        {
            get => _direction;
            set
            {
                if (_direction == value) return;
                _direction = value;
                MarkDirtyRepaint();
            }
        }

#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute]
#endif
        public Color GradientFrom
        {
            get => _from;
            set
            {
                if (_from == value) return;
                _from = value;
                MarkDirtyRepaint();
            }
        }

#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute]
#endif
        public Color GradientTo
        {
            get => _to;
            set
            {
                if (_to == value) return;
                _to = value;
                MarkDirtyRepaint();
            }
        }

#if UNITY_6000_0_OR_NEWER
        [UxmlAttribute]
#endif
        public bool UseGammaCorrection
        {
            get => _gamma;
            set
            {
                if (_gamma == value) return;
                _gamma = value;
                MarkDirtyRepaint();
            }
        }

        public GradientElement()
        {
            generateVisualContent += GenerateVisualContent;
        }

        void GenerateVisualContent(MeshGenerationContext ctx)
        {
            Rect rect = contentRect;

            if (rect.width < 0.01f || rect.height < 0.01f)
                return;

            UpdateVerticesPosition(rect);
            UpdateVerticesTint(rect);

            var mesh = ctx.Allocate(4, 6);
            mesh.SetAllVertices(_vertices);
            mesh.SetAllIndices(_indices);
        }

        void UpdateVerticesPosition(Rect rect)
        {
            float left = 0;
            float right = rect.width;
            float top = 0;
            float bottom = rect.height;

            _vertices[0].position = new Vector3(left, bottom, Vertex.nearZ);
            _vertices[1].position = new Vector3(left, top, Vertex.nearZ);
            _vertices[2].position = new Vector3(right, top, Vertex.nearZ);
            _vertices[3].position = new Vector3(right, bottom, Vertex.nearZ);
        }

        void UpdateVerticesTint(Rect rect)
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
                    Color mid = Color.Lerp(from, to, 0.5f);
                    _vertices[0].tint = mid;
                    _vertices[1].tint = from;
                    _vertices[2].tint = mid;
                    _vertices[3].tint = to;
                    break;
            }
        }

        public void SetColors(Color from, Color to)
        {
            GradientFrom = from;
            GradientTo = to;
        }

        public void SetDirection(GradientDirection dir)
        {
            GradientDirection = dir;
        }

#if !UNITY_6000_0_OR_NEWER
        public new class UxmlFactory : UxmlFactory<GradientElement, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlEnumAttributeDescription<GradientDirection> direction =
                new() { name = "gradient-direction", defaultValue = GradientDirection.Horizontal };

            UxmlColorAttributeDescription from =
                new() { name = "gradient-from", defaultValue = new Color(0.2f, 0.4f, 0.8f) };

            UxmlColorAttributeDescription to =
                new() { name = "gradient-to", defaultValue = new Color(0.1f, 0.1f, 0.1f) };

            UxmlBoolAttributeDescription gamma =
                new() { name = "use-gamma-correction", defaultValue = true };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var e = (GradientElement)ve;

                e.GradientDirection = direction.GetValueFromBag(bag, cc);
                e.GradientFrom = from.GetValueFromBag(bag, cc);
                e.GradientTo = to.GetValueFromBag(bag, cc);
                e.UseGammaCorrection = gamma.GetValueFromBag(bag, cc);
            }
        }
#endif
    }
}
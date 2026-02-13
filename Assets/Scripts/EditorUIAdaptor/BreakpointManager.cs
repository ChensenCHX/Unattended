using System;
using InGameTextEditor;
using InGameTextEditor.Format;
using UnityEngine;
using TextEditor = InGameTextEditor.TextEditor;

namespace EditorUIAdaptor
{
    public class BreakpointManager : TextFormatter
    {
        public TextEditor textEditor;
        public RectTransform lineIconTransform;
        public RectTransform lineIconContainerTransform;
        public Sprite labelIconSprite;
        public Color breakpointLabelColor = Color.red;
        
        private static readonly Color _dummyColor = Color.black;
        private bool mouseDownBefore = false;
        
        void Update()
        {
            var mouseDownNow = Input.GetMouseButton(0);
            if (mouseDownBefore == mouseDownNow) return;    // skip if not at flip frame
            mouseDownBefore = mouseDownNow;
            if (!textEditor.EditorActive) return;           // skip if not active
            if (!Input.GetMouseButton(0)) return;           // skip if no input
            if (!RectTransformUtility.RectangleContainsScreenPoint(lineIconTransform, Input.mousePosition, null)) return;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(lineIconTransform, Input.mousePosition, null, out var mousePosition);

            var lineAt = (int)((-mousePosition.y + lineIconContainerTransform.anchoredPosition.y) / textEditor.CharacterHeight);
            if (lineAt >= textEditor.Lines.Count) return;   // illegal line (not exist)
            
            var line = textEditor.Lines[lineAt];
            
            if (line.Labels.Count != 0)
            {
                line.RemoveLabels();
                // TODO:: send breakpoint change message here
            }
            else
            {
                line.AddLabel(0, 0, 
                    null, _dummyColor,
                    labelIconSprite, breakpointLabelColor,
                    null, 
                    Line.Label.DeleteCondition.ANYTHING_CHANGES);
                // TODO:: send breakpoint change message here
            }
        }

        public override bool Initialized { get; } = true;
        public override void Init() { }
        public override void OnLineChanged(Line line)
        {
            // TODO:: send breakpoint change message here
            // delete every breakpoints
        }
    }
}
using CodeExecutor;
using InGameTextEditor;
using InGameTextEditor.Format;
using UnityEngine;
using TextEditor = InGameTextEditor.TextEditor;

namespace EditorUIAdaptor.Behaviours
{
    public class BreakpointManager : TextFormatter
    {
        public EditorWindowHandler windowHandler;
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
            if (windowHandler.transform.GetSiblingIndex() != windowHandler.transform.parent.childCount - 1) return; // skip if not first editor
            if (!RectTransformUtility.RectangleContainsScreenPoint(lineIconTransform, Input.mousePosition, null)) return;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(lineIconTransform, Input.mousePosition, null, out var mousePosition);

            var lineAt = (int)((-mousePosition.y + lineIconContainerTransform.anchoredPosition.y) / textEditor.CharacterHeight);
            if (lineAt >= textEditor.Lines.Count) return;   // illegal line (not exist)
            
            var line = textEditor.Lines[lineAt];
            
            if (line.Labels.Count != 0)
            {
                line.RemoveLabels();
                windowHandler.RemoveBreakpoint(lineAt + 1);
                CodeService.Instance.RemoveBreakpoint(windowHandler, lineAt + 1);
            }
            else
            {
                line.AddLabel(0, 0, 
                    null, _dummyColor,
                    labelIconSprite, breakpointLabelColor,
                    null, 
                    Line.Label.DeleteCondition.ANYTHING_CHANGES);
                windowHandler.AddBreakpoint(lineAt + 1);
                CodeService.Instance.AddBreakpoint(windowHandler, lineAt + 1);
            }
        }

        public override bool Initialized { get; } = true;
        public override void Init() { }
        
        private int lastExecFrame = -1;
        public override void OnLineChanged(Line _)
        {
            if (Time.frameCount == lastExecFrame) return;   // 一个update执行一次就可以 没必要每行都要触发
            lastExecFrame = Time.frameCount;
            
            windowHandler.ClearBreakpoints();
            CodeService.Instance.ResetBreakpoint(windowHandler);
        }
    }
}
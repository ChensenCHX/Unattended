using InGameTextEditor;
using InGameTextEditor.Format;
using UnityEngine;
using TextEditor = InGameTextEditor.TextEditor;

namespace EditorUIAdaptor.Behaviours
{
    public class WindowAdjustor : TextFormatter
    {
        public TextEditor textEditor;
        public RectTransform mainWindow;
        public RectTransform codeWindow;
        public RectTransform lineNumberBar;
        
        public bool TryResizeWindow(float width, float height)
        {
            var success = true;
            var minWidth = Mathf.Max(480f, 
                textEditor.LongestLineWidth
                    + textEditor.CharacterWidth
                    + textEditor.CharacterWidth
                    + textEditor.lineLabelIconsWidth
                    + textEditor.scrollbarWidth 
                    + lineNumberBar.sizeDelta.x
                    + codeWindow.offsetMin.x
                    + (-codeWindow.offsetMax.x));
            var minHeight = 270f;
            
            if (width < minWidth ||  height < minHeight) success = false;
            width = Mathf.Max(width, minWidth);
            height = Mathf.Max(height, minHeight);
            mainWindow.sizeDelta = new Vector2(width, height);

            var widthChanged = !Mathf.Approximately(width, mainWindow.sizeDelta.x);
            if (widthChanged) textEditor.UpdateLayout();
            return success;
        }
        
        public override bool Initialized { get; } = true;
        public override void Init() { }
        
        private int lastExecFrame = -1;
        public override void OnLineChanged(Line _)
        {
            if (Time.frameCount == lastExecFrame) return;   // 一个update执行一次就可以 没必要每行都要触发
            lastExecFrame = Time.frameCount;
            var minWidth = Mathf.Max(480f, 
                textEditor.LongestLineWidth
                    + textEditor.CharacterWidth
                    + textEditor.CharacterWidth
                    + textEditor.lineLabelIconsWidth
                    + textEditor.scrollbarWidth 
                    + lineNumberBar.sizeDelta.x
                    + codeWindow.offsetMin.x
                    + (-codeWindow.offsetMax.x));
            
            TryResizeWindow(Mathf.Max(minWidth, mainWindow.sizeDelta.x), mainWindow.sizeDelta.y);
        }
    }
}
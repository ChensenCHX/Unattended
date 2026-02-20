using System.Collections.Generic;
using CodeExecutor;
using InGameTextEditor;
using UnityEngine;
using TextEditor = InGameTextEditor.TextEditor;

namespace EditorUIAdaptor.Behaviours
{
    public class EditorWindowHandler : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private WindowAdjustor windowAdjustor;
        [SerializeField] private ScriptNameAdjustor scriptName;
        [SerializeField] private TextEditor textEditor;
        [SerializeField] private ResumeButton resumeButton;
        [SerializeField] private StepButton stepButton;
        
        public string GetWindowName() => scriptName.ScriptName;
        public string GetScript() => textEditor.Text;
        public Vector2 GetWindowSize() => rectTransform.sizeDelta;
        public Vector2 GetWindowPosition() => rectTransform.anchoredPosition;
        public TextEditor GetTextEditor() => textEditor;

        private readonly TextPosition highlightLeft = new(0, 0);
        private readonly TextPosition highlightRight = new(0, 0);
        private readonly Selection highlightSelector = new(new TextPosition(0, 0), new TextPosition(0,0));
        public void HighlightZone(int startLine, int startChar, int endLine, int endChar)
        {
            highlightLeft.lineIndex = startLine; highlightLeft.colIndex = startChar;
            highlightRight.lineIndex = endLine; highlightRight.colIndex = endChar;
            highlightSelector.start = highlightLeft; highlightSelector.end = highlightRight;
            textEditor.Selection = highlightSelector;
        }
        public void StopHighlightZone() => textEditor.Selection = null;
        public void SetRunningState(CodeService.WorkingState runningState)
        {
            resumeButton.RunningState = runningState;
            stepButton.RunningState = runningState;
        }

        private readonly HashSet<int> breakpointList = new();
        public void AddBreakpoint(int lineAt) => breakpointList.Add(lineAt);
        public void RemoveBreakpoint(int lineAt) => breakpointList.Remove(lineAt);
        public void ClearBreakpoints() => breakpointList.Clear();
        public HashSet<int> GetBreakpoints() => breakpointList;

        public void Init(string windowName=null, string text=null, float x=0, float y=0, float width=0, float height=0)
        {
            scriptName.Init(windowName);
            textEditor.SetText(text ?? "", true);
            rectTransform.anchoredPosition = new Vector2(x, y);
            windowAdjustor.TryResizeWindow(width, height);
        }
    }
}

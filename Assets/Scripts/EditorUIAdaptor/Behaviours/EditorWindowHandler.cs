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
        
        public string GetWindowName() => scriptName.ScriptName;
        public string GetScript() => textEditor.Text;
        public Vector2 GetWindowSize() => rectTransform.sizeDelta;
        public Vector2 GetWindowPosition() => rectTransform.anchoredPosition;

        public void Init(string windowName=null, string text=null, int x=0, int y=0, int width=0, int height=0)
        {
            scriptName.Init(windowName);
            textEditor.SetText(text ?? "", true);   // TODO:: try not immediately set later
            rectTransform.anchoredPosition = new Vector2(x, y);
            windowAdjustor.TryResizeWindow(width, height);
        }
    }
}

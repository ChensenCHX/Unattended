using System.Collections.Generic;
using EditorUIAdaptor.Behaviours;
using JetBrains.Annotations;
using UnityEngine;
using Utils;

namespace EditorUIAdaptor
{
    public class EditorWindowManager : SingletonMono<EditorWindowManager>
    {
        public GameObject CodeEditorPrefab;
        public RectTransform RectTransform;
        
        private readonly List<EditorWindowHandler> windowHandlers = new();
        public HashSet<string> ProcessedWindowNames { get; } = new();

        [CanBeNull] 
        public EditorWindowHandler FindWindow(string windowName) => windowHandlers.Find(w => w.GetWindowName().ToUpperInvariant() == windowName);
        public IReadOnlyList<EditorWindowHandler> GetAllWindows() => windowHandlers;
        public void CreateEditorWindow(string windowName=null, string text=null, float x=0, float y=0, float width=0, float height=0)
        {
            var windowObj = Instantiate(CodeEditorPrefab, transform);
            var windowHandler = windowObj.GetComponent<EditorWindowHandler>();
            windowHandler.Init(windowName, text, x, y, width, height);
            windowHandlers.Add(windowHandler);
            ProcessedWindowNames.Add(windowHandler.GetWindowName().ToUpperInvariant());
        }
        public void RemoveEditorWindow(string windowName)
        {
            windowName = windowName.ToUpperInvariant();
            ProcessedWindowNames.Remove(windowName);
            EditorWindowHandler handler = null;
            windowHandlers.RemoveAll(w => {
                if (w.GetWindowName().ToUpperInvariant() != windowName) return false;
                handler = w; return true;
            });
            
            if (handler != null) Destroy(handler.gameObject);
            // TODO:: notify code service to remove this script
        }
        public void LockAllWindowsInput() { foreach (var editorHandler in windowHandlers) editorHandler.GetTextEditor().DisableInput = true; }
        public void UnlockAllWindowsInput() { foreach (var editorHandler in windowHandlers) editorHandler.GetTextEditor().DisableInput = false; }
    }
}
using System;
using System.Collections.Generic;
using CodeExecutor;
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

        [CanBeNull] 
        public EditorWindowHandler FindWindow(string windowName) => windowHandlers.Find(w => w.GetWindowName().Equals(windowName, StringComparison.InvariantCultureIgnoreCase));
        public IReadOnlyList<EditorWindowHandler> GetAllWindows() => windowHandlers;
        public void CreateEditorWindow(string windowName=null, string text=null, float x=0, float y=0, float width=0, float height=0)
        {
            var windowObj = Instantiate(CodeEditorPrefab, transform);
            var windowHandler = windowObj.GetComponent<EditorWindowHandler>();
            windowHandler.Init(windowName, text, x, y, width, height);
            windowHandlers.Add(windowHandler);
        }
        public void RemoveEditorWindow(string windowName)
        {
            EditorWindowHandler handler = null;
            windowHandlers.RemoveAll(w =>
            {
                if (!w.GetWindowName().Equals(windowName, StringComparison.InvariantCultureIgnoreCase)) return false;
                handler = w; return true;
            });

            if (handler == null) return;
            CodeService.Instance.RemoveExistScript(handler);
            Destroy(handler.gameObject);
        }
        public void LockAllWindowsInput() { foreach (var editorHandler in windowHandlers) editorHandler.GetTextEditor().DisableInput = true; }
        public void UnlockAllWindowsInput() { foreach (var editorHandler in windowHandlers) editorHandler.GetTextEditor().DisableInput = false; }
        
        public void LockAllWindowsFocus() { foreach (var editorHandler in windowHandlers) editorHandler.GetTextEditor().DisableFocus = true; }
        public void UnlockAllWindowsFocus() { foreach (var editorHandler in windowHandlers) editorHandler.GetTextEditor().DisableFocus = false; }
    }
}
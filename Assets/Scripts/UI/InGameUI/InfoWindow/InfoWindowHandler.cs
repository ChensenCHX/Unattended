using System;
using MarkdownToUnity.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using Utils;

namespace UI.InGameUI.InfoWindow
{
    public class InfoWindowHandler : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private MarkdownRenderer markdownRenderer;
        [SerializeField] private LocalizeStringEvent localizeEvent;
        
        public string CurrentChapter => chapterStack.Peek();
        private readonly HistoryStack<string> chapterStack = new("main.zip");
        private string cachedChapterName = null;

        private void OnChapterChangeByClick(string chapterName) => chapterStack.Push(chapterName);
        public void BackToLastChapter() => markdownRenderer.ShowChapter(chapterStack.Pop(), false);
        
        public string GetCurrentChapter() => chapterStack.Peek();
        public void ForceSetCurrentChapter(string chapterName)
        {
            if (gameObject.activeInHierarchy)
            {
                markdownRenderer.ShowChapterInMarkBook(chapterName, markdownRenderer.markzip);
                cachedChapterName = null;
            }
            else 
                cachedChapterName = chapterName;
        }
        public void SetLanguageMarker(string langMark) => markdownRenderer.SetLanguageMarker(langMark);
        
        private void Start() 
        {
            markdownRenderer.OnChapterChange += OnChapterChangeByClick;
        }
        private void OnEnable()
        {
            localizeEvent.RefreshString();
            if (cachedChapterName != null) ForceSetCurrentChapter(cachedChapterName);
        }
        
        public void Init(string chapterName)
        {
            chapterStack.Push(chapterName);
            ForceSetCurrentChapter(chapterName);
        }

        public InfoWindowSaveData SaveWindow()
        {
            return new InfoWindowSaveData
            {
                CurrentChapter = chapterStack.Peek(),
                X = rectTransform.anchoredPosition.x,
                Y = rectTransform.anchoredPosition.y,
                Width = rectTransform.sizeDelta.x,
                Height = rectTransform.sizeDelta.y
            };
        }
        private void OnDestroy() => markdownRenderer.OnChapterChange -= OnChapterChangeByClick;
    }
    
    public struct InfoWindowSaveData
    {
        public string CurrentChapter;
        public float X;
        public float Y;
        public float Width;
        public float Height;
    }
}
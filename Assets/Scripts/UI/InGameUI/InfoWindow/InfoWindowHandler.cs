using MarkdownToUnity.Runtime;
using UnityEngine;
using Utils;

namespace UI.InGameUI.InfoWindow
{
    public class InfoWindowHandler : MonoBehaviour
    {
        [SerializeField] private MarkdownRenderer markdownRenderer;
        
        public string CurrentChapter => chapterStack.Peek();
        private readonly HistoryStack<string> chapterStack = new("main.zip");

        private void OnChapterChangeByClick(string chapterName) => chapterStack.Push(chapterName);
        public void BackToLastChapter() => markdownRenderer.ShowChapter(chapterStack.Pop(), false);
        
        public string GetCurrentChapter() => chapterStack.Peek();
        public void ForceSetCurrentChapter(string chapterName) => markdownRenderer.ShowChapterInMarkBook(chapterName, markdownRenderer.markzip);

        private void Start() 
        {
            ForceSetCurrentChapter(chapterStack.Pop());
            markdownRenderer.OnChapterChange += OnChapterChangeByClick;
        }
        private void OnDestroy() => markdownRenderer.OnChapterChange -= OnChapterChangeByClick;
    }
}
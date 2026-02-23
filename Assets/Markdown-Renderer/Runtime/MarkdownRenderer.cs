using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MarkdownToUnity.Runtime {
    public class MarkdownRenderer : MonoBehaviour {

        public TextAsset markdownAsset;                 // Reference to the Markdownfile file in Unity
        public TextAsset markzip;                       // Reference to own Markzip structure

        public RectTransform content;                   // Container for displaying images
        public TMP_Text textPrefab;
        public GameObject horizontalLinePrefab;
        public GameObject blockquotePrefab;
        public GameObject codeblockPrefab;
        public GameObject spinner;

        [SerializeField] private float visualElementScaleFactor = 1f;
        [SerializeField] private Color defaultTextColor = Color.black;
        [SerializeField] private Color defaultLinkColor = Color.blue;
        [SerializeField] private float defaultTextSize = 15f;
        [SerializeField] private TMP_FontAsset defaultFont = default;
        [SerializeField] private float tableGridWidth = .5f;
        [SerializeField] private Color tableGridColor = Color.black;
        [SerializeField] private int tableInnerCellPadding = 2;

        [SerializeField] private bool useLanguageMarker = false;
        [SerializeField] private string languageMarker = "";

        public void SetDefaultTextColor(Color defaultTextColor)
        {
            this.defaultTextColor = defaultTextColor;
        }

        public void SetDefaultLinkColor(Color defaultLinkColor)
        {
            this.defaultLinkColor = defaultLinkColor;
        }

        public void SetDefaultTextSize(float defaultTextSize)
        {
            this.defaultTextSize = defaultTextSize;
        }

        public void SetDefaultFont(TMP_FontAsset font)
        {
            defaultFont = font;
        }

        public void SetTableGridWidth(float width)
        {
            tableGridWidth = width;
        }

        public void SetTableGridColor(Color color)
        {
            tableGridColor = color;
        }

        public void SetTableInnerCellPadding(int padding)
        {
            tableInnerCellPadding = padding;
        }

        public void SetUseLanguageMarker(bool useMarker)
        {
            useLanguageMarker = useMarker;
        }

        public void SetLanguageMarker( string marker ) {
            this.useLanguageMarker = marker != null;
            this.languageMarker = marker;
        }

        void Start()
        {
            Render();
        }

        public void Render( TextAsset obj ) {

            if(obj == null)
                return;

            markdownAsset = null; markzip = null;
            
            if (!obj.name.Contains("bin")) {
                markdownAsset = obj;
            } else {
                markzip = obj;
            }

            Render();
        }

        public void Render() {

            ApplyStyles();

            if(markdownAsset != null) {
                OpenMarkdownAsset();
            } else if(markzip != null) {
                OpenMarkdownZip();
            }
        }

        private void ApplyStyles()
        {
            MarkdownUnityRenderer.visualElementScaleFactor = visualElementScaleFactor;
            MarkdownUnityRenderer.defaultTextColor = defaultTextColor;
            MarkdownUnityRenderer.defaultLinkColor = defaultLinkColor;
            MarkdownUnityRenderer.defaultTextSize = defaultTextSize;
            MarkdownUnityRenderer.defaultFont = defaultFont;
            MarkdownUnityRenderer.tableGridWidth = tableGridWidth;
            MarkdownUnityRenderer.tableGridColor = tableGridColor;
            MarkdownUnityRenderer.tableInnerCellPadding = tableInnerCellPadding;
        }

        public void OpenMarkdownAsset() {

            if(markdownAsset == null) {
                Debug.LogWarning("No Markdownfile to render");
            }

            #if UNITY_EDITOR
                if (EditorApplication.isPlaying) {
                    
                    UnityUIMarkdownTextAsset asset = MarkdownUnityParser.ParseMarkdown(markdownAsset.text);
                    if (asset != null) {
                        MarkdownUnityRenderer.Render(asset, content, textPrefab.gameObject, horizontalLinePrefab, blockquotePrefab, codeblockPrefab);
                    }

                } else
                {
                    Debug.Log("Rendering is only Supported while Playing");
                }
            #else
                UnityUIMarkdownTextAsset asset = MarkdownUnityParser.ParseMarkdown(markdownAsset.text);
                if (asset != null) {
                    MarkdownUnityRenderer.Render(asset, content, textPrefab.gameObject, horizontalLinePrefab, blockquotePrefab, codeblockPrefab);
                }
            #endif

        }

        public void OpenMarkdownZip() {
            if(markzip == null) {
                Debug.LogWarning("No MarkdownZip to render");
            }

            
            #if UNITY_EDITOR
                if (EditorApplication.isPlaying) {
                    StartCoroutine(OpenMarkdownZipCoroutine(markzip, useLanguageMarker ? languageMarker : null));
                } else
                {
                    Debug.Log("Rendering is only Supported while Playing");
                }
            #else
                StartCoroutine(OpenMarkdownZipCoroutine(markzip, useLanguageMarker ? languageMarker : null));
            #endif

        }

        private IEnumerator OpenMarkdownZipCoroutine(TextAsset markZip, string languageMarker) {
            
            if(spinner != null) { spinner.SetActive(true); }

            Task<UnityUIMarkdownTextAsset> parseTask = MarkdownUnityParser.ParseMarkdown(markZip, languageMarker);
            
            yield return new WaitUntil(() => parseTask.IsCompleted);

            if (parseTask.IsFaulted) {
                Debug.LogError($"Exception: {parseTask.Exception?.InnerException?.Message}");
            } else {
                UnityUIMarkdownTextAsset asset = parseTask.Result;
                if (asset != null) {
                    MarkdownUnityRenderer.Render(asset, content, textPrefab.gameObject, horizontalLinePrefab, blockquotePrefab, codeblockPrefab);
                }
            }

            if (spinner != null) { spinner.SetActive(false); }
        }

        public void ShowChapterInMarkBook( string chaptername, TextAsset markBook ) {
            MarkdownUnityParser.OpenMarkbook( markBook );
            ShowChapter( chaptername );
        }

        public void ShowChapter( string chaptername ) {
            StartCoroutine(ShowChapterCoroutine(chaptername));
        }

        private IEnumerator ShowChapterCoroutine(string chapterName)
        {

            if (spinner != null) { spinner.SetActive(true); }

            var parseTask = MarkdownUnityParser.OpenMarkbookChapter(chapterName, languageMarker);

            yield return new WaitUntil(() => parseTask.IsCompleted);

            if (parseTask.IsFaulted)
            {
                Debug.LogError($"Exception: {parseTask.Exception?.InnerException?.Message}");
            }
            else
            {
                UnityUIMarkdownTextAsset asset = parseTask.Result;
                if (asset != null)
                {
                    MarkdownUnityRenderer.Render(asset, content, textPrefab.gameObject, horizontalLinePrefab, blockquotePrefab, codeblockPrefab);
                }
            }

            if (spinner != null) { spinner.SetActive(false); }
        }
    }
}

using UnityEngine;
using UnityEditor;
using MarkdownToUnity.Runtime;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;

namespace MarkdownToUnity.Editor
{
    [CustomEditor(typeof(MarkdownRenderer))]
    public class MarkdownRendererEditor : UnityEditor.Editor
    {

        public VisualElement root;
        private VisualElement Content => root.Query<VisualElement>("content");
        private MarkdownRenderer markdownRenderer => (MarkdownRenderer)target;

        private int selectedChapterIndex = 0; 
        private string[] chapterNames = new string[0]; 

        private void OnEnable()
        {
            serializedObject.Update();

            UIFactory.LoadUnityIcon(target, "markdownRenderer_Icon");
        }
        
        public override VisualElement CreateInspectorGUI()
        {
            root = new VisualElement()
            {
                style =
                {
                    flexGrow = 1,
                    marginLeft = -15
                }
            };

            VisualTreeAsset asset = UIFactory.LoadInspector("markdown-renderer-inspector");
            if (asset != null)
                asset.CloneTree(root);

            var container = new VisualElement
            {
                style = { flexDirection = FlexDirection.Column, marginTop = 10 }
            };

            // --- INPUT SECTION ---
            var (inputOuter, inputFoldout, _) = UIFactory.CreateFoldout("Input", "The Markdown file and the Markzip reference were rendered.", new Color(0.4f, 0.6f, 1f), true);
            inputFoldout.Add(UIFactory.CreateFieldWithInfo(serializedObject.FindProperty("markdownAsset"),
                "Markdown Asset", "The main markdown file that will be rendered.", new Color(0.4f, 0.6f, 1f)));
            inputFoldout.Add(UIFactory.CreateFieldWithInfo(serializedObject.FindProperty("markzip"),
                "Markzip Asset", "Use Markzip for more files and multilingual support.", new Color(0.4f, 0.6f, 1f)));
            container.Add(inputOuter);

            // --- ELEMENTS SECTION ---
            var (elemOuter, elemFoldout, _) = UIFactory.CreateFoldout("Elements", "Assigned UI references used for rendering.", new Color(0.6f, 0.4f, 1f));
            elemFoldout.Add(UIFactory.CreateFieldWithInfo(serializedObject.FindProperty("content"), "Content", "Container that holds generated UI content.", new Color(0.6f, 0.4f, 1f)));
            elemFoldout.Add(UIFactory.CreateFieldWithInfo(serializedObject.FindProperty("textPrefab"), "Text Prefab", "TMP prefab used for rendering text blocks.", new Color(0.6f, 0.4f, 1f)));
            elemFoldout.Add(UIFactory.CreateFieldWithInfo(serializedObject.FindProperty("horizontalLinePrefab"), "Horizontal Line", "Prefab used for horizontal rules (---).", new Color(0.6f, 0.4f, 1f)));
            elemFoldout.Add(UIFactory.CreateFieldWithInfo(serializedObject.FindProperty("blockquotePrefab"), "Blockquote", "Prefab used for quote blocks (>)", new Color(0.6f, 0.4f, 1f)));
            elemFoldout.Add(UIFactory.CreateFieldWithInfo(serializedObject.FindProperty("codeblockPrefab"), "Codeblock", "Prefab used for code blocks (>)", new Color(0.6f, 0.4f, 1f)));
            elemFoldout.Add(UIFactory.CreateFieldWithInfo(serializedObject.FindProperty("spinner"), "Spinner", "Loading animation shown during processing.", new Color(0.6f, 0.4f, 1f)));
            container.Add(elemOuter);

            // --- STYLES SECTION ---
            var (styleOuter, styleFoldout, _) = UIFactory.CreateFoldout("Styles", "Visual appearance and layout settings.", new Color(1f, 0.5f, 0.2f));
            styleFoldout.Add(UIFactory.CreateFieldWithInfo(serializedObject.FindProperty("visualElementScaleFactor"), "Scale Factor", "Global scaling factor for all visual elements.", new Color(1f, 0.5f, 0.2f)));
            styleFoldout.Add(UIFactory.CreateFieldWithInfo(serializedObject.FindProperty("defaultTextColor"), "Default Text Color", "Default color used for normal text.", new Color(1f, 0.5f, 0.2f)));
            styleFoldout.Add(UIFactory.CreateFieldWithInfo(serializedObject.FindProperty("defaultLinkColor"), "Default Link Color", "Color used for clickable links.", new Color(1f, 0.5f, 0.2f)));
            styleFoldout.Add(UIFactory.CreateFieldWithInfo(serializedObject.FindProperty("defaultTextSize"), "Default Text Size", "Base text size for rendered markdown.", new Color(1f, 0.5f, 0.2f)));
            styleFoldout.Add(UIFactory.CreateFieldWithInfo(serializedObject.FindProperty("defaultFont"), "Default Font", "Font asset used as default for markdown text.", new Color(1f, 0.5f, 0.2f)));
            styleFoldout.Add(UIFactory.CreateFieldWithInfo(serializedObject.FindProperty("tableGridWidth"), "Table Grid Width", "Line width for markdown tables.", new Color(1f, 0.5f, 0.2f)));
            styleFoldout.Add(UIFactory.CreateFieldWithInfo(serializedObject.FindProperty("tableGridColor"), "Table Grid Color", "Color of table borders.", new Color(1f, 0.5f, 0.2f)));
            styleFoldout.Add(UIFactory.CreateFieldWithInfo(serializedObject.FindProperty("tableInnerCellPadding"), "Cell Padding", "Inner padding for table cells.", new Color(1f, 0.5f, 0.2f)));
            container.Add(styleOuter);

            // --- LANGUAGE SECTION ---
            var (langOuter, langFoldout, _) = UIFactory.CreateFoldout("Language", "Language-based parsing options.", new Color(0.3f, 0.8f, 0.3f));
            langFoldout.Add(UIFactory.CreateFieldWithInfo(serializedObject.FindProperty("useLanguageMarker"), "Use Language Marker", "If enabled, markdown is filtered by language tags.", new Color(0.3f, 0.8f, 0.3f)));
            langFoldout.Add(UIFactory.CreateFieldWithInfo(serializedObject.FindProperty("languageMarker"), "Language Marker", "Language key to use (e.g. 'en' or 'de').", new Color(0.3f, 0.8f, 0.3f)));
            container.Add(langOuter);

            // --- Chapter Navigation ---
            if (MarkdownUnityParser.HasLoadedChapters())
            {
                var chapterDict = MarkdownUnityParser.GetChapterDictionary();

                if (chapterDict != null && chapterDict.Count > 0)
                {
                    chapterNames = new List<string>(chapterDict.Keys).ToArray();

                    langFoldout.Add(UIFactory.CreateLabel("Chapter Navigation", bold: true));

                    langFoldout.Add(UIFactory.CreatePopup("Select Chapter", new List<string>(chapterNames), 0, (value, index) => selectedChapterIndex = index));

                    var showButton = UIFactory.CreateButton("Show Chapter", new Color(0.25f, 0.6f, 0.3f), () =>
                    {
                        string selected = chapterNames[selectedChapterIndex];
                        markdownRenderer.ShowChapter(selected);
                    });

                    langFoldout.Add(showButton);
                }
            }

            // --- Render ---
            var renderButton = UIFactory.CreateButton("Render Markdown", new Color(0.2f, 0.4f, 0.8f), () =>
                {
                    markdownRenderer.Render();
                }
            );
            renderButton.style.marginLeft = 25f;
            renderButton.style.marginRight = 25f;
            renderButton.style.marginBottom = 10;
            container.Add(renderButton);
            
            if (Content != null)
                Content.Add(container);
            else
                root.Add(container);

            root.Bind(serializedObject);
            return root;
        }
    
    }
}

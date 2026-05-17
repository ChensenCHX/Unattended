using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

namespace MarkdownToUnity.Runtime {
    public static class MarkdownUnityRenderer
    {

        // Styles
        public static float visualElementScaleFactor = 1f;
        public static Color defaultTextColor = Color.black;
        public static Color defaultLinkColor = Color.blue;
        public static float defaultTextSize = 15f;
        public static TMP_FontAsset defaultFont = default;
        public static float tableGridWidth = .5f;
        public static Color tableGridColor = Color.black;
        public static int tableInnerCellPadding = 2;
        //

        private static GameObject textPrefab;
        private static GameObject horizontalLineBreakPrefab;
        private static GameObject blockquotePrefab;
        private static GameObject codeBlockPrefab;

        private static UnityUIMarkdownTextAsset asset;

        private static void ClearContent(RectTransform parent) {
            RawImage[] images = parent.GetComponentsInChildren<RawImage>();
            foreach (var image in images)
                GameObject.Destroy(image.texture); // To avoid Memory leaks

            foreach (Transform child in parent.transform)
                GameObject.Destroy(child.gameObject);
        }

        public static void Render(UnityUIMarkdownTextAsset asset, RectTransform parent, GameObject textPrefab, GameObject horizontalLineBreakPrefab, GameObject blockquotePrefab, GameObject codeBlockPrefab) {
            
            if(parent == null) {
                Debug.LogError("NO Parent Contianer provided");
                return;
            }

            ClearContent(parent);
            
            MarkdownUnityRenderer.textPrefab = textPrefab;
            MarkdownUnityRenderer.horizontalLineBreakPrefab = horizontalLineBreakPrefab;
            MarkdownUnityRenderer.blockquotePrefab = blockquotePrefab;
            MarkdownUnityRenderer.codeBlockPrefab = codeBlockPrefab;

            MarkdownUnityRenderer.asset = asset;
            
            // Add Requeired Componenets
            VerticalLayoutGroup verticalLayoutGroup;
            if(!parent.TryGetComponent(out verticalLayoutGroup)) {
                verticalLayoutGroup = parent.gameObject.AddComponent<VerticalLayoutGroup>();
            }
            verticalLayoutGroup.childScaleWidth = true;
            verticalLayoutGroup.childScaleHeight = true;
            verticalLayoutGroup.childForceExpandWidth = false;
            verticalLayoutGroup.childForceExpandHeight = false;

            ContentSizeFitter contentSizeFitter;
            if(!parent.TryGetComponent(out contentSizeFitter)) {
                contentSizeFitter = parent.gameObject.AddComponent<ContentSizeFitter>();
            }
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            //

            foreach (var block in asset.blocks) {
                RenderContent(block, parent);
            }
        }

        private static void RenderContent(UnityUIMarkdownTextAsset.Block block, Transform parent)
        {
            GameObject uiElement = null;

            switch (block)
            {
                case UnityUIMarkdownTextAsset.TextBlock textBlock:
                    uiElement = CreateTextBlock(textBlock, parent);
                    break;
                case UnityUIMarkdownTextAsset.ImageBlock imageBlock:
                    uiElement = CreateImageElement(imageBlock, parent);
                    break;
                case UnityUIMarkdownTextAsset.VideoBlock videoBlock:
                    uiElement = CreateVideoElement(videoBlock, parent);
                    break;
                case UnityUIMarkdownTextAsset.TableBlock tableBlock:
                    uiElement = CreateTableElement(tableBlock, parent);
                    break;
                case UnityUIMarkdownTextAsset.HorizontalLine:
                    uiElement = CreateHorizontalLine(parent);
                    break;
                case UnityUIMarkdownTextAsset.LinkBlock linkBlock:
                    uiElement = CreateLinkElement(linkBlock, parent);
                    break;
                case UnityUIMarkdownTextAsset.Blockquote blockquote:
                    uiElement = CreateBlockquoteElement(blockquote, parent);
                    break;
                case UnityUIMarkdownTextAsset.CodeBlock codeBlock:
                    uiElement = CreateCodeBlockElement(codeBlock, parent);
                    break;
                default:
                    Debug.LogWarning($"Unrecognized Block element type: {block.GetType().Name}");
                    break;
            }
        }

        private static GameObject CreateHorizontalLine(Transform parent)
        {
            var obj = GameObject.Instantiate(horizontalLineBreakPrefab, parent);
            obj.GetComponentInChildren<Image>().color = MarkdownUnityRenderer.defaultTextColor;
            obj.SetActive( true );
            return obj;
        }

        private static GameObject CreateTableElement(UnityUIMarkdownTextAsset.TableBlock tableBlock, Transform parent)
        {
            GameObject tableObject = new GameObject("TableElement", typeof(RectTransform));
            tableObject.transform.SetParent(parent);
            tableObject.transform.localScale = Vector3.one;

            VerticalLayoutGroup tableLayout = tableObject.AddComponent<VerticalLayoutGroup>();
            tableLayout.childControlWidth = true;
            tableLayout.childControlHeight = true;
            tableLayout.childForceExpandWidth = true;
            tableLayout.childForceExpandHeight = false;

            UIOutlineRect uIOutlineRect = tableObject.AddComponent<UIOutlineRect>();
            uIOutlineRect.thickness = tableGridWidth * 2f;
            uIOutlineRect.color = tableGridColor;

            UITableCellSpacer tableCellSpacer = tableObject.AddComponent<UITableCellSpacer>();
            List<List<LayoutElement>> tableElements = new List<List<LayoutElement>>();

            foreach (var row in tableBlock.Rows)
            {
                List<LayoutElement> rowElements = new List<LayoutElement>();

                GameObject rowObject = new GameObject("TableRow", typeof(RectTransform));
                rowObject.transform.SetParent(tableObject.transform);
                rowObject.transform.localScale = Vector3.one;

                HorizontalLayoutGroup rowLayout = rowObject.AddComponent<HorizontalLayoutGroup>();
                rowLayout.childControlWidth = true;
                rowLayout.childControlHeight = true;
                rowLayout.childForceExpandWidth = true;
                rowLayout.childForceExpandHeight = false;

                foreach (var cell in row.Cells)
                {

                    GameObject cellObject = new GameObject("TableCell", typeof(RectTransform));
                    cellObject.transform.SetParent(rowObject.transform);
                    cellObject.transform.localScale = Vector3.one;

                    VerticalLayoutGroup cellLayout = cellObject.AddComponent<VerticalLayoutGroup>();
                    cellLayout.childControlWidth = true;
                    cellLayout.childControlHeight = true;
                    cellLayout.childForceExpandWidth = false;
                    cellLayout.childForceExpandHeight = true;
                    cellLayout.padding = new RectOffset(tableInnerCellPadding, tableInnerCellPadding, tableInnerCellPadding, tableInnerCellPadding);

                    UIOutlineRect uIOutlineCellRect = cellObject.AddComponent<UIOutlineRect>();
                    uIOutlineCellRect.thickness = tableGridWidth;
                    uIOutlineCellRect.color = tableGridColor;

                    LayoutElement layoutElement = cellObject.AddComponent<LayoutElement>();
                    rowElements.Add(layoutElement);

                    foreach (var cellBlock in cell.blocks)
                    {
                        switch (cellBlock)
                        {
                            case UnityUIMarkdownTextAsset.TextBlock textBlock:
                                CreateTextBlock(textBlock, cellObject.transform);
                                break;
                            case UnityUIMarkdownTextAsset.ImageBlock imageBlock:
                                CreateImageElement(imageBlock, cellObject.transform);
                                break;
                            case UnityUIMarkdownTextAsset.VideoBlock videoBlock:
                                CreateVideoElement(videoBlock, cellObject.transform);
                                break;
                            case UnityUIMarkdownTextAsset.HorizontalLine:
                                CreateHorizontalLine(cellObject.transform);
                                break;
                            case UnityUIMarkdownTextAsset.Blockquote blockquote:
                                CreateBlockquoteElement(blockquote, cellObject.transform);
                                break;
                             case UnityUIMarkdownTextAsset.CodeBlock codeBlock:
                                CreateCodeBlockElement(codeBlock, cellObject.transform);
                                break;
                            default:
                                Debug.LogWarning($"Unrecognized Block element type in cell: {cellBlock.GetType().Name}");
                                break;
                        }
                    }
                }

                tableElements.Add(rowElements);
                 
            }

            tableCellSpacer.tableCellElements = tableElements;
            tableCellSpacer.Calculate();

            return tableObject;
        }
        
        private static GameObject CreateTextBlock(UnityUIMarkdownTextAsset.TextBlock textElement, Transform parent)
        {
            var textComponent = GameObject.Instantiate(textPrefab, parent).GetComponent<TMP_Text>();

            textComponent.text = textElement.RichText;
            SetDefaultTextComponentStyles(textComponent);
            textComponent.gameObject.SetActive(true);

            return textComponent.gameObject;
        }

        private static GameObject CreateImageElement(UnityUIMarkdownTextAsset.ImageBlock imageBlock, Transform parent)
        {
            var imageObject = new GameObject("ImageElement", typeof(RectTransform));
            imageObject.transform.SetParent(parent);
            imageObject.transform.localScale = Vector3.one;

            var rawImage = imageObject.AddComponent<RawImage>();
            var aspectRatioFitter = imageObject.AddComponent<AspectRatioFitter>();

            var layoutElement = imageObject.AddComponent<LayoutElement>();

            aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;

            RectTransform rectTransform = imageObject.GetComponent<RectTransform>();
            rectTransform.pivot = new Vector2(0, 1);
        
            if (!string.IsNullOrEmpty(imageBlock.Key) && asset.internData.ContainsKey(imageBlock.Key))
            {
                var texture = LoadTextureFromBase64(asset.internData[imageBlock.Key]);
                rawImage.texture = texture;
            }
            else if (!string.IsNullOrEmpty(imageBlock.Url) && asset.externData.ContainsKey(imageBlock.Url))
            {
                var path = asset.externData[imageBlock.Url];
                
                if (File.Exists(path))
                {
                    byte[] imageBytes = File.ReadAllBytes( path );
                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(imageBytes);
                    rawImage.texture = texture;
                }
            } else if (!string.IsNullOrEmpty(imageBlock.Url) 
                && Uri.IsWellFormedUriString(imageBlock.Url, UriKind.Absolute) 
                && (imageBlock.Url.StartsWith("http://") || imageBlock.Url.StartsWith("https://"))) {
                rawImage.StartCoroutine(LoadImageFromUrl(imageBlock, rawImage, rectTransform, layoutElement, aspectRatioFitter));
                return imageObject;
            }

            ApplyImageDimensions(imageBlock, rawImage, layoutElement, aspectRatioFitter, rectTransform);

            return imageObject;
        }

        private static void ApplyImageDimensions(UnityUIMarkdownTextAsset.ImageBlock imageBlock, RawImage rawImage, LayoutElement layoutElement, AspectRatioFitter aspectRatioFitter, RectTransform rectTransform) {
            if(rawImage.texture != null) {
                layoutElement.preferredWidth = rawImage.texture.width * MarkdownUnityRenderer.visualElementScaleFactor;
                layoutElement.preferredHeight = rawImage.texture.height * MarkdownUnityRenderer.visualElementScaleFactor;
                aspectRatioFitter.aspectRatio = layoutElement.preferredWidth / layoutElement.preferredHeight;
                aspectRatioFitter.StartCoroutine(AdjustHeightNextFrame(rectTransform, layoutElement));
            } else {
                Debug.LogWarning("MarkdownToUnityUI: Image could not be loaded at: Key:" + imageBlock.Key + " URL: " + imageBlock.Url);
            }

            rectTransform.localScale = Vector3.one;
        }

        private static Texture2D LoadTextureFromBase64(string base64ImageData)
        {
            
            if (base64ImageData.StartsWith("data:image"))
            {
                int commaIndex = base64ImageData.IndexOf(',');
                base64ImageData = base64ImageData.Substring(commaIndex + 1);
            }

            byte[] imageBytes = System.Convert.FromBase64String(base64ImageData);

            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);

            return texture;
        }

        private static IEnumerator LoadImageFromUrl(UnityUIMarkdownTextAsset.ImageBlock imageBlock, RawImage rawImage, RectTransform rectTransform, LayoutElement layoutElement, AspectRatioFitter aspectRatioFitter)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageBlock.Url);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogWarning(request.error);
            }
            else
            {
                var texture = DownloadHandlerTexture.GetContent(request);
                rawImage.texture = texture;

                layoutElement.preferredWidth = rawImage.texture.width * MarkdownUnityRenderer.visualElementScaleFactor;
                layoutElement.preferredHeight = rawImage.texture.height * MarkdownUnityRenderer.visualElementScaleFactor;
                aspectRatioFitter.aspectRatio = layoutElement.preferredWidth / layoutElement.preferredHeight;
                rawImage.StartCoroutine(AdjustHeightNextFrame(rectTransform, layoutElement));
            }

            ApplyImageDimensions(imageBlock, rawImage, layoutElement, aspectRatioFitter, rectTransform);
        }


        private static GameObject CreateVideoElement(UnityUIMarkdownTextAsset.VideoBlock videoBlock, Transform parent) {
            var videoObject = new GameObject("VideoElement", typeof(RectTransform));
            videoObject.transform.SetParent(parent);
            videoObject.transform.localScale = Vector3.one;

            var videoPlayer = videoObject.AddComponent<VideoPlayer>();
            var rawImage = videoObject.AddComponent<RawImage>();
            var layoutElement = videoObject.AddComponent<LayoutElement>();
            var aspectRatioFitter = videoObject.AddComponent<AspectRatioFitter>();

            aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;

            RectTransform rectTransform = videoObject.GetComponent<RectTransform>();
            rectTransform.pivot = new Vector2(0, 1);

            if (!string.IsNullOrEmpty(videoBlock.Url) && asset.externData.ContainsKey(videoBlock.Url))
            {
                videoPlayer.source = VideoSource.Url;
                videoPlayer.url = asset.externData[videoBlock.Url];

                var renderTexture = new RenderTexture(1920, 1080, 0); 
                videoPlayer.targetTexture = renderTexture;
                rawImage.texture = renderTexture;

                layoutElement.preferredWidth = renderTexture.width * MarkdownUnityRenderer.visualElementScaleFactor;
                layoutElement.preferredHeight = renderTexture.height * MarkdownUnityRenderer.visualElementScaleFactor;

                videoPlayer.prepareCompleted += (source) =>
                {
                    if (videoPlayer.texture != null)
                    {
                        int videoWidth = videoPlayer.texture.width;
                        int videoHeight = videoPlayer.texture.height;

                        aspectRatioFitter.aspectRatio = (float)videoWidth / videoHeight;

                        renderTexture.Release();
                        renderTexture = new RenderTexture(videoWidth, videoHeight, 0);
                        videoPlayer.targetTexture = renderTexture;
                        rawImage.texture = renderTexture;

                        layoutElement.preferredWidth = videoWidth * MarkdownUnityRenderer.visualElementScaleFactor;
                        layoutElement.preferredHeight = videoHeight * MarkdownUnityRenderer.visualElementScaleFactor;

                        rectTransform.localScale = Vector3.one;

                        aspectRatioFitter.StartCoroutine(AdjustHeightNextFrame(rectTransform, layoutElement));

                        videoPlayer.isLooping = true;
                        videoPlayer.Play();
                    }
                };

                videoPlayer.Prepare();
            }

            return videoObject;
        }


        private static GameObject CreateLinkElement(UnityUIMarkdownTextAsset.LinkBlock linkBlock, Transform parent)
        {
            var textComponent = GameObject.Instantiate(textPrefab, parent).GetComponent<TMP_Text>();
            textComponent.transform.name = "LinkElement";

            SetDefaultTextComponentStyles(textComponent);

            textComponent.transform.localPosition = Vector3.zero;
            textComponent.text = linkBlock.Text;
            textComponent.color = MarkdownUnityRenderer.defaultLinkColor; // Link color
            textComponent.fontStyle = FontStyles.Underline; // Underline for link
            textComponent.alignment = TextAlignmentOptions.Left; // Adjust alignment as needed
            textComponent.raycastTarget = false;

            // Add a Button component to handle click events
            var pressObject = new GameObject("Button", typeof(RectTransform));
            pressObject.transform.SetParent(textComponent.transform);
            pressObject.transform.localScale = Vector3.one;
            pressObject.transform.localPosition = Vector3.zero;

            var button = pressObject.AddComponent<Button>();
            button.onClick.AddListener(() =>
            {
                if (linkBlock.Uri.StartsWith("chapter:"))
                {
                    parent.parent.parent.parent.GetComponent<MarkdownRenderer>()
                        .ShowChapter(linkBlock.Uri["chapter:".Length..] + ".zip");
                }
                else
                {
                    Application.OpenURL(linkBlock.Uri);
                }
            });

            var image = pressObject.AddComponent<Image>();
            image.color = new Color(1, 1, 1, 0);
            button.targetGraphic = image;

            RectTransform rectTransform = pressObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0.5f);
            rectTransform.anchorMax = new Vector2(0, 0.5f);
            rectTransform.pivot = new Vector2(0, 0.5f);
            rectTransform.sizeDelta = new Vector2(textComponent.preferredWidth, textComponent.preferredHeight);

            var layoutElement = textComponent.gameObject.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 40; 

            textComponent.gameObject.SetActive(true);
            return textComponent.gameObject;
        }

        private static GameObject CreateBlockquoteElement(UnityUIMarkdownTextAsset.Blockquote blockquote, Transform parent)
        {
            var blockquoteObject = GameObject.Instantiate(MarkdownUnityRenderer.blockquotePrefab, parent);

            TMP_Text textComponent = blockquoteObject.GetComponent<TMP_Text>();
            if (textComponent != null)
            {
                textComponent.text = blockquote.RichText;
                SetDefaultTextComponentStyles(textComponent);
            }

            blockquoteObject.SetActive(true);

            return blockquoteObject;
        }
        
        private static GameObject CreateCodeBlockElement(UnityUIMarkdownTextAsset.CodeBlock codeBlock, Transform parent)
        {
            var codeBlockObject = GameObject.Instantiate(MarkdownUnityRenderer.codeBlockPrefab, parent);

            var highlightedCode = SyntaxHighlighter.Highlight(codeBlock.Code, codeBlock.Language);
            
            TMP_Text textComponent = codeBlockObject.GetComponent<TMP_Text>();
            if (textComponent != null)
            {
                textComponent.text = highlightedCode;
                SetDefaultTextComponentStyles(textComponent);
            }
            
            TMP_Text codeComponent = codeBlockObject.transform.GetChild(1).GetComponent<TMP_Text>();
            if (codeComponent != null)
            {
                codeComponent.text = highlightedCode;
                SetDefaultTextComponentStyles(codeComponent);
            }
            
            TMP_Text languageComponent = codeBlockObject.transform.GetChild(2).GetComponent<TMP_Text>();
            if(languageComponent != null) {
                languageComponent.text = codeBlock.Language;
                languageComponent.font = MarkdownUnityRenderer.defaultFont;
            }

            codeBlockObject.SetActive(true);

            return codeBlockObject;
        }

        private static void SetDefaultTextComponentStyles(TMP_Text textComponent)
        {
            textComponent.color = MarkdownUnityRenderer.defaultTextColor;
            textComponent.font = MarkdownUnityRenderer.defaultFont;
            textComponent.fontSize = MarkdownUnityRenderer.defaultTextSize;

#if UNITY_6000_0_OR_NEWER
            textComponent.textWrappingMode = TextWrappingModes.Normal;
#else
            textComponent.enableWordWrapping = true;
#endif
        }

        private static IEnumerator AdjustHeightNextFrame(RectTransform rectTransform, LayoutElement layoutElement) {
            yield return null;
            layoutElement.preferredHeight = rectTransform.rect.height;
        }

    }
}

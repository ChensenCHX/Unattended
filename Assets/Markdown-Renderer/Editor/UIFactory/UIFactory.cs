using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MarkdownToUnity.Editor
{
    public static class UIFactory
    {

        public static VisualTreeAsset LoadInspector(string name)
        {
            string[] guids = AssetDatabase.FindAssets($"{name} t:VisualTreeAsset");

            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
            }

            Debug.LogWarning($"No UXML with name '{name}' found – using fallback layout.");
            return null;
        }

        public static void LoadUnityIcon(UnityEngine.Object target, string name)
        {
            if (target == null) return;
            MonoScript monoScript = MonoScript.FromMonoBehaviour(target as MonoBehaviour);
            if (monoScript == null) return;

            if (AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(monoScript)) is not MonoImporter monoImporter) return;
            if (monoImporter.GetIcon() != null) return;

            string[] guids = AssetDatabase.FindAssets($"{name} t:Texture2D");
            if (guids.Length > 0)
            {
                
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

                if(icon != null)
                {
                    monoImporter.SetIcon(icon);
                    monoImporter.SaveAndReimport();
                }
            }
        }

        public static VisualElement CreateFieldWithInfo(SerializedProperty prop, string label, string infoText, Color borderLeftColor)
        {
            PropertyField field = new PropertyField(prop, label);

            var container = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Column,
                    marginBottom = 10,
                    paddingBottom = 4,
                    borderBottomWidth = 1,
                    borderBottomColor = new Color(0.3f, 0.3f, 0.3f),
                }
            };

            var fieldGroup = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Column,
                    paddingTop = 6,
                    paddingBottom = 6,
                    paddingLeft = 8,
                    paddingRight = 8,
                    borderLeftWidth = 4,
                    borderLeftColor = borderLeftColor,
                    backgroundColor = new Color(0.05f, 0.05f, 0.05f),
                    borderTopLeftRadius = 3,
                    borderBottomLeftRadius = 3
                }
            };

            fieldGroup.Add(field);

            if(infoText != null && infoText != "")
            {

                var infoBox = new VisualElement
                {
                    style =
                    {
                        marginTop = 4,
                        backgroundColor = new Color(0.1f, 0.1f, 0.1f),
                        borderTopWidth = 1,
                        borderTopColor = new Color(0.4f, 0.6f, 1f),
                        paddingTop = 4,
                        paddingBottom = 4,
                        paddingLeft = 6,
                        paddingRight = 6,
                    }
                };

                var infoLabel = new Label(infoText)
                {
                    style =
                    {
                        unityTextAlign = TextAnchor.UpperLeft,
                        color = new Color(0.8f, 0.8f, 0.8f),
                        fontSize = 11,
                        flexWrap = Wrap.Wrap,
                        whiteSpace = WhiteSpace.Normal
                    }
                };

                infoBox.Add(infoLabel);
                fieldGroup.Add(infoBox);
            }
            container.Add(fieldGroup);

            return container;
        }

        public static (VisualElement outer, Foldout foldout, VisualElement colorBar) CreateFoldout(string label, string infoText = null, Color? accentColor = null, bool defaultOpen = false)
        {
            Color accent = accentColor ?? new Color(0.4f, 0.8f, 0.4f);

            var outer = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Column,
                    marginBottom = 10,
                    borderBottomWidth = 1,
                    borderBottomColor = new Color(0.25f, 0.25f, 0.25f)
                }
            };

            var colorBar = new VisualElement
            {
                style =
                {
                    width = 5,
                    backgroundColor = accent,
                    borderTopLeftRadius = 3,
                    borderBottomLeftRadius = 3
                }
            };

            var content = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Column,
                    flexGrow = 1,
                    backgroundColor = new Color(0.05f, 0.05f, 0.05f),
                    paddingLeft = 15,
                    paddingRight = 10,
                    paddingTop = 6,
                    paddingBottom = 0,
                    borderTopRightRadius = 3,
                    borderBottomRightRadius = 3
                }
            };

            var row = new VisualElement { style = { flexDirection = FlexDirection.Row } };
            row.Add(colorBar);
            row.Add(content);
            outer.Add(row);

            var foldout = new Foldout
            {
                text = label,
                value = defaultOpen
            };
            foldout.style.unityFontStyleAndWeight = FontStyle.Bold;
            foldout.style.color = new StyleColor(new Color(0.85f, 0.85f, 0.85f));
            foldout.style.marginBottom = 4;
            content.Add(foldout);

            if (!string.IsNullOrEmpty(infoText))
            {
                var infoBox = new VisualElement
                {
                    style =
                    {
                        marginTop = 6,
                        backgroundColor = new Color(0.1f, 0.1f, 0.1f),
                        borderTopWidth = 1,
                        borderTopColor = accent,
                        paddingTop = 4,
                        paddingBottom = 4,
                        paddingLeft = 6,
                        paddingRight = 6
                    }
                };

                var infoLabel = new Label(infoText)
                {
                    style =
                    {
                        unityTextAlign = TextAnchor.UpperLeft,
                        color = new Color(0.8f, 0.8f, 0.8f),
                        fontSize = 11,
                        flexWrap = Wrap.Wrap,
                        whiteSpace = WhiteSpace.Normal
                    }
                };

                infoBox.Add(infoLabel);
                foldout.Add(infoBox);
            }


            return (outer, foldout, colorBar);
        }

        public static Button CreateButton(string label, Color color, Action onClick)
        {
            return new Button(onClick)
            {
                text = label,
                style = {
                    flexGrow = 1f,
                    height = 26,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    backgroundColor = color,
                    color = Color.white,
                    borderTopLeftRadius = 6,
                    borderTopRightRadius = 6,
                    borderBottomRightRadius = 6,
                    borderBottomLeftRadius = 6,
                    borderTopWidth = 1,
                    borderBottomWidth = 1,
                    borderLeftWidth = 1,
                    borderRightWidth = 1,
                    borderTopColor = new Color(1, 1, 1, 0.3f),
                    borderBottomColor = new Color(0, 0, 0, 0.5f),
                    borderLeftColor = new Color(1, 1, 1, 0.3f),
                    borderRightColor = new Color(0, 0, 0, 0.5f)
                }
            };
        }

        public static Slider CreateSlider(SerializedProperty prop, string label)
        {
            var slider = new Slider(0f, 1f)
            {
                value = prop.floatValue,
                label = label
            };

            slider.style.flexGrow = 1;
            slider.style.marginBottom = 4;
            slider.style.marginTop = 2;
            slider.style.marginLeft = 6;
            slider.style.marginRight = 6;
            slider.style.height = 20;
            slider.style.unityTextAlign = TextAnchor.MiddleLeft;
            slider.labelElement.style.minWidth = 80;
            slider.labelElement.style.color = new Color(0.85f, 0.85f, 0.85f);
            slider.labelElement.style.unityFontStyleAndWeight = FontStyle.Bold;

            var valueLabel = new Label($"{prop.floatValue:0.00}")
            {
                style =
                {
                    minWidth = 40,
                    unityTextAlign = TextAnchor.MiddleRight,
                    color = new Color(0.9f, 0.9f, 0.9f),
                    marginLeft = 4
                }
            };

            var container = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center,
                    justifyContent = Justify.SpaceBetween,
                    paddingLeft = 6,
                    paddingRight = 8,
                    paddingTop = 2,
                    paddingBottom = 2,
                    marginBottom = 4,
                }
            };

            container.Add(slider);
            container.Add(valueLabel);

            slider.RegisterValueChangedCallback(evt =>
            {
                prop.floatValue = evt.newValue;
                valueLabel.text = $"{evt.newValue:0.00}";
            });

            slider.Add(container);

            return slider;
        }

        public static (VisualElement, Slider) CreateSlider(string label, Func<float> getter, Action<float> setter, float min = 0, float max = 1)
        {
            var row = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center,
                    marginTop = 4,
                    marginBottom = 4,
                }
            };

            var slider = new Slider(label, min, max)
            {
                value = getter(),
                style =
                {
                    flexGrow = 1,
                    marginRight = 6
                }
            };

            var input = new FloatField
            {
                value = getter(),
                style =
                {
                    width = 50,
                    marginRight = 4
                }
            };

            slider.RegisterValueChangedCallback(evt =>
            {
                if (Math.Abs(input.value - evt.newValue) > 0.0001f)
                    input.SetValueWithoutNotify(evt.newValue);

                setter(evt.newValue);
            });

            input.RegisterValueChangedCallback(evt =>
            {
                float clamped = Mathf.Clamp(evt.newValue, slider.lowValue, slider.highValue);
                if (Math.Abs(slider.value - clamped) > 0.0001f)
                    slider.SetValueWithoutNotify(clamped);

                setter(clamped);
            });

            row.Add(slider);
            row.Add(input);

            return (row, slider);
        }

        public static Slider CreateSlider(string label, float value, System.Action<float> onChange, float min = 0f, float max = 1f)
        {
            var slider = new Slider(label, min, max) { value = value };
            slider.RegisterValueChangedCallback(evt => onChange(evt.newValue));
            return slider;
        }

        public static Slider CreateSlider(string label, int value, System.Action<int> onChange, int min = 0, int max = 1)
        {
            var slider = new Slider(label, min, max) { value = value };
            slider.RegisterValueChangedCallback(evt => onChange((int)evt.newValue));
            return slider;
        }

        public static Label CreateLabel(string text, bool bold = false, Color? color = null, float top = 4, float bottom = 2)
        {
            var label = new Label(text)
            {
                style =
                {
                    marginTop = top,
                    marginBottom = bottom
                }
            };

            if (bold)
                label.style.unityFontStyleAndWeight = FontStyle.Bold;

            if (color.HasValue)
                label.style.color = color.Value;

            return label;
        }

        public static PopupField<T> CreatePopup<T>(
            string label,
            List<T> options,
            int defaultIndex,
            Action<T, int> onChanged = null,
            float marginTop = 2,
            float marginBottom = 2)
        {
            var popup = new PopupField<T>(label, options, defaultIndex)
            {
                style =
                {
                    marginTop = marginTop,
                    marginBottom = marginBottom
                }
            };

            if (onChanged != null)
            {
                popup.RegisterValueChangedCallback(evt =>
                {
                    int index = options.IndexOf(evt.newValue);
                    onChanged(evt.newValue, index);
                });
            }

            return popup;
        }

        public static TextField CreateTextField(string label, string initialValue, Action<string> onValueChanged = null, string tooltip = "", Color? color = null)
        {
            var textField = new TextField(label)
            {
                value = initialValue ?? string.Empty,
                tooltip = tooltip ?? "",
                style =
                {
                    marginTop = 4,
                    marginBottom = 4,
                    flexGrow = 1
                }
            };

            if (color.HasValue)
            {
                textField.labelElement.style.color = new StyleColor(color.Value);
            }

            textField.RegisterValueChangedCallback(evt =>
            {
                onValueChanged?.Invoke(evt.newValue);
            });

            return textField;
        }

        public static VisualElement CreateHelpBox(string message, HelpBoxMessageType type = HelpBoxMessageType.Info)
        {
            var helpBox = new HelpBox(message, type)
            {
                style =
                {
                    marginTop = 6,
                    marginBottom = 6,
                    marginLeft = 2,
                    marginRight = 2
                }
            };

            return helpBox;
        }

        public static PopupField<string> CreatePopup(string label, List<string> options, int defaultIndex = 0, Action<string, int> onChanged = null)
        {
            if (options == null || options.Count == 0)
                options = new List<string> { "(none)" };

            var popup = new PopupField<string>(label, options, defaultIndex);
            popup.style.marginTop = 4;
            popup.style.marginBottom = 4;

            popup.RegisterValueChangedCallback(evt =>
            {
                onChanged?.Invoke(evt.newValue, popup.index);
            });

            return popup;
        }

    }

}
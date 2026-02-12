using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;


namespace InGameTextEditor.Format
{
    /// <summary>
    /// Defines a text style used when formatting the text editor's content.
    /// </summary>
    [System.Serializable]
    public class TextStyle
    {
        /// <summary>
        /// Font style.
        /// </summary>
        public FontStyles fontStyle = FontStyles.Normal;

        /// <summary>
        /// Indicates whether the default font style should be overridden by
        /// this text style.
        /// </summary>
        public bool overrideFontStyle = false;

        /// <summary>
        /// Font color.
        /// </summary>
        public Color fontColor = new Color(0f, 0f, 0f, 1f);

        /// <summary>
        /// Indicates whether the default font color should be overridden by
        /// this text style.
        /// </summary>
        public bool overrideColor = false;

        /// <summary>
        /// Creates a new text style overriding the default font style but
        /// leaving the font color unchanged.
        /// </summary>
        /// <param name="fontStyle">Font style.</param>
        public TextStyle(FontStyles fontStyle)
        {
            this.fontStyle = fontStyle;
            overrideFontStyle = true;
        }

        /// <summary>
        /// Creates a new text style overriding the default font color but
        /// leaving the font style unchanged.
        /// </summary>
        /// <param name="fontColor">Font color.</param>
        public TextStyle(Color fontColor)
        {
            this.fontColor = fontColor;
            overrideColor = true;
        }

        /// <summary>
        /// Creates a new text style overriding both the default font style and
        /// the default font color.
        /// </summary>
        /// <param name="fontStyle">Font style.</param>
        /// <param name="fontColor">Font color.</param>
        public TextStyle(FontStyles fontStyle, Color fontColor)
        {
            this.fontStyle = fontStyle;
            this.fontColor = fontColor;
            overrideFontStyle = true;
            overrideColor = true;
        }

        /// <summary>
        /// Gets the richt text open tag for this text style.
        /// </summary>
        /// <value>The richt text open tag.</value>
        public string RichtTextOpenTag
        {
            get
            {
                var openTag = new StringBuilder();
                if (overrideColor) { openTag.Append("<color=#"); openTag.Append(ColorUtility.ToHtmlStringRGBA(fontColor)); openTag.Append(">"); }
                
                if (!overrideFontStyle) return openTag.ToString();
                if ((fontStyle & FontStyles.Bold) != 0) openTag.Append("<b>");
                if ((fontStyle & FontStyles.Italic) != 0) openTag.Append("<i>");

                return openTag.ToString();
            }
        }

        /// <summary>
        /// Gets the richt text close tag for this text style.
        /// </summary>
        /// <value>The richt text close tag.</value>
        public string RichtTextCloseTag
        {
            get
            {
                var closeTag = new StringBuilder();

                if (overrideFontStyle)
                {
                    if ((fontStyle & FontStyles.Italic) != 0) closeTag.Append("</i>");
                    if ((fontStyle & FontStyles.Bold) != 0) closeTag.Append("</b>");
                }

                if (overrideColor) closeTag.Append("</color>");

                return closeTag.ToString();
            }
        }
    }
}
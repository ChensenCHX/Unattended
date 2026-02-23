using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MarkdownToUnity.Runtime
{
    public static class SyntaxHighlighter
    {
        private static readonly string KeywordColor = "blue";
        private static readonly string StringColor = "orange";
        private static readonly string CommentColor = "green";
        private static readonly string NumberColor = "magenta";
        private static readonly string PropertyColor = "cyan"; 
        private static readonly string FunctionColor = "yellow";


        private static readonly Dictionary<string, HashSet<string>> LanguageKeywords =
            new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase)
        {
            {
                "csharp", new HashSet<string>
                {
                    "public","private","protected","class","struct","enum","void","int","float","double","string","bool",
                    "if","else","for","foreach","while","do","switch","case","return","new","using","namespace","try","catch","finally",
                    "true","false","null","var","async","await","yield","get","set"
                }
            },
            {
                "python", new HashSet<string>
                {
                    "def","class","import","from","as","if","elif","else","for","while","return","True","False","None",
                    "try","except","finally","with","yield","async","await","lambda","global","nonlocal","pass","break","continue"
                }
            },
            {
                "json", new HashSet<string>() // JSON hat keine Keywords, aber wir färben Keys
            }
        };

       
        public static string Highlight(string code, string languageKey)
        {
            if (string.IsNullOrEmpty(code))
                return string.Empty;

            string highlighted = code;

            switch (languageKey.ToLowerInvariant())
            {
                case "csharp":
                case "cs":
                    highlighted = HighlightCSharp(highlighted);
                    break;
                case "python":
                case "py":
                    highlighted = HighlightPython(highlighted);
                    break;
                case "json":
                    highlighted = HighlightJson(highlighted);
                    break;
                default:
                    highlighted = HighlightGeneric(highlighted);
                    break;
            }

            return highlighted;
        }

        private static string HighlightCSharp(string code)
        {
            string result = code;

            // Comments
            result = Regex.Replace(result, @"//.*?$",
                m => WrapColor(m.Value, CommentColor), RegexOptions.Multiline);

            // Strings
            result = Regex.Replace(result, "\".*?\"",
                m => WrapColor(m.Value, StringColor));

            // Numbers
            result = Regex.Replace(result, @"\b\d+(\.\d+)?\b",
                m => WrapColor(m.Value, NumberColor));

            // Functions
            result = Regex.Replace(result, @"\b([A-Za-z_]\w*)(?=\s*\()", 
                m => WrapColor(m.Value, FunctionColor));

            // Keywords
            result = Regex.Replace(result, @"\b\w+\b", m =>
            {
                if (LanguageKeywords["csharp"].Contains(m.Value))
                    return WrapColor(m.Value, KeywordColor);
                return m.Value;
            });

            return result;
        }

        private static string HighlightPython(string code)
        {
            string result = code;

            // Comments (# ...)
            result = Regex.Replace(result, @"#.*?$",
                m => WrapColor(m.Value, CommentColor), RegexOptions.Multiline);

            // Strings
            result = Regex.Replace(result, @"(['""]).*?\1",
                m => WrapColor(m.Value, StringColor));

            // Numbers
            result = Regex.Replace(result, @"\b\d+(\.\d+)?\b",
                m => WrapColor(m.Value, NumberColor));

            // Function definitions
            result = Regex.Replace(result, @"(?<=\bdef\s+)([A-Za-z_]\w*)",
                m => WrapColor(m.Value, FunctionColor));

            // Funktion calls
            result = Regex.Replace(result, @"\b([A-Za-z_]\w*)(?=\s*\()",
                m => WrapColor(m.Value, FunctionColor));

            // Keywords
            result = Regex.Replace(result, @"\b\w+\b", m =>
            {
                if (LanguageKeywords["python"].Contains(m.Value))
                    return WrapColor(m.Value, KeywordColor);
                return m.Value;
            });

            return result;
        }

        private static string HighlightJson(string code)
        {
            string result = code;

            // Keys: "key":
            result = Regex.Replace(result, "\"(.*?)\"(?=\\s*:)",
                m => WrapColor(m.Value, PropertyColor));

            // Strings (Values)
            result = Regex.Replace(result, ":\\s*\".*?\"",
                m => ":" + WrapColor(m.Value.Substring(1), StringColor));

            // Numbers
            result = Regex.Replace(result, @"\b\d+(\.\d+)?\b",
                m => WrapColor(m.Value, NumberColor));

            // true/false/null
            result = Regex.Replace(result, @"\b(true|false|null)\b",
                m => WrapColor(m.Value, KeywordColor), RegexOptions.IgnoreCase);

            return result;
        }

        private static string HighlightGeneric(string code)
        {
            string result = code;

            // Strings
            result = Regex.Replace(result, "\".*?\"",
                m => WrapColor(m.Value, StringColor));

            // Numbers
            result = Regex.Replace(result, @"\b\d+(\.\d+)?\b",
                m => WrapColor(m.Value, NumberColor));

            return result;
        }

        private static string WrapColor(string text, string color)
        {
            return $"<color={color}>{text}</color>";
        }
    }

}
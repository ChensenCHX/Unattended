using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace InGameTextEditor.Format
{
    public class LuaSyntaxHighlighter : TextFormatter
    {
        // ===========================
        // 样式
        // ===========================

        public TextStyle textStyleComment       = new TextStyle(new Color(0.5f, 0.5f, 0.5f));
        public TextStyle textStyleString        = new TextStyle(new Color(0.9f, 0.4f, 0.1f));
        public TextStyle textStyleNumber        = new TextStyle(new Color(0.2f, 0.4f, 0.8f));
        public TextStyle textStyleKeyword       = new TextStyle(new Color(0.2f, 0.7f, 0.7f));
        public TextStyle textStyleKeywordValue  = new TextStyle(new Color(0.9f, 0.4f, 0.1f));
        public TextStyle textStyleFunction      = new TextStyle(new Color(0.3f, 0.9f, 0.3f));
        public TextStyle textStyleMember        = new TextStyle(new Color(0.4f, 0.7f, 1f));
        public TextStyle textStyleLabel         = new TextStyle(new Color(0.8f, 0.6f, 0.2f));

        public TextStyle textStyleSelf          = new TextStyle(new Color(1f, 0.4f, 0.4f));

        readonly string[] keywordsControl =
        {
            "break","do","else","elseif","end","for","function",
            "goto","if","in","repeat","return",
            "then","until","while"
        };

        readonly string[] keywordsValue =
        {
            "and","true","false","nil","or","not","local","..."
        };

        Regex regex;

        bool initialized = false;
        public override bool Initialized => initialized;

        // ===========================
        // INIT
        // ===========================

        public override void Init()
        {
            string pattern = "";

            pattern += @"(?<comment>--(?!\[=*\[).*$)";
            pattern += @"|(?<label>::[a-zA-Z_]\w*::)";
            pattern += @"|(?<self>\bself\b)";
            pattern += @"|(?<string>""(?:[^""\\]|\\.)*"")";
            pattern += @"|(?<string>'(?:[^'\\]|\\.)*')";
            pattern += @"|(?<number>\b\d+(\.\d+)?\b)";

            pattern += @"|(?<keyword>\b(";
            for (int i = 0; i < keywordsControl.Length; i++)
            {
                pattern += keywordsControl[i];
                if (i < keywordsControl.Length - 1)
                    pattern += "|";
            }
            pattern += @")\b)";

            pattern += @"|(?<keywordvalue>(";
            for (int i = 0; i < keywordsValue.Length; i++)
            {
                string k = keywordsValue[i];
                if (k == "...")
                {
                    pattern += @"\.{3}";
                }
                else
                {
                    pattern += @"\b" + k + @"\b";
                }

                if (i < keywordsValue.Length - 1)
                    pattern += "|";
            }
            pattern += @"))";

            pattern += @"|(?<identifier>\b[a-zA-Z_]\w*\b)";

            regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.Multiline);

            initialized = true;
        }

        // ===========================
        // LINE CHANGE
        // ===========================

        public override void OnLineChanged(Line line)
        {
            List<TextFormatGroup> groups = new List<TextFormatGroup>();
            string text = line.Text;

            bool prevLongComment =
                line.PreviousLine != null &&
                line.PreviousLine.GetProperty<bool>("endsWithLongComment", false);

            bool prevLongString =
                line.PreviousLine != null &&
                line.PreviousLine.GetProperty<bool>("endsWithLongString", false);

            int equalsCount =
                line.PreviousLine != null
                ? line.PreviousLine.GetProperty<int>("longBracketEqualsCount", 0)
                : 0;

            if (HandleLongState(line, text, groups, prevLongComment, prevLongString, equalsCount))
                return;

            Match longMatch = Regex.Match(text, @"(--)?\[(=*)\[");

            if (longMatch.Success)
            {
                bool isComment = longMatch.Groups[1].Success;
                int eqCount = longMatch.Groups[2].Value.Length;

                groups.Add(new TextFormatGroup(
                    longMatch.Index,
                    text.Length - 1,
                    isComment ? textStyleComment : textStyleString));

                line.SetProperty("endsWithLongComment", isComment);
                line.SetProperty("endsWithLongString", !isComment);
                line.SetProperty("longBracketEqualsCount", eqCount);

                line.ApplyTextFormat(groups);
                return;
            }

            ApplyRegex(text, 0, groups);

            line.SetProperty("endsWithLongComment", false);
            line.SetProperty("endsWithLongString", false);
            line.SetProperty("longBracketEqualsCount", 0);

            line.ApplyTextFormat(groups);
        }

        // ===========================
        // LONG STATE
        // ===========================

        bool HandleLongState(Line line, string text, List<TextFormatGroup> groups,
            bool prevComment, bool prevString, int eqCount)
        {
            if (!prevComment && !prevString)
                return false;

            string endToken = "]" + new string('=', eqCount) + "]";
            int endIndex = text.IndexOf(endToken);

            if (endIndex >= 0)
            {
                groups.Add(new TextFormatGroup(
                    0,
                    endIndex + endToken.Length - 1,
                    prevComment ? textStyleComment : textStyleString));

                string rest = text.Substring(endIndex + endToken.Length);
                ApplyRegex(rest, endIndex + endToken.Length, groups);

                line.SetProperty("endsWithLongComment", false);
                line.SetProperty("endsWithLongString", false);

                line.ApplyTextFormat(groups);
                return true;
            }
            else
            {
                if (text.Length > 0)
                {
                    groups.Add(new TextFormatGroup(
                        0,
                        text.Length - 1,
                        prevComment ? textStyleComment : textStyleString));
                }

                line.SetProperty("endsWithLongComment", prevComment);
                line.SetProperty("endsWithLongString", prevString);
                line.SetProperty("longBracketEqualsCount", eqCount);

                line.ApplyTextFormat(groups);
                return true;
            }
        }

        // ===========================
        // REGEX APPLY
        // ===========================

        void ApplyRegex(string text, int offset, List<TextFormatGroup> groups)
        {
            MatchCollection matches = regex.Matches(text);

            foreach (Match match in matches)
            {
                foreach (string groupName in regex.GetGroupNames())
                {
                    if (groupName == "0") continue;

                    Group g = match.Groups[groupName];
                    if (!g.Success) continue;

                    int start = offset + g.Index;
                    int end = start + g.Length - 1;

                    switch (groupName)
                    {
                        case "comment":
                            groups.Add(new TextFormatGroup(start, end, textStyleComment));
                            break;

                        case "string":
                            groups.Add(new TextFormatGroup(start, end, textStyleString));
                            break;

                        case "number":
                            groups.Add(new TextFormatGroup(start, end, textStyleNumber));
                            break;

                        case "keyword":
                            groups.Add(new TextFormatGroup(start, end, textStyleKeyword));
                            break;

                        case "keywordvalue":
                            groups.Add(new TextFormatGroup(start, end, textStyleKeywordValue));
                            break;

                        case "label":
                            groups.Add(new TextFormatGroup(start, end, textStyleLabel));
                            break;

                        case "self":
                            groups.Add(new TextFormatGroup(start, end, textStyleSelf));
                            break;

                        case "identifier":
                            ColorizeIdentifier(text, start, end, groups);
                            break;
                    }
                }
            }
        }

        // ===========================
        // IDENTIFIER RULE
        // ===========================

        void ColorizeIdentifier(string text, int start, int end, List<TextFormatGroup> groups)
        {
            int next = end + 1;

            while (next < text.Length && char.IsWhiteSpace(text[next]))
                next++;

            if (next >= text.Length)
                return;

            char c = text[next];

            if (c == '(')
            {
                groups.Add(new TextFormatGroup(start, end, textStyleFunction));
            }
            else if (c == '.' || c == ':' || c == '[')
            {
                groups.Add(new TextFormatGroup(start, end, textStyleMember));
            }
        }
    }
}

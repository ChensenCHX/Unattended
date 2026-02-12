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

        public TextStyle textStyleComment       = new TextStyle(new Color(0.42f, 0.54f, 0.22f));
        public TextStyle textStyleString        = new TextStyle(new Color(0.85f, 0.48f, 0.25f));
        public TextStyle textStyleNumber        = new TextStyle(new Color(0.60f, 1.00f, 0.40f));
        public TextStyle textStyleKeyword       = new TextStyle(new Color(0.72f, 0.27f, 0.69f));
        public TextStyle textStyleKeywordValue  = new TextStyle(new Color(0.22f, 0.57f, 0.87f));
        public TextStyle textStyleFunction      = new TextStyle(new Color(1.00f, 1.00f, 0.23f));
        public TextStyle textStyleMember        = new TextStyle(new Color(0.39f, 0.77f, 1.00f));
        public TextStyle textStyleLabel         = new TextStyle(new Color(0.28f, 0.93f, 0.76f));
        public TextStyle textStyleEscape        = new TextStyle(new Color(1.00f, 0.80f, 0.20f));
        public TextStyle textStyleBadEscape     = new TextStyle(new Color(1.00f, 0.20f, 0.20f));

        readonly string[] keywordsControl =
        {
            "break","do","else","elseif","end","for","function",
            "goto","if","in","repeat","return",
            "then","until","while"
        };

        readonly string[] keywordsValue =
        {
            "and","true","false","nil","or","not","local","self","..."
        };

        Regex regex;
        // Strict valid escape sequences for Lua 5.2: \a \b \f \n \r \t \v \\\ " ' \z<ws>* and octal \ddd (0-7) and hex \xHH
        static readonly Regex validEscapeRegex = new Regex(@"\\(?:(?:[0-7]{1,3})|(?:x[0-9A-Fa-f]{1,2})|(?:z\s*)|[abfnrtv""'\\])", RegexOptions.Compiled);

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
            // `self` is handled as a keywordvalue now
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

                // 循环扫描 rest，识别可能存在的多个长括号开/关对：
                // - 在遇到新的开头时，先把开头之前的普通部分用短正则处理；
                // - 若该开头在同一行能找到对应的结束标记，则把整个段落着色并继续扫描其后部分；
                // - 若找不到结束标记，则把从开头到行尾作为新的多行状态处理并返回。
                int scanPos = 0;
                int baseOffset = endIndex + endToken.Length;
                while (scanPos < rest.Length)
                {
                    // use substring to get a Match starting at scanPos
                    string restSub = rest.Substring(scanPos);
                    Match innerMatch = Regex.Match(restSub, "(--)?\\[(=*)\\[");
                    if (!innerMatch.Success)
                    {
                        // no more openers, process the tail and finish
                        if (scanPos < rest.Length)
                            ApplyRegex(rest.Substring(scanPos), baseOffset + scanPos, groups);

                        line.SetProperty("endsWithLongComment", false);
                        line.SetProperty("endsWithLongString", false);
                        line.SetProperty("longBracketEqualsCount", 0);

                        line.ApplyTextFormat(groups);
                        return true;
                    }

                    // process text before this opener
                    if (innerMatch.Index > 0)
                    {
                        ApplyRegex(restSub.Substring(0, innerMatch.Index), baseOffset + scanPos, groups);
                    }

                    bool isComment2 = innerMatch.Groups[1].Success;
                    int eq2 = innerMatch.Groups[2].Value.Length;

                    // absolute index of opener in rest
                    int openerRel = scanPos + innerMatch.Index;
                    int openerAbs = baseOffset + openerRel;

                    // search for the corresponding closer within rest starting after the opener
                    string closeToken2 = "]" + new string('=', eq2) + "]";
                    int closeRel = rest.IndexOf(closeToken2, openerRel + innerMatch.Length);

                    if (closeRel >= 0)
                    {
                        // closer exists on same line: color the whole [==[ ... ]==] segment and continue
                        int closerAbsEnd = baseOffset + closeRel + closeToken2.Length - 1;
                        groups.Add(new TextFormatGroup(
                            openerAbs,
                            closerAbsEnd,
                            isComment2 ? textStyleComment : textStyleString));

                        // advance scanPos to after the closer
                        scanPos = closeRel + closeToken2.Length;
                        continue;
                    }
                    else
                    {
                        // no closer on this line: the opener starts a multi-line block that continues
                        groups.Add(new TextFormatGroup(
                            openerAbs,
                            text.Length - 1,
                            isComment2 ? textStyleComment : textStyleString));

                        line.SetProperty("endsWithLongComment", isComment2);
                        line.SetProperty("endsWithLongString", !isComment2);
                        line.SetProperty("longBracketEqualsCount", eq2);

                        line.ApplyTextFormat(groups);
                        return true;
                    }
                }

                // processed entire rest
                line.SetProperty("endsWithLongComment", false);
                line.SetProperty("endsWithLongString", false);
                line.SetProperty("longBracketEqualsCount", 0);
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
                        {
                            string s = text.Substring(g.Index, g.Length);
                            int localOffset = start; // start index of the string in the whole line

                            int i = 0;
                            int cur = 0;
                            while (i < s.Length)
                            {
                                if (s[i] == '\\')
                                {
                                    // add preceding plain string segment
                                    if (i > cur)
                                    {
                                        groups.Add(new TextFormatGroup(
                                            localOffset + cur,
                                            localOffset + i - 1,
                                            textStyleString));
                                    }

                                    // try to match a valid escape at position i
                                    Match vm = validEscapeRegex.Match(s, i);
                                    if (vm.Success && vm.Index == i)
                                    {
                                        groups.Add(new TextFormatGroup(
                                            localOffset + i,
                                            localOffset + i + vm.Length - 1,
                                            textStyleEscape));

                                        i += vm.Length;
                                        cur = i;
                                    }
                                    else
                                    {
                                        // invalid escape: highlight backslash and next char (if any)
                                        int badLen = (i + 1 < s.Length) ? 2 : 1;
                                        groups.Add(new TextFormatGroup(
                                            localOffset + i,
                                            localOffset + i + badLen - 1,
                                            textStyleBadEscape));

                                        i += badLen;
                                        cur = i;
                                    }
                                }
                                else
                                {
                                    i++;
                                }
                            }

                            if (cur < s.Length)
                            {
                                groups.Add(new TextFormatGroup(
                                    localOffset + cur,
                                    localOffset + s.Length - 1,
                                    textStyleString));
                            }
                        }
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
            // If followed by '(', string, long-bracket opener or '{' treat as function call/construction
            if (c == '(' || c == '\'' || c == '"' || c == '{' || (c == '[' && IsLongBracketStart(text, next)))
            {
                groups.Add(new TextFormatGroup(start, end, textStyleFunction));
            }
            else if (c == '.' || c == ':' || c == '[')
            {
                // '[' that is not a long-bracket opener is an indexing/member access
                groups.Add(new TextFormatGroup(start, end, textStyleMember));
            }
        }

        bool IsLongBracketStart(string text, int pos)
        {
            // expects text[pos] == '['
            if (pos >= text.Length || text[pos] != '[') return false;

            int i = pos + 1;
            while (i < text.Length && text[i] == '=')
                i++;

            return (i < text.Length && text[i] == '[');
        }
    }
}

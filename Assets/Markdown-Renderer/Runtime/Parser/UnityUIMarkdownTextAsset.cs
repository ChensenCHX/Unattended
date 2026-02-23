using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkdownToUnity.Runtime
{
    public class UnityUIMarkdownTextAsset
    {
        public List<Block> blocks = new List<Block>();
        public Dictionary<string, string> internData; // Images mostly
        public Dictionary<string, string> externData;

        public abstract class Block { }

        public class TextBlock : Block
        {
            public string RichText;
        }

        public class ImageBlock : Block
        {
            public string Key;
            public string Url;
        }

        public class VideoBlock : Block
        {
            public string Url;
        }

        public class Blockquote : Block
        {
            public string RichText;
        }

        public class HorizontalLine : Block
        {
            public string RichText;
        }

        public class TableBlock : Block
        {
            public List<TableRow> Rows;
        }

        public class TableRow
        {
            public List<TableCell> Cells;
        }

        public class TableCell
        {
            public List<Block> blocks;
        }

        public class LinkBlock : Block
        {
            public string Text;
            public string Uri;
        }

        public class CodeBlock : Block
        {
            public string Language;
            public string Code;
        }
    }
}
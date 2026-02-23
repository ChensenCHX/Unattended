using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Linq;
using System.IO.Compression;
using System.IO;
using System.Threading.Tasks;

namespace MarkdownToUnity.Runtime
{
    public static class MarkdownUnityParser
    {
        private static Dictionary<string, string> externData = new Dictionary<string, string>();
        private static Dictionary<string, string> nestedMarkChapters = new Dictionary<string, string>();

        private static List<string> temporaryDirectories = new List<string>();
        private static bool dirty = false;

        private static string standardLanguageMarker = "EN";

        public static void Clean() {
            foreach (var pathDirectory in temporaryDirectories) {
                Directory.Delete(pathDirectory, true);
            }
            temporaryDirectories.Clear();

            nestedMarkChapters.Clear();
            externData.Clear();
        }

        public static bool HasLoadedChapters() { return nestedMarkChapters != null && nestedMarkChapters.Count > 0; }

        public static Dictionary<string, string> GetChapterDictionary() { return nestedMarkChapters; }

        public static async void OpenMarkbook( TextAsset markBook, string languageMarker = null ) {
            await ParseMarkdown( markBook, languageMarker );
        }

        public static async Task<UnityUIMarkdownTextAsset> OpenMarkbookChapter(string chapterName, string languageMarker = null)
        {
            if (nestedMarkChapters.ContainsKey(chapterName))
            {
                string zipFilePath = nestedMarkChapters[chapterName];

                using (FileStream zipFileStream = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
                {
                    return await ParseMarkdown(zipFileStream, languageMarker);
                }
            }
            else
            {
                Debug.LogWarning($"Chapter '{chapterName}' not found.");
            }

            return null;
        }


        public static async Task<UnityUIMarkdownTextAsset> ParseMarkdown(object markZip, string languageMarker = null)
        {
            ZipArchive zipArchive = ConvertToZipArchive(markZip);

            var tempFolder = Path.Combine(Application.temporaryCachePath, Path.GetRandomFileName());
            Directory.CreateDirectory(tempFolder);

            MarkdownUnityParser.temporaryDirectories.Add(tempFolder);
            if (!MarkdownUnityParser.dirty)
            {
                Application.quitting += MarkdownUnityParser.Clean;
                MarkdownUnityParser.dirty = true;
            }

            ZipArchiveEntry markdownFileEntry = null;
            MarkdownUnityParser.externData = new Dictionary<string, string>();
            UnityUIMarkdownTextAsset markdownAsset = null; bool markBook = false;

            foreach (var entry in zipArchive.Entries)
            {

                var entryPath = Path.Combine(tempFolder, entry.FullName);
                if (string.IsNullOrEmpty(entry.Name))
                {
                    Directory.CreateDirectory(entryPath);
                }
                else
                {
                    if (entry.FullName.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
                    {
                        if (markdownFileEntry == null)
                            markdownFileEntry = entry;
                        else if (languageMarker != null)
                        {
                            if (entry.FullName.Contains(languageMarker))
                            {
                                markdownFileEntry = entry;
                            }
                            else if (!markdownFileEntry.FullName.Contains(languageMarker) && entry.FullName.Contains(standardLanguageMarker))
                            {
                                markdownFileEntry = entry;
                            }
                        }
                    }
                    else if (entry.FullName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                    {
                        markBook = true;

                        string fullFileName = Path.GetFileName(entry.FullName);

                        var nestedZipPath = Path.Combine(tempFolder, fullFileName);

                        using (var entryStream = entry.Open())
                        using (var fileStream = new FileStream(nestedZipPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                        {
                            await entryStream.CopyToAsync(fileStream);
                        }

                        MarkdownUnityParser.nestedMarkChapters[fullFileName] = nestedZipPath;
                    }
                    else
                    {
                        // Andere Dateien speichern
                        MarkdownUnityParser.externData[Uri.EscapeDataString(entry.Name)] = entryPath;
                        Directory.CreateDirectory(Path.GetDirectoryName(entryPath));
                        entry.ExtractToFile(entryPath, overwrite: true);
                    }
                }
            }

            if (markdownFileEntry != null)
            {
                using (var entryStream = markdownFileEntry.Open())
                using (var reader = new StreamReader(entryStream))
                {
                    var markdownContent = await reader.ReadToEndAsync();
                    markdownAsset = ParseMarkdown(markdownContent);
                }

                if (languageMarker != null && !markdownFileEntry.FullName.Contains(languageMarker))
                {
                    Debug.LogError("No matching translation markdown file found in the ZIP archive.");
                }
            }
            else if (!markBook)
            {
                Debug.LogError("No markdown file found in the ZIP archive.");
            }

            return markdownAsset;
        }


        private static ZipArchive ConvertToZipArchive(object markZip)
        {
            if (markZip is ZipArchive zipArchive)
            {
                return zipArchive;
            }
            else if (markZip is Stream stream)
            {
                return new ZipArchive(stream, ZipArchiveMode.Read);
            }
            else if (markZip is byte[] byteArray)
            {
                return new ZipArchive(new MemoryStream(byteArray), ZipArchiveMode.Read);
            }
            else if (markZip is string filePath && File.Exists(filePath))
            {
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096);
                return new ZipArchive(fileStream, ZipArchiveMode.Read);
            }
            else if (markZip is TextAsset textAsset)
            {
                return new ZipArchive(new MemoryStream(textAsset.bytes), ZipArchiveMode.Read);
            }
            else
            {
                throw new ArgumentException("Unsupported object type", nameof(markZip));
            }
        }



        public static UnityUIMarkdownTextAsset ParseMarkdown(string markdown)
        {

            Dictionary<string, string> internData = ExtractImageReferences(ref markdown);
            
            UnityUIMarkdownTextAsset asset = new UnityUIMarkdownTextAsset {
                internData = internData,
                externData = MarkdownUnityParser.externData
            };

            // ! IMPORTANT: Processs Blocks first. Otherwise Names_URLS would be Parsed in RichText.

            Blocky(richText: markdown, ref asset);

            foreach (var block in asset.blocks) {
                ProcessMarkdownBlock(block);
            }

            return asset;
        }
        
        private static void ProcessMarkdownBlock(UnityUIMarkdownTextAsset.Block block)
        {
            switch (block)
            {
                case UnityUIMarkdownTextAsset.TextBlock textBlock:
                    textBlock.RichText = ProcessMarkdown(textBlock.RichText);
                    break;

                case UnityUIMarkdownTextAsset.TableBlock tableBlock:
                    foreach (var cellTextBlock in tableBlock.Rows
                                .SelectMany(r => r.Cells)
                                .SelectMany(c => c.blocks)
                                .OfType<UnityUIMarkdownTextAsset.TextBlock>())
                    {
                        cellTextBlock.RichText = ProcessMarkdown(cellTextBlock.RichText);
                    }
                    break;
            }
        }

        public static void Blocky(string richText, ref UnityUIMarkdownTextAsset asset)
        {

            richText += "\n \n";

            var regexPattern =
                       @"(?<table>(^\|(.+?)\|\s*(\n|$))+)|" +                                           // Detect simple table
                       @"(?<videoUrl>!\[.*?\]\((?<videoUri>.*?)\.mp4\))|" +                             // Detect video by URL (assuming .mp4 for simplicity)
                       @"(?<imageKey>!\[\]\[(?<imageKeyName>.*?)\])|" +                                 // Detect internal image by key
                       @"(?<imageUrl>!\[.*?\]\((?<imageUri>.*?)\))|" +                                  // Detect external image by URL
                       @"(?<blockquote>^> .+?$)|" +                                                     // Detect blockquote
                       @"(?<horizontalline>^\s{0,3}(?:-{3,}|_{3,}|\*{3,})(?:\s*)$)|" +                  // Detect horizontal line
                       @"(?<link>\[(?<linkText>.*?)\]\((?<linkUri>.*?)\))|" +                           // Detect Links
                       @"(?<codeblock>```[ \t]*(?<language>\w+)?\s*\n(?<code>[\s\S]*?)```[ \t]*)";      // Detect fenced code blocks

            var regex = new Regex(regexPattern, RegexOptions.Multiline | RegexOptions.Singleline);
            richText = richText.Replace("\r\n", "\n");
            var matches = regex.Matches(richText);

            int currentIndex = 0;

            foreach (Match match in matches)
            {
                if (match.Index > currentIndex)
                {
                    string textBetween = richText.Substring(currentIndex, match.Index - currentIndex).Trim();
                    if (!string.IsNullOrEmpty(textBetween))
                    {
                        asset.blocks.Add(new UnityUIMarkdownTextAsset.TextBlock { RichText = textBetween });
                    }
                }

                if (match.Groups["imageKey"].Success)
                {
                    // Internal image block identified by ![][imageKey]
                    string imageKey = match.Groups["imageKeyName"].Value;
                    asset.blocks.Add(new UnityUIMarkdownTextAsset.ImageBlock { Key = imageKey });
                }
                else if (match.Groups["imageUrl"].Success)
                {
                    // External image block identified by ![alt text](imageUrl)
                    string imageUrl = match.Groups["imageUri"].Value;
                    asset.blocks.Add(new UnityUIMarkdownTextAsset.ImageBlock { Url = imageUrl });
                }
                else if (match.Groups["videoUrl"].Success)
                {
                    // Video block identified by ![alt text](videoUrl.mp4)
                    string videoUrl = match.Groups["videoUri"].Value;
                    asset.blocks.Add(new UnityUIMarkdownTextAsset.VideoBlock { Url = videoUrl + ".mp4" }); // Assume .mp4 for now
                }
                else if (match.Groups["blockquote"].Success)
                {
                    // Blockquote identified by text starting with > on a new line
                    string blockquoteText = match.Value.Trim();

                    if (blockquoteText.StartsWith('>'))
                    {
                        blockquoteText = blockquoteText.Substring(1);
                    }

                    asset.blocks.Add(new UnityUIMarkdownTextAsset.Blockquote { RichText = blockquoteText });
                }
                else if (match.Groups["horizontalline"].Success)
                {
                    // Horizontal line identified by three or more dashes on a new line
                    asset.blocks.Add(new UnityUIMarkdownTextAsset.HorizontalLine { RichText = "---" });
                }
                else if (match.Groups["table"].Success)
                {
                    // Simple table identified by rows starting and ending with |
                    string tableText = match.Value.Trim();
                    asset.blocks.Add(ParseTableBlock(tableText));
                }
                else if (match.Groups["link"].Success)
                {
                    string linkText = match.Groups["linkText"].Value;
                    string linkUri = match.Groups["linkUri"].Value;
                    asset.blocks.Add(new UnityUIMarkdownTextAsset.LinkBlock { Text = linkText, Uri = linkUri });
                }
                else if (match.Groups["codeblock"].Success)
                {
                    string language = match.Groups["language"].Value;
                    string code = match.Groups["code"].Value;
                    asset.blocks.Add(new UnityUIMarkdownTextAsset.CodeBlock { Language = language, Code = code });
                }

                // Update current index to the end of the current match
                currentIndex = match.Index + match.Length;
            }

            // Handle any remaining text after the last match
            if (currentIndex < richText.Length)
            {
                string remainingText = richText.Substring(currentIndex).Trim();
                if (!string.IsNullOrEmpty(remainingText))
                {
                    asset.blocks.Add(new UnityUIMarkdownTextAsset.TextBlock { RichText = remainingText });
                }
            }
        }

        private static UnityUIMarkdownTextAsset.TableBlock ParseTableBlock(string tableText)
        {
            var rows = tableText.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var tableBlock = new UnityUIMarkdownTextAsset.TableBlock { Rows = new List<UnityUIMarkdownTextAsset.TableRow>() };

            foreach (var row in rows)
            {
                if (row.Contains("---")) continue;

                var cells = row.Split('|').Select(cell => cell.Trim()).Where(cell => !string.IsNullOrEmpty(cell)).ToList();
                var tableRow = new UnityUIMarkdownTextAsset.TableRow { Cells = new List<UnityUIMarkdownTextAsset.TableCell>() };

                foreach (var cell in cells)
                {
                    tableRow.Cells.Add(ParseCellContent(cell));
                }

                tableBlock.Rows.Add(tableRow);
            }

            return tableBlock;
        }


        private static UnityUIMarkdownTextAsset.TableCell ParseCellContent(string cellContent)
        {
            var blocks = new List<UnityUIMarkdownTextAsset.Block>();

            var regexPattern =
                       @"(?<videoUrl>!\[.*?\]\((?<videoUri>.*?)\.mp4\))|" +             // Detect video by URL (assuming .mp4 for simplicity)
                       @"(?<imageKey>!\[\]\[(?<imageKeyName>.*?)\])|" +                 // Detect internal image by key
                       @"(?<imageUrl>!\[.*?\]\((?<imageUri>.*?)\))|" +                  // Detect external image by URL
                       @"(?<blockquote>^> .+?$)|" +                                     // Detect blockquote
                       @"(?<horizontalline>^\s{0,3}(?:-{3,}|_{3,}|\*{3,})(?:\s*)$)|" +  // Detect horizontal line
                       @"(?<link>\[(?<linkText>.*?)\]\((?<linkUri>.*?)\))" +             // Detect Links
                       @"(?<codeblock>```(?<language>\w+)?\n(?<code>[\s\S]*?)```)";   // Detect fenced code blocks

            var regex = new Regex(regexPattern, RegexOptions.Multiline);
            cellContent = cellContent.Replace("\r\n", "\n");
            var matches = regex.Matches(cellContent);

            int currentIndex = 0;

            foreach (Match match in matches)
            {
                if (match.Index > currentIndex)
                {
                    string textBetween = cellContent.Substring(currentIndex, match.Index - currentIndex).Trim();
                    if (!string.IsNullOrEmpty(textBetween))
                    {
                        blocks.Add(new UnityUIMarkdownTextAsset.TextBlock { RichText = textBetween });
                    }
                }

                if (match.Groups["imageKey"].Success)
                {
                    // Internal image block identified by ![][imageKey]
                    string imageKey = match.Groups["imageKeyName"].Value;
                    blocks.Add(new UnityUIMarkdownTextAsset.ImageBlock { Key = imageKey });
                }
                else if (match.Groups["imageUrl"].Success)
                {
                    // External image block identified by ![alt text](imageUrl)
                    string imageUrl = match.Groups["imageUri"].Value;
                    blocks.Add(new UnityUIMarkdownTextAsset.ImageBlock { Url = imageUrl });
                }
                else if (match.Groups["videoUrl"].Success)
                {
                    // Video block identified by ![alt text](videoUrl.mp4)
                    string videoUrl = match.Groups["videoUri"].Value;
                    blocks.Add(new UnityUIMarkdownTextAsset.VideoBlock { Url = videoUrl + ".mp4" }); // Assume .mp4 for now
                }
                else if (match.Groups["blockquote"].Success)
                {
                    // Blockquote identified by text starting with > on a new line
                    string blockquoteText = match.Value.Trim();

                    if(blockquoteText.StartsWith('>')) {
                        blockquoteText = blockquoteText.Substring(1);
                    }

                    blocks.Add(new UnityUIMarkdownTextAsset.Blockquote { RichText = blockquoteText });
                }
                else if (match.Groups["horizontalline"].Success)
                {
                    // Horizontal line identified by three or more dashes on a new line
                    blocks.Add(new UnityUIMarkdownTextAsset.HorizontalLine { RichText = "---" });
                }
                else if (match.Groups["link"].Success)
                {
                    string linkText = match.Groups["linkText"].Value;
                    string linkUri = match.Groups["linkUri"].Value;
                    blocks.Add(new UnityUIMarkdownTextAsset.LinkBlock { Text = linkText, Uri = linkUri });
                }

                currentIndex = match.Index + match.Length;
            }

            if (currentIndex < cellContent.Length)
            {
                string remainingText = cellContent.Substring(currentIndex).Trim();
                if (!string.IsNullOrEmpty(remainingText))
                {
                    blocks.Add(new UnityUIMarkdownTextAsset.TextBlock { RichText = remainingText });
                }
            }

            var cell = new UnityUIMarkdownTextAsset.TableCell();
            cell.blocks = blocks;

            return cell;
        }

        public static Dictionary<string, string> ExtractImageReferences(ref string markdown) {
            Dictionary<string, string> imageReferences = new Dictionary<string, string>();
            var regex = new Regex(@"\[(image\d+)\]: <(data:image\/.+?)>", RegexOptions.Multiline);

            // Suche nach allen Bilddaten und füge sie zum Dictionary hinzu
            var matches = regex.Matches(markdown);
            foreach (Match match in matches)
            {
                string id = match.Groups[1].Value;      // Image-ID (z.B. image1)
                string data = match.Groups[2].Value;    // Imagedata (Base64)

                imageReferences[id] = data;
            }

            markdown = regex.Replace(markdown, string.Empty).Trim();

            return imageReferences;
        }

        private static string ProcessMarkdown(string markdownText) {
            
            string TMPString;

            TMPString = ProcessHeadings(markdownText);
            TMPString = ProcessEmphasis(TMPString);
            TMPString = HighlightInlineCode(TMPString);

            return TMPString;
        }

        public static string ProcessHeadings(string markdown) {
            var html = System.Text.RegularExpressions.Regex.Replace(markdown, @"^(#{1,6})\s*(.+)$", match =>
            {
                int level = match.Groups[1].Value.Length;
                string text = match.Groups[2].Value;
                int fontSize = 36 - (level - 1) * 6; // Angepasste Schriftgröße für TextMeshPro
                return $"<size={fontSize}><b>{text}</b></size>";
            }, RegexOptions.Multiline);
            return html;
        }

        public static string ProcessEmphasis(string markdown)
        {
            // Ersetze fettgedruckten Text (**Text**) mit <b>Text</b>
            var html = System.Text.RegularExpressions.Regex.Replace(markdown, @"\*\*\*(.*?)\*\*\*", "<b><i>$1</i></b>");
            html = System.Text.RegularExpressions.Regex.Replace(html, @"__\*(.*?)\*__", "<b><i>$1</i></b>");
            html = System.Text.RegularExpressions.Regex.Replace(html, @"\*\*_(.*?)_\*\*", "<b><i>$1</i></b>");
            html = System.Text.RegularExpressions.Regex.Replace(html, @"\*\*(.*?)\*\*", "<b>$1</b>");

            // Ersetze kursiven Text (*Text* oder _Text_) mit <i>Text</i>
            html = System.Text.RegularExpressions.Regex.Replace(html, @"___(.*?)___", "<b><i>$1</i></b>");
            html = System.Text.RegularExpressions.Regex.Replace(html, @"__(.*?)__", "<b>$1</b>");
            html = System.Text.RegularExpressions.Regex.Replace(html, @"\*\*\*(.*?)\*\*\*", "<b><i>$1</i></b>");
            html = System.Text.RegularExpressions.Regex.Replace(html, @"(\*|_)(.*?)\1", "<i>$2</i>");

            html = System.Text.RegularExpressions.Regex.Replace(html, @"~~(.*?)~~", "<s>$1</s>");

            return html;
        }
        
        public static string HighlightInlineCode(string markdown)
        {
            // Ersetze `code` mit farblich hervorgehobenem Text
            return System.Text.RegularExpressions.Regex.Replace(markdown, @"`(.*?)`", $"<color=#ff00ffff><b><size=75%>$1</size></b></color>");
        }


        private static MatchCollection SplitWithMarkers(string input, char[] splitChars) {
            if (splitChars == null || splitChars.Length == 0)
                throw new ArgumentException("No split characters provided.");

            string pattern = $"[{string.Join("", splitChars)}]";
            var regex = new Regex(pattern, RegexOptions.Compiled);
            return regex.Matches(input);
        }

        private static string[] SplitWithString(string input, string delimiter) {
            return input.Split(delimiter, StringSplitOptions.None);
        }
    }
}

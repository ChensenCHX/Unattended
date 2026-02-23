using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MarkdownToUnity.Editor
{
    public class MarkdownToBinaryAssetEditor : EditorWindow
    {
        private string pathToProcess;
        private string targetAssetPath = "Assets/Untitled.bytes"; // Default target path

        private int selectedConversionType = 0; // 0 for MarkZip, 1 for MarkBook
        private string[] conversionOptions = new[] { "MarkZip (Single Markdown Folder)", "MarkBook (Multiple Markdown Folders)" };

        [MenuItem("Window/MarkdownToUnityUI/Convert Markdown to Asset")]
        public static void ShowWindow()
        {
            GetWindow<MarkdownToBinaryAssetEditor>("Convert Markdown to Asset");
        }

        public VisualElement root;
        private VisualElement Content => root.Query<VisualElement>("content");


        public void CreateGUI()
        {
            root = new VisualElement()
            {
                style =
                {
                    flexGrow = 1,
                    marginLeft = -15
                }
            };

            VisualTreeAsset asset = UIFactory.LoadInspector("markdown-to-binary-inspector");
            if (asset != null)
                asset.CloneTree(root);

            var container = new VisualElement
            {
                style = { flexDirection = FlexDirection.Column, marginTop = 10 }
            };

            // --- Conversion Type ---
            var (foldOuter, foldContent, _) = UIFactory.CreateFoldout("Conversion Type", "Choose the conversion method.", new Color(0.3f, 0.6f, 1f), true);
            var popup = UIFactory.CreatePopup("Conversion Mode", new List<string>(conversionOptions), 0, (val, index) =>
            {
                selectedConversionType = index;
            });
            foldContent.Add(popup);
            container.Add(foldOuter);

            // --- Input Section ---
            var (inputOuter, inputFoldout, _) = UIFactory.CreateFoldout("Input", "Select your Markdown ZIP or folder to convert.", new Color(0.4f, 0.6f, 1f), true);
            var selectButton = UIFactory.CreateButton("Select Markdown ZIP File or Folder", new Color(0.3f, 0.5f, 0.9f), OnSelectInput);
            inputFoldout.Add(selectButton);
            container.Add(inputOuter);

            // --- Output Section ---
            var (outputOuter, outputFoldout, _) = UIFactory.CreateFoldout("Output", "Specify where to save the resulting binary asset.", new Color(0.3f, 0.8f, 0.5f), true);
            var outputPathField = UIFactory.CreateTextField("Target Path (.bytes)", targetAssetPath, val => targetAssetPath = val);
            outputFoldout.Add(outputPathField);
            container.Add(outputOuter);

            // --- ACTIONS ---
            var convertButton = UIFactory.CreateButton("Convert and Save", new Color(0.2f, 0.4f, 0.8f), OnConvertClicked);
            convertButton.style.marginTop = 15;
            convertButton.style.marginBottom = 15;
            convertButton.style.maxWidth = 300;
            convertButton.style.flexGrow = 0f;
            convertButton.style.alignSelf = Align.Center;
            container.Add(convertButton);

            if (Content != null)
                Content.Add(container);
            else
                root.Add(container);

            rootVisualElement.Add(root);
        }

        private void OnSelectInput()
        {
            if (EditorUtility.DisplayDialog("Choose Input Type", "Would you like to select a ZIP file or a folder?", "ZIP File", "Folder"))
            {
                string filePath = EditorUtility.OpenFilePanel("Select ZIP File", "", "zip");
                if (!string.IsNullOrEmpty(filePath))
                {
                    pathToProcess = filePath;
                    Debug.Log($"Selected ZIP File: {pathToProcess}");
                }
            }
            else
            {
                string folderPath = EditorUtility.OpenFolderPanel("Select Folder", "", "");
                if (!string.IsNullOrEmpty(folderPath))
                {
                    pathToProcess = folderPath;
                    Debug.Log($"Selected Folder: {pathToProcess}");
                }
            }
        }

        private void OnConvertClicked()
        {
            if (string.IsNullOrEmpty(pathToProcess))
            {
                EditorUtility.DisplayDialog("Error", "Please select a folder or ZIP file first.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(targetAssetPath))
            {
                EditorUtility.DisplayDialog("Error", "Please specify a valid target path.", "OK");
                return;
            }

            if (selectedConversionType == 0)
                ConvertMarkZip();
            else
                ConvertMarkBook();
        }

        private void ConvertMarkZip()
        {
            try
            {
                byte[] fileBytes;

                if (Directory.Exists(pathToProcess))
                {
                    string tempZipPath = Path.Combine(Path.GetTempPath(), "temp.zip");
                    ZipDirectory(pathToProcess, tempZipPath);
                    fileBytes = File.ReadAllBytes(tempZipPath);
                    File.Delete(tempZipPath);
                }
                else if (File.Exists(pathToProcess) && Path.GetExtension(pathToProcess).Equals(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    fileBytes = File.ReadAllBytes(pathToProcess);
                }
                else
                {
                    throw new ArgumentException("The selected path is neither a valid ZIP file nor a folder.", nameof(pathToProcess));
                }

                SaveBinaryAsset(fileBytes);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error during conversion and saving: " + ex.Message);
                EditorUtility.DisplayDialog("Error", "Error during conversion: " + ex.Message, "OK");
            }
        }

        private void ConvertMarkBook()
        {
            try
            {
                if (!Directory.Exists(pathToProcess))
                {
                    throw new ArgumentException("A folder must be selected for MarkBook conversion.", nameof(pathToProcess));
                }

                // Create a temporary folder for zipping
                string tempMainFolder = Path.Combine(Path.GetTempPath(), "markbook_temp");
                Directory.CreateDirectory(tempMainFolder);

                // Recursively process all subfolders and zip them if they contain .md files
                ProcessSubdirectoriesRecursively(pathToProcess, tempMainFolder);

                // Copy any ZIP files from the root of the original folder into the temp folder
                string[] allFiles = Directory.GetFiles(pathToProcess, "*.zip", SearchOption.TopDirectoryOnly);
                foreach (var zipFile in allFiles)
                {
                    string destFileName = Path.Combine(tempMainFolder, Path.GetFileName(zipFile));
                    File.Copy(zipFile, destFileName);
                }

                // Create the final ZIP for the entire folder with the same name as the main folder
                string finalZipName = Path.Combine(Path.GetTempPath(), Path.GetFileName(pathToProcess) + ".zip");
                ZipDirectory(tempMainFolder, finalZipName);

                byte[] fileBytes = File.ReadAllBytes(finalZipName);

                // Cleanup temporary files
                Directory.Delete(tempMainFolder, true);
                File.Delete(finalZipName);

                SaveBinaryAsset(fileBytes);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error during MarkBook conversion and saving: " + ex.Message);
                EditorUtility.DisplayDialog("Error", "Error during conversion: " + ex.Message, "OK");
            }
        }

        // Helper method to recursively process subdirectories
        private void ProcessSubdirectoriesRecursively(string currentDirectory, string tempMainFolder)
        {
            // Get all subdirectories in the current directory
            string[] subDirectories = Directory.GetDirectories(currentDirectory);

            foreach (var subDir in subDirectories)
            {
                // Check if the subfolder contains .md files
                string[] markdownFiles = Directory.GetFiles(subDir, "*.md", SearchOption.AllDirectories);

                if (markdownFiles.Length > 0)
                {
                    // If .md files are found, zip the subfolder and store it in the tempMainFolder
                    string zipFileName = Path.Combine(tempMainFolder, Path.GetFileName(subDir) + ".zip");
                    ZipDirectory(subDir, zipFileName);
                }

                // Recursively process deeper subfolders
                ProcessSubdirectoriesRecursively(subDir, tempMainFolder);
            }
        }


        private void SaveBinaryAsset(byte[] fileBytes)
        {
            // Ensure the target path ends with "_bin.bytes"
            if (!targetAssetPath.EndsWith("_bin.bytes"))
            {
                targetAssetPath = Path.ChangeExtension(targetAssetPath, null);
                if (!targetAssetPath.EndsWith("_bin"))
                {
                    targetAssetPath += "_bin";
                }
                targetAssetPath = Path.ChangeExtension(targetAssetPath, ".bytes");
            }

            File.WriteAllBytes(targetAssetPath, fileBytes);
            AssetDatabase.ImportAsset(targetAssetPath);
            AssetDatabase.SaveAssets();

            Debug.Log("Binary Asset saved: " + targetAssetPath);
            EditorUtility.DisplayDialog("Success", "Binary asset has been successfully saved!", "OK");
        }

        private void ZipDirectory(string sourceDir, string zipFilePath)
        {
            try
            {
                ZipFile.CreateFromDirectory(sourceDir, zipFilePath, System.IO.Compression.CompressionLevel.Optimal, includeBaseDirectory: true);
                Debug.Log("Folder successfully zipped: " + zipFilePath);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error zipping the folder: " + ex.Message);
                throw;
            }
        }
    }
}

using System.Collections.Generic;
using System.IO;
using EditorUIAdaptor;
using EditorUIAdaptor.Behaviours;
using GlobalSettings;
using Newtonsoft.Json;
using UI.InGameUI;
using UI.InGameUI.InfoWindow;
using UnityEngine;

namespace Save
{
    public static class SaveManager
    {
        private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "Datas");
        
        public static void SaveAll()
        {
            if (!Directory.Exists(SavePath)) Directory.CreateDirectory(SavePath);

            var editorWindows = new List<EditorWindowSaveData>();
            foreach (var editorWindowHandler in EditorWindowManager.Instance.GetAllWindows()) 
                editorWindows.Add(editorWindowHandler.SaveWindow());
            var editorWindowsPath = Path.Combine(SavePath, nameof(EditorWindowSaveData));
            File.WriteAllText(editorWindowsPath, JsonConvert.SerializeObject(editorWindows, Formatting.Indented));

            var infoWindows = new List<InfoWindowSaveData>();
            foreach (var infoWindowHandler in InfoWindowManager.Instance.GetAllWindows())
                infoWindows.Add(infoWindowHandler.SaveWindow());
            var infoWindowsPath = Path.Combine(SavePath, nameof(InfoWindowSaveData));
            File.WriteAllText(infoWindowsPath, JsonConvert.SerializeObject(infoWindows, Formatting.Indented));
            
            var globalInfos = JsonConvert.SerializeObject(GlobalInfos.Instance, Formatting.Indented);
            var globalInfosPath = Path.Combine(SavePath, nameof(GlobalInfos));
            File.WriteAllText(globalInfosPath, globalInfos);
        }

        public static void LoadAll()
        {
            // TODO:: 要先删除所有已有的窗口再根据json创建
        }
    }
}
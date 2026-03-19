using System.Diagnostics;
using Michsky.MUIP;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace UI.MenuUI.Buttons
{
    public class OpenSaveButton : MonoBehaviour
    { 
        [SerializeField] private NotificationManager notificationManager;
        public string MessageFormatTemplate { get; set; }
        private string TargetFolderPath => Application.persistentDataPath;

        public void OnClicked() { if (!TryOpenFolder(TargetFolderPath)) ShowFallbackUI(TargetFolderPath); }
        
        private static bool TryOpenFolder(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            
            try
            {
#if UNITY_STANDALONE_WIN
                Process.Start("explorer.exe", $"\"{path.Replace("/", "\\")}\"");
                return true;
#elif UNITY_STANDALONE_OSX
                Process.Start("open", $"\"{path}\"");
                return true;
#elif UNITY_STANDALONE_LINUX
                Process.Start("xdg-open", $"\"{path}\"");
                return true;
#else
                return false;
#endif
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"尝试打开文件夹失败: {e.Message}");
                return false;
            }
        }
        
        private void ShowFallbackUI(string path)
        {
            GUIUtility.systemCopyBuffer = path;

            notificationManager.Close();
#if UNITY_STANDALONE_WIN
            var msg = string.Format(MessageFormatTemplate, path.Replace("/", "\\"));
#else
            var msg = string.Format(MessageFormatTemplate, path);
#endif
            notificationManager.description = msg;
            notificationManager.UpdateUI();
            notificationManager.Open();
        }
    }
}
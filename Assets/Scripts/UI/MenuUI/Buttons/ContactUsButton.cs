using Michsky.MUIP;
using UnityEngine;

namespace UI.MenuUI.Buttons
{
    public class ContactUsButton : MonoBehaviour
    {
        [SerializeField] private NotificationManager notificationManager;
        public string NotifyContext { get; set; }
        
        public void OnClick()
        {
            Application.OpenURL("mailto:ChensenCHX@gmail.com");
            ShowMessage();
        }
        
        private void ShowMessage()
        {
            GUIUtility.systemCopyBuffer = "ChensenCHX@gmail.com";
            notificationManager.Close();
            notificationManager.description = NotifyContext;
            notificationManager.UpdateUI();
            notificationManager.Open();
        }
    }
}

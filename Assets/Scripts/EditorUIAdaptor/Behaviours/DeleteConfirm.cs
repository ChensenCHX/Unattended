using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace EditorUIAdaptor.Behaviours
{
    public class DeleteConfirm : SingletonMono<DeleteConfirm>
    {
        [SerializeField] private Button rejectButton;
        [SerializeField] private Button confirmButton;
        [SerializeField] private TextMeshProUGUI notifyText;
        
        public string TextFormatTemplate { get; set; }
        private void Start() => gameObject.SetActive(false);
        public void PopDeleteConfirm(string scriptName, UnityAction confirmCallback)
        {
            gameObject.SetActive(true);
            notifyText.text = string.Format(TextFormatTemplate, scriptName);
            EditorWindowManager.Instance.LockAllWindowsFocus();
            
            UnityAction confirmAction = null;
            UnityAction rejectAction = null;
            confirmAction = () => {
                confirmCallback();
                confirmButton.onClick.RemoveListener(confirmAction);
                rejectButton.onClick.RemoveListener(rejectAction);
                gameObject.SetActive(false);
                EditorWindowManager.Instance.UnlockAllWindowsFocus();
                EventSystem.current.SetSelectedGameObject(UIBGMouseListener.Instance.gameObject);
            };
            rejectAction = () =>
            {
                confirmButton.onClick.RemoveListener(confirmAction);
                rejectButton.onClick.RemoveListener(rejectAction);
                gameObject.SetActive(false);
                EditorWindowManager.Instance.UnlockAllWindowsFocus();
                EventSystem.current.SetSelectedGameObject(UIBGMouseListener.Instance.gameObject);
            };
            
            confirmButton.onClick.AddListener(confirmAction);
            rejectButton.onClick.AddListener(rejectAction);
        }
    }
}

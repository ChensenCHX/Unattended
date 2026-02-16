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
        private void Start() => gameObject.SetActive(false);
        public void PopDeleteConfirm(string scriptName, UnityAction confirmCallback)
        {
            gameObject.SetActive(true);
            notifyText.text = $" 真的要删除脚本 \"{scriptName}\" 吗？\n它将会被移动到回收站文件夹";
            UnityAction confirmAction = null;
            confirmAction = () => {
                confirmCallback();
                confirmButton.onClick.RemoveListener(confirmAction);
                gameObject.SetActive(false);
                EventSystem.current.SetSelectedGameObject(UIBGMouseListener.Instance.gameObject);
            };
            UnityAction rejectAction = null;
            rejectAction = () =>
            {
                confirmButton.onClick.RemoveListener(confirmAction);
                rejectButton.onClick.AddListener(rejectAction);
                gameObject.SetActive(false);
                EventSystem.current.SetSelectedGameObject(UIBGMouseListener.Instance.gameObject);
            };
            
            confirmButton.onClick.AddListener(confirmAction);
            rejectButton.onClick.AddListener(rejectAction);
        }
    }
}

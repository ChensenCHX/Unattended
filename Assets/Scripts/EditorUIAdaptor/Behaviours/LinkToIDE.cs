using System;
using CodeExecutor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EditorUIAdaptor.Behaviours
{
    public class LinkToIDE : MonoBehaviour
    {
        public EditorWindowHandler windowHandler;
        public Button closeButton;
        public Image icon;
        public Sprite disableSprite;
        public Sprite enableSprite;

        private void OnEnableThis()
        {
            EventSystem.current.SetSelectedGameObject(UIBGMouseListener.Instance.gameObject);
            icon.sprite = enableSprite;
            closeButton.interactable = false;
            windowHandler.GetTextEditor().DisableInput = true;
            CodeService.Instance.StartListeningOutsideChange(windowHandler);
        }
        private void OnDisableThis()
        {
            EventSystem.current.SetSelectedGameObject(UIBGMouseListener.Instance.gameObject);
            icon.sprite = disableSprite;
            closeButton.interactable = true;
            windowHandler.GetTextEditor().DisableInput = false;
            CodeService.Instance.StopListeningOutsideChange(windowHandler);
        }
        
        public void OnStatuChanged(bool statu) { if (statu) OnEnableThis(); else OnDisableThis(); }
    }
}

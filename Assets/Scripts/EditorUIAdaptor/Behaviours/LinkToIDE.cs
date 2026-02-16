using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EditorUIAdaptor.Behaviours
{
    public class LinkToIDE : MonoBehaviour
    {
        public ScriptNameAdjustor scriptName;
        public Button closeButton;
        public Image icon;
        public Sprite disableSprite;
        public Sprite enableSprite;

        private void OnEnableThis()
        {
            EventSystem.current.SetSelectedGameObject(UIBGMouseListener.Instance.gameObject);
            icon.sprite = enableSprite;
            closeButton.interactable = false;
            // TODO:: send message to code service
        }
        private void OnDisableThis()
        {
            EventSystem.current.SetSelectedGameObject(UIBGMouseListener.Instance.gameObject);
            icon.sprite = disableSprite;
            closeButton.interactable = true;
            // TODO:: send message to code service
        }
        
        public void OnStatuChanged(bool statu) { if (statu) OnEnableThis(); else OnDisableThis(); }
    }
}

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EditorUIAdaptor.Behaviours
{
    public class LinkToIDE : MonoBehaviour
    {
        public ScriptNameAdjustor scriptName;
        public Image Icon;
        public Sprite disableSprite;
        public Sprite enableSprite;

        private void OnEnableThis()
        {
            EventSystem.current.SetSelectedGameObject(UIBGMouseListener.Instance.gameObject);
            Icon.sprite = enableSprite;
            // TODO:: send message to code service
        }
        private void OnDisableThis()
        {
            EventSystem.current.SetSelectedGameObject(UIBGMouseListener.Instance.gameObject);
            Icon.sprite = disableSprite;
            // TODO:: send message to code service
        }
        
        public void OnStatuChanged(bool statu) { if (statu) OnEnableThis(); else OnDisableThis(); }
    }
}

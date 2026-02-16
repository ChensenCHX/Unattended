using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EditorUIAdaptor.Behaviours
{
    public class MinimalEditor : MonoBehaviour
    {
        public WindowAdjustor windowAdjustor;
        public GameObject draggerRight;
        public GameObject draggerBottom;
        public GameObject draggerCorner;
        public GameObject codeZone;
        public Image icon;
        public Sprite disableSprite;
        public Sprite enableSprite;
        
        private Vector2 memoriedSize;
        private void OnEnableThis()
        {
            EventSystem.current.SetSelectedGameObject(UIBGMouseListener.Instance.gameObject);
            memoriedSize = windowAdjustor.mainWindow.sizeDelta;
            draggerRight.SetActive(false);
            draggerBottom.SetActive(false);
            draggerCorner.SetActive(false);
            codeZone.SetActive(false);
            icon.sprite = enableSprite;
            windowAdjustor.TryResizeWindow(0, 0, true);
        }
        private void OnDisableThis()
        {
            EventSystem.current.SetSelectedGameObject(UIBGMouseListener.Instance.gameObject);
            memoriedSize = windowAdjustor.mainWindow.sizeDelta;
            draggerRight.SetActive(true);
            draggerBottom.SetActive(true);
            draggerCorner.SetActive(true);
            codeZone.SetActive(true);
            icon.sprite = disableSprite;
            windowAdjustor.TryResizeWindow(memoriedSize.x, memoriedSize.y);
        }
        
        public void OnStatuChanged(bool statu) { if (statu) OnEnableThis(); else OnDisableThis(); }
    }
}

using EditorUIAdaptor.Behaviours;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.InGameUI.InfoWindow
{
    public class MinimalButton : MonoBehaviour
    {
        [SerializeField] private RectTransform windowRectTransform;
        [SerializeField] private GameObject draggerRight;
        [SerializeField] private GameObject draggerBottom;
        [SerializeField] private GameObject draggerCorner;
        [SerializeField] private GameObject markdownZone;
        [SerializeField] private Image icon;
        [SerializeField] private Sprite disableSprite;
        [SerializeField] private Sprite enableSprite;

        private Vector2 oldSizeDelta;
        private static readonly Vector2 minimalSizeDelta = new Vector2(480f, 42f);
        private void OnEnableThis()
        {
            draggerRight.SetActive(false);
            draggerBottom.SetActive(false);
            draggerCorner.SetActive(false);
            markdownZone.SetActive(false);
            icon.sprite = enableSprite;
            oldSizeDelta = windowRectTransform.sizeDelta;
            windowRectTransform.sizeDelta = minimalSizeDelta;
            EventSystem.current.SetSelectedGameObject(UIBGMouseListener.Instance.gameObject);
        }
        private void OnDisableThis()
        {
            draggerRight.SetActive(true);
            draggerBottom.SetActive(true);
            draggerCorner.SetActive(true);
            markdownZone.SetActive(true);
            icon.sprite = disableSprite;
            windowRectTransform.sizeDelta = oldSizeDelta;
            EventSystem.current.SetSelectedGameObject(UIBGMouseListener.Instance.gameObject);
        }
        
        public void OnStatuChanged(bool statu) { if (statu) OnEnableThis(); else OnDisableThis(); }
    }
}

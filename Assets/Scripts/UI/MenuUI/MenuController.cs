using Save;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.MenuUI
{
    public class MenuController : SingletonMono<MenuController>
    {
        [SerializeField] private GameObject inGameUIOverlayObject;
        [SerializeField] private GameObject worldUICanvasObject;
        [SerializeField] private GameObject screenUICanvasObject;
        [SerializeField] private GameObject menuUIRootObject;
        
        [Header("Inner State References")]
        [SerializeField] private Toggle switchUIButton;
        
        private bool isActive;
        private void SetActive(bool active, bool isNotFirst=true)
        {
            menuUIRootObject.SetActive(active);
            inGameUIOverlayObject.SetActive(!active);
            worldUICanvasObject.SetActive(!active);
            screenUICanvasObject.SetActive(!active);
            isActive = active;
            if (active && isNotFirst) SaveManager.SaveAll();
        } 
        public void SwitchState() => SetActive(!isActive);
        
        private void Start()
        {
            SetActive(true, false);
        }
        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            if (switchUIButton.isOn) { switchUIButton.isOn = false; return; }
            SetActive(!isActive);
        }
    }
}

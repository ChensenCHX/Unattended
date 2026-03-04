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
        private void SetActive(bool active)
        {
            inGameUIOverlayObject.SetActive(active);
            worldUICanvasObject.SetActive(active);
            screenUICanvasObject.SetActive(active);
            menuUIRootObject.SetActive(!active);
            isActive = active;
        } 
        
        private void Start() => SetActive(false);
        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            if (switchUIButton.isOn) { switchUIButton.isOn = false; return; }
            SetActive(!isActive);
        }
    }
}

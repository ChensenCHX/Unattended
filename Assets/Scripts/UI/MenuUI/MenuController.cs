using System.Collections.Generic;
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
        [SerializeField] private List<GameObject> menuPages;
        
        private HistoryStack<GameObject> historyStack;
        
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

        public void PushPage(int pageIndex)
        {
            if  (pageIndex < 0 || pageIndex >= menuPages.Count) throw new System.IndexOutOfRangeException();
            historyStack.Peek().SetActive(false);
            historyStack.Push(menuPages[pageIndex]);
            historyStack.Peek().SetActive(true);
        }
        public int PopPage()
        {
            var obj = historyStack.Pop();
            obj.SetActive(false);
            historyStack.Peek().SetActive(true);
            return menuPages.IndexOf(obj);
        }
        
        protected override void OnAwake()
        {
            historyStack = new HistoryStack<GameObject>(menuPages[0]);
        }
        private void Start()
        {
            SetActive(true, false);
        }
        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            if (switchUIButton.isOn) { switchUIButton.isOn = false; return; }
            if (historyStack.Count != 0) { PopPage(); return; }

            SetActive(!isActive);
        }
    }
}

using UnityEngine;
using Utils;

namespace UI.MenuUI
{
    public class MenuController : SingletonMono<MenuController>
    {
        [SerializeField] private GameObject inGameUIOverlayObject;
        [SerializeField] private GameObject worldUICanvasObject;
        [SerializeField] private GameObject screenUICanvasObject;
        
        private void SetActive(bool active)
        {
            inGameUIOverlayObject.SetActive(active);
            worldUICanvasObject.SetActive(active);
            screenUICanvasObject.SetActive(active);
            gameObject.SetActive(!active);
        } 
        private void Start() => SetActive(false);
    }
}

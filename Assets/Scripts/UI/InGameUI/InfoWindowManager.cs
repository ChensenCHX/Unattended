using System.Collections.Generic;
using EditorUIAdaptor.Behaviours;
using UI.InGameUI.InfoWindow;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace UI.InGameUI
{
    public class InfoWindowManager : SingletonMono<InfoWindowManager>
    {
        public GameObject InfoWindowPrefab; 
        public RectTransform RectTransform;
        
        private readonly List<InfoWindowHandler> windowHandlers = new();
        
        public void CreateWindow()
        {
            var screenPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                InfoWindowManager.Instance.RectTransform,
                screenPoint,
                WindowCamera.Instance.Camera,
                out var localPoint
            );

            var windowObj = Instantiate(InfoWindowPrefab, InfoWindowManager.Instance.transform);
            var rectTransform = windowObj.GetComponent<RectTransform>();
            windowHandlers.Add(windowObj.GetComponent<InfoWindowHandler>());
            rectTransform.anchoredPosition = localPoint;
            EventSystem.current.SetSelectedGameObject(UIBGMouseListener.Instance.gameObject);
        }
        public void DeleteWindow(InfoWindowHandler window)
        {
            windowHandlers.Remove(window);
            Destroy(window.gameObject);
        }
    }
}
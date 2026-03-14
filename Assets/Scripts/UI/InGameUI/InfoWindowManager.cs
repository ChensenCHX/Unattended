using System;
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
        
        public void CreateWindow(string chapterName="main.zip", float x=float.NaN, float y=float.NaN, float width=float.NaN, float height=float.NaN)
        {
            Vector2 localPoint;
            if (float.IsNaN(x) || float.IsNaN(y))
            {
                var screenPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    InfoWindowManager.Instance.RectTransform,
                    screenPoint,
                    WindowCamera.Instance.Camera,
                    out localPoint
                );
            }
            else
            {
                localPoint = new Vector2(x, y);
            }

            var windowObj = Instantiate(InfoWindowPrefab, InfoWindowManager.Instance.transform);
            var rectTransform = windowObj.GetComponent<RectTransform>();
            var infoWindowHandler = windowObj.GetComponent<InfoWindowHandler>();
            windowHandlers.Add(infoWindowHandler);
            rectTransform.anchoredPosition = localPoint;
            if (!float.IsNaN(width) && !float.IsNaN(height)) rectTransform.sizeDelta = new Vector2(width, height);
            infoWindowHandler.Init(chapterName);
            EventSystem.current.SetSelectedGameObject(UIBGMouseListener.Instance.gameObject);
        }
        public void CreateWindow(InfoWindowSaveData data) => CreateWindow(data.CurrentChapter, data.X, data.Y, data.Width, data.Height);
        
        public void RemoveWindow(InfoWindowHandler window)
        {
            windowHandlers.Remove(window);
            Destroy(window.gameObject);
        }
        public void RemoveAllWindow()
        {
            windowHandlers.ForEach(handler => Destroy(handler.gameObject));
            windowHandlers.Clear();
        }
        
        public IReadOnlyList<InfoWindowHandler> GetAllWindows() => windowHandlers;
    }
}
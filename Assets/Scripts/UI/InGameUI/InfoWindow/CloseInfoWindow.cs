using UnityEngine;

namespace UI.InGameUI.InfoWindow
{
    public class CloseInfoWindow : MonoBehaviour
    {
        [SerializeField] private InfoWindowHandler windowHandler;
        
        public void OnClickThis() => InfoWindowManager.Instance.RemoveWindow(windowHandler);
    }
}
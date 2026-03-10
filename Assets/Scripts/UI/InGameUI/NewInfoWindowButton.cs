using UnityEngine;

namespace UI.InGameUI
{
    public class NewInfoWindow : MonoBehaviour
    {
        public void CreateNewWindow() => InfoWindowManager.Instance.CreateWindow();
    }
}
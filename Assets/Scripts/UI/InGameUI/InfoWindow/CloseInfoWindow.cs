using UnityEngine;

namespace UI.InGameUI.InfoWindow
{
    public class CloseInfoWindow : MonoBehaviour
    {
        [SerializeField] private GameObject windowObject;
        
        public void OnClickThis() => Destroy(windowObject);
    }
}
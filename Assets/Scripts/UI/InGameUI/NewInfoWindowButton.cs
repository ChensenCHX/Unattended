using EditorUIAdaptor;
using EditorUIAdaptor.Behaviours;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.InGameUI
{
    public class NewInfoWindow : MonoBehaviour
    {
        [SerializeField] private GameObject infoWindowPrefab;
        
        public void CreateNewWindow()
        {
            var screenPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                EditorWindowManager.Instance.RectTransform,
                screenPoint,
                WindowCamera.Instance.Camera,
                out var localPoint
            );

            var windowObj = Instantiate(infoWindowPrefab, EditorWindowManager.Instance.transform);
            var rectTransform = windowObj.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = localPoint;
            EventSystem.current.SetSelectedGameObject(UIBGMouseListener.Instance.gameObject);
        }
    }
}
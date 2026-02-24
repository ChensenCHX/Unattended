using EditorUIAdaptor;
using EditorUIAdaptor.Behaviours;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.InGameUI
{
    public class NewCodeEditorWindow : MonoBehaviour
    {
        public void CreateNewWindow()
        {
            var screenPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                EditorWindowManager.Instance.RectTransform,
                screenPoint,
                WindowCamera.Instance.Camera,
                out var localPoint
            );
            
            EditorWindowManager.Instance.CreateEditorWindow(null, null, localPoint.x, localPoint.y);
            EventSystem.current.SetSelectedGameObject(UIBGMouseListener.Instance.gameObject);
        }
    }
}

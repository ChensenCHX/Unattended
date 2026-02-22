using EditorUIAdaptor;
using UnityEngine;

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
                null,
                out var localPoint
            );
            
            EditorWindowManager.Instance.CreateEditorWindow(null, null, localPoint.x, localPoint.y);
        }
    }
}

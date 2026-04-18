using UnityEngine;

namespace UI.MenuUI.Buttons
{
    public class ExitGameButton : MonoBehaviour
    {
        public void OnClick()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}

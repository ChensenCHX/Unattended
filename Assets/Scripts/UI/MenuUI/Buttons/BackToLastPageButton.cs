using UnityEngine;

namespace UI.MenuUI.Buttons
{
    public class BackToLastPageButton : MonoBehaviour
    {
        public void OnClick() => MenuController.Instance.PopPage();
    }
}

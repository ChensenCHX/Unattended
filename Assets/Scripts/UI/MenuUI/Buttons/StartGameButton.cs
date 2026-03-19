using UnityEngine;

namespace UI.MenuUI.Buttons
{
    public class StartGameButton : MonoBehaviour
    {
        public void OnClick() => MenuController.Instance.SwitchState();
    }
}
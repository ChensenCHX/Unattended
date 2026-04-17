using UnityEngine;

namespace UI.MenuUI.Buttons
{
    public class SettingButton : MonoBehaviour
    {
        [SerializeField] private GameObject pageObj;
        public void OnClick() => MenuController.Instance.PushPage(MenuController.Instance.FindPage(pageObj));
    }
}

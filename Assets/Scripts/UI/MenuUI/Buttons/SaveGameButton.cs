using Save;
using UnityEngine;

namespace UI.MenuUI.Buttons
{
    public class SaveGameButton : MonoBehaviour
    {
        public void OnClick() => SaveManager.SaveAll();
    }
}
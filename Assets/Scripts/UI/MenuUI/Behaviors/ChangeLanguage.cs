using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace UI.MenuUI.Behaviors
{
    public class ChangeLanguage : MonoBehaviour
    {
        public Locale ChineseLocal;
        public Locale EnglishLocal;
        public Locale JapaneseLocal;

        public void ToChinese()
        {
            LocalizationSettings.Instance.SetSelectedLocale(ChineseLocal);
            Debug.Log("Chinese");
        }
        public void ToEnglish()
        {
            LocalizationSettings.Instance.SetSelectedLocale(EnglishLocal);
            Debug.Log("English");
        }
        public void ToJapanese()
        {
            LocalizationSettings.Instance.SetSelectedLocale(JapaneseLocal);
            Debug.Log("Japanese");
        }
    }
}
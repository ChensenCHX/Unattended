using System.Linq;
using System.Reflection;
using System.Text;
using CodeExecutor;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace EditorUIAdaptor.Behaviours
{
    public static class TMPInputFieldHack
    {
        private static readonly FieldInfo caretField;

        static TMPInputFieldHack()
        {
            caretField = typeof(TMP_InputField)
                .GetField("caretRectTrans", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// FUCK TMP_InputField,,,,,,
        public static RectTransform GetCaretRect(TMP_InputField input)
        {
            if (caretField == null) return null;
            return caretField.GetValue(input) as RectTransform;
        }
    }
    
    public class ScriptNameAdjustor : MonoBehaviour
    {
        private const float MIN_WIDTH = 32f;
        private const float MAX_WIDTH = 256f;
    
        public RectTransform scriptNameHolder;
        public TextMeshProUGUI scriptNameText;
        public TMP_InputField scriptNameInput;
        
        private string previousValidName;
        public string ScriptName => previousValidName;
        private static readonly string[] reservedNames = new string[]
        {
            "CON", "PRN", "AUX", "NUL",
            "COM0", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
            "LPT0", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
        };
        
        public void Init(string scriptName=null)
        {
            previousValidName = scriptName ?? CodeService.Instance.GetSafeFileName();
            scriptNameInput.text = previousValidName;
            AdjustWidth(previousValidName);
            
            // TODO:: notify code service here
            
            scriptNameInput.onValueChanged.AddListener(OnValueChanged);
            scriptNameInput.onEndEdit.AddListener(OnEndEdit);
        }

        private void OnValueChanged(string value)
        {
            var filtered = FilterToValidFileName(value);
            if (filtered != value) scriptNameInput.SetTextWithoutNotify(filtered); 
            AdjustWidth(filtered);
        }
        
        private void OnEndEdit(string currentText)
        {
            if (currentText == previousValidName) return;
            if (string.IsNullOrEmpty(currentText)
                || !IsValidFileName(currentText)
                || CodeService.Instance.CheckScriptExist(currentText))
            {
                scriptNameInput.SetTextWithoutNotify(previousValidName); 
                AdjustWidth(previousValidName);
                return;
            }

            CodeService.Instance.RenameExistScript(previousValidName, currentText);
            previousValidName = currentText;
            AdjustWidth(previousValidName);
        }
        
        private static string FilterToValidFileName(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            var sb = new StringBuilder(input.Length);
            foreach (var c in input.Where(
                         c => c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or >= '0' and <= '9' or '_'))
                sb.Append(c);
            
            return sb.ToString();
        }
        private void AdjustWidth(string newName)
        {
            scriptNameHolder.sizeDelta = new Vector2(
                Mathf.Clamp(scriptNameText.GetPreferredValues(newName).x + 5f, MIN_WIDTH, MAX_WIDTH),
                scriptNameHolder.sizeDelta.y);

            var rt = scriptNameInput.textComponent.rectTransform;
            rt.offsetMin = new Vector2(0, rt.offsetMin.y);   // 设置 Left
            rt.offsetMax = new Vector2(0, rt.offsetMax.y);  // 设置 Right
            rt = TMPInputFieldHack.GetCaretRect(scriptNameInput);
            rt.offsetMin = new Vector2(0, rt.offsetMin.y);   // 设置 Left
            rt.offsetMax = new Vector2(0, rt.offsetMax.y);  // 设置 Right
        }
        
        private static bool IsValidFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return false;
            var upperName = fileName.ToUpperInvariant();
            return reservedNames.All(reserved => !upperName.Equals(reserved));
        }
    }
}
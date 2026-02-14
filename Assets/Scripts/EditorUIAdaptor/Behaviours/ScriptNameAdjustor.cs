using System.Linq;
using CodeExecutor;
using TMPro;
using UnityEngine;

namespace EditorUIAdaptor
{
    public class ScriptNameAdjustor : MonoBehaviour
    {
        private const float MIN_WIDTH = 32f;
        private const float MAX_WIDTH = 254f;
    
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
            
            scriptNameInput.onValidateInput = ValidateChar;
            scriptNameInput.onEndEdit.AddListener(OnEndEdit);
            scriptNameInput.onValueChanged.AddListener(AdjustWidth);
        }
        
        private void OnEndEdit(string currentText)
        {
            if (currentText == previousValidName) return;
            if (string.IsNullOrEmpty(currentText)
                || !IsValidFileName(currentText)
                || CodeService.Instance.CheckScriptExist(currentText))
            {
                scriptNameInput.text = previousValidName; return;
            }

            CodeService.Instance.RenameExistScript(previousValidName, currentText);
            previousValidName = currentText;
        }
        private static char ValidateChar(string text, int charIndex, char addedChar)
        {
            return (char.IsLetterOrDigit(addedChar) || addedChar == '_') ? addedChar : '\0';
        }
        private void AdjustWidth(string newName)
        {
            scriptNameHolder.sizeDelta = new Vector2(
                Mathf.Clamp(scriptNameText.GetPreferredValues(newName).x + 5f, MIN_WIDTH, MAX_WIDTH),
                scriptNameHolder.sizeDelta.y);
        }
        
        private static bool IsValidFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return false;
            var upperName = fileName.ToUpperInvariant();
            return reservedNames.All(reserved => !upperName.Equals(reserved));
        }
    }
}
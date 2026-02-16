using UnityEngine;
using UnityEngine.UI;

namespace EditorUIAdaptor.Behaviours
{
    public class CloseEditorWindow : MonoBehaviour
    {
        public ScriptNameAdjustor scriptName;

        public void OnClickThis() => DeleteConfirm.Instance.PopDeleteConfirm(scriptName.ScriptName, ConfirmCallback);
        private void ConfirmCallback() => EditorWindowManager.Instance.RemoveEditorWindow(scriptName.ScriptName);
    }
}

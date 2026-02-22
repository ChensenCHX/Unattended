using EditorUIAdaptor;
using UnityEngine;

namespace UI.InGameUI
{
    public class SwitchUIButton : MonoBehaviour
    {
        [SerializeField] private GameObject newCodeEditorButtonObj;
        [SerializeField] private GameObject newInfoWindowButtonObj;
        [SerializeField] private GameObject researchPanelObj;
        
        private void OnEnableThis()
        {
            EditorWindowManager.Instance.LockAllWindowsFocus();
            newCodeEditorButtonObj.SetActive(false);
            newInfoWindowButtonObj.SetActive(false);
            researchPanelObj.SetActive(true);
        }
        private void OnDisableThis()
        {
            EditorWindowManager.Instance.UnlockAllWindowsFocus();
            newCodeEditorButtonObj.SetActive(true);
            newInfoWindowButtonObj.SetActive(true);
            researchPanelObj.SetActive(false);
        }
        
        public void OnStatuChanged(bool statu) { if (statu) OnEnableThis(); else OnDisableThis(); }
    }
}

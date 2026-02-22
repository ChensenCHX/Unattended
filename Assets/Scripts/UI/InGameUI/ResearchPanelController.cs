using Utils;

namespace UI.InGameUI
{
    public class ResearchPanelController : SingletonMono<ResearchPanelController>
    {
        private void Start() => gameObject.SetActive(false);
    }
}

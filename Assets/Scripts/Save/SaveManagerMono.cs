using Utils;

namespace Save
{
    public class SaveManagerMono : SingletonMono<SaveManagerMono>
    {
        private void Start() => SaveManager.LoadAll();
        private void OnApplicationQuit() => SaveManager.SaveAll();
    }
}
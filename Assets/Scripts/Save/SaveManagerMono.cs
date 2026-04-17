using Utils;

namespace Save
{
    public class SaveManagerMono : SingletonMono<SaveManagerMono>
    {
        private void Start() => SaveManager.LoadAll();
        private void OnApplicationQuit() => SaveManager.SaveAll();

        private void Save() => SaveManager.SaveAll();
        public void SetAutoSave(float repeatTime)
        {
            CancelInvoke();
            InvokeRepeating(nameof(Save), repeatTime, repeatTime);
        }
    }
}
using System;
using Save;
using TMPro;
using UnityEngine;

namespace UI.MenuUI.Behaviors
{
    public class AutoSaveSetting : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI infoText;
        
        private string _infText = "Never";
        private int currVal = 0;
        public string InfText { get => _infText; set { _infText = value; if (currVal == 4) infoText.text = value; } }

        public void OnValueChange(float value)
        {
            var val = Mathf.RoundToInt(value);
            currVal = val;
            switch (val)
            {
                case 0: SaveManagerMono.Instance.SetAutoSave(60f * 5f); infoText.text = "5 min"; break;
                case 1: SaveManagerMono.Instance.SetAutoSave(60f * 10f); infoText.text = "10 min"; break;
                case 2: SaveManagerMono.Instance.SetAutoSave(60f * 30f); infoText.text = "30 min"; break;
                case 3: SaveManagerMono.Instance.SetAutoSave(60f * 60f); infoText.text = "60 min"; break;
                case 4: SaveManagerMono.Instance.SetAutoSave(float.MaxValue); infoText.text = InfText; break;
                default: throw new ArgumentOutOfRangeException(nameof(value), value, "should in 0~4.");
            }
        }
    }
}

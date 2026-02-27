using System;
using System.Collections.ObjectModel;
using GlobalSettings;
using Items;
using Michsky.MUIP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.InGameUI
{
    public class ItemCountRenderer : MonoBehaviour
    {
        [SerializeField] private ItemType itemType;
        [SerializeField] private Sprite itemSprite;
        [SerializeField] private TextMeshProUGUI itemCount;
        [SerializeField] private Image itemImage;
        [SerializeField] private TooltipContent tooltipContent;

        public string TooltipText { set => tooltipContent.description = value; }
        
        private static readonly ReadOnlyCollection<string> suffixes = new(new [] { "", "K", "M", "G", "T", "P", "E", "Z", "Y", "R", "Q" });
        private static string ToMetricString(double value)
        {
            var divisor = 1.0;
            var suffixIndex = 0;

            var absValue = Math.Abs(value);
            while (absValue >= 1000 && suffixIndex < suffixes.Count - 1)
            {
                divisor *= 1000;
                suffixIndex++;
                absValue /= 1000;
            }

            var roundedValue = value / divisor;
            return absValue >= 100 ? $"{roundedValue:F1}{suffixes[suffixIndex]}" : 
                absValue >= 10 ? $"{roundedValue:F2}{suffixes[suffixIndex]}" 
                : $"{roundedValue:F3}{suffixes[suffixIndex]}";
        }
        private void UpdateCount(double count) => itemCount.text = ToMetricString(count);

        private void Awake() 
        {
            UpdateCount(GlobalInfos.Instance.GetItemCountByType(itemType));
            itemImage.sprite = itemSprite;
            GlobalInfos.Instance.SubscribeItemCountEventByType(itemType, UpdateCount);
        }
        private void OnDestroy() => GlobalInfos.Instance.UnsubscribeItemCountEventByType(itemType, UpdateCount);
    }
}

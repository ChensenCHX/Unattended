using System;
using System.Collections.Generic;
using System.Linq;
using GlobalSettings;
using Items;
using UnityEngine;
using System.Reflection;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR
using Utils.UnityEditor;
#endif

namespace UI.InGameUI.UpgradePanel
{
    #region 序列化辅助
    
    [Serializable]
    public class TechCondition
    {
        [BoolMemberReference(typeof(GlobalInfos))]
        public string boolMemberName;
        
        private FieldInfo cachedField;
        private PropertyInfo cachedProperty;
        private bool cacheValid;

        // 读取
        public bool GetValue()
        {
            EnsureCache();
            var instance = GlobalInfos.Instance;
            if (instance == null) return false;

            if (cachedField != null)
                return (bool)cachedField.GetValue(instance);
            if (cachedProperty != null)
                return (bool)cachedProperty.GetValue(instance);
            return false;
        }

        // 写入
        public void SetValue(bool value)
        {
            EnsureCache();
            var instance = GlobalInfos.Instance;
            if (instance == null)
            {
                Debug.LogError("GlobalInfos.Instance 为空，无法写入值。");
                return;
            }

            if (cachedField != null)
            {
                cachedField.SetValue(instance, value);
            }
            else if (cachedProperty != null)
            {
                if (cachedProperty.CanWrite)
                {
                    cachedProperty.SetValue(instance, value, null);
                }
                else
                {
                    Debug.LogWarning($"属性 '{boolMemberName}' 没有 setter，无法写入。");
                }
            }
            else
            {
                Debug.LogWarning($"未找到bool成员 '{boolMemberName}'，无法写入。");
            }
        }

        // 初始化
        private void EnsureCache()
        {
            if (cacheValid) return;
            cacheValid = true;

            Type type = typeof(GlobalInfos);
            // 优先字段
            cachedField = type.GetField(boolMemberName, BindingFlags.Public | BindingFlags.Instance);
            if (cachedField != null && cachedField.FieldType == typeof(bool))
                return;

            // 再试属性
            cachedProperty = type.GetProperty(boolMemberName, BindingFlags.Public | BindingFlags.Instance);
            if (cachedProperty != null && cachedProperty.PropertyType == typeof(bool) && cachedProperty.CanRead)
                return;

            // 都没找到
            cachedField = null;
            cachedProperty = null;
            Debug.LogWarning($"在 {type.Name} 中未找到名为 '{boolMemberName}' 的 public bool 字段或可读属性。");
        }

        // 编辑器改名后重置缓存
        public void ResetCache()
        {
            cacheValid = false;
            cachedField = null;
            cachedProperty = null;
        }
    }
    
    [Serializable]
    public struct ItemCostPair
    {
        public ItemType type;
        public int value;
        public ItemCostPair(ItemType type, int value) 
        { 
            this.type = type;
            this.value = value;
        }
    }
    
    #endregion

    public class UnlockableNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("触发条件与代价")]
        [SerializeField] private TechCondition thisCondition = new();
        [SerializeField] private List<TechCondition> dependentConditions = new();
        [SerializeField] private List<ItemCostPair> costPairs = new();
        
        [Header("构成对象")]
        [SerializeField] private Image background;
        
        [Header("状态颜色")]
        [SerializeField] private Color achievedColor = new(0.85f, 0.74f, 0.39f, 1.00f);
        [SerializeField] private Color pendingColor = new(0.36f, 0.43f, 0.05f, 1.00f);
        [SerializeField] private Color lockedColor = Color.gray;
        
        private enum NodeState { Locked, Pending, Achieved }
        private NodeState CurrentState => thisCondition.GetValue() switch
        {
            true => NodeState.Achieved,
            false => dependentConditions.All(condition => condition.GetValue()) ? NodeState.Pending : NodeState.Locked,
        };
        private Color CurrentColor => CurrentState switch
        {
            NodeState.Locked => lockedColor,
            NodeState.Pending => pendingColor,
            NodeState.Achieved => achievedColor,
            _ => throw new ArgumentOutOfRangeException()
        };
        private bool isPointerInNode = false;

        private static readonly Color highlightColorAddon = new Color(0.05f, 0.05f, 0.05f, 1.00f);

        public void OnEnable() => background.color = CurrentColor;

        public void OnPointerEnter(PointerEventData eventData) { isPointerInNode = true; background.color = CurrentColor + highlightColorAddon; }
        public void OnPointerExit(PointerEventData eventData) { isPointerInNode = false; background.color = CurrentColor; }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (CurrentState == NodeState.Achieved) return;
            if (dependentConditions.Any(condition => !condition.GetValue())) return;
            var solved = true;
            costPairs.ForEach(cost => solved &= (GlobalInfos.Instance.GetItemCountByType(cost.type) >= cost.value));
            if (!solved) return;
            costPairs.ForEach(cost => GlobalInfos.Instance.TryConsumeItem(cost.type, cost.value));
            thisCondition.SetValue(true);
            var currentColor = CurrentColor;
            background.color = isPointerInNode ? currentColor + highlightColorAddon : currentColor;
        }
    }
}

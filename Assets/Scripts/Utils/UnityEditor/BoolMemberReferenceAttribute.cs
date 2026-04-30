using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
#endif

namespace Utils.UnityEditor
{
    public class BoolMemberReferenceAttribute : PropertyAttribute
    {
        public Type TargetType { get; }
        public BoolMemberReferenceAttribute(Type targetType) => TargetType = targetType;
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(BoolMemberReferenceAttribute))]
    public class BoolMemberReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.HelpBox(position, "BoolMemberReference can only be used on string fields.", MessageType.Error);
                return;
            }

            BoolMemberReferenceAttribute attr = (BoolMemberReferenceAttribute)attribute;
            Type targetType = attr.TargetType;

            // 收集所有 public 实例 bool 成员（字段 + 属性）
            List<string> memberNames = new List<string>();
            List<string> displayNames = new List<string>();
            
            FieldInfo[] fields = targetType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo fi in fields)
            {
                if (fi.FieldType == typeof(bool))
                {
                    memberNames.Add(fi.Name);
                    displayNames.Add($"{fi.Name}  (field)");
                }
            }

            PropertyInfo[] props = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo pi in props)
            {
                if (pi.PropertyType == typeof(bool) && pi.CanRead)
                {
                    // 避免与字段重名（若重名则字段优先）
                    if (!memberNames.Contains(pi.Name))
                    {
                        memberNames.Add(pi.Name);
                        displayNames.Add($"{pi.Name}  (property)");
                    }
                }
            }

            if (memberNames.Count == 0)
            {
                EditorGUI.HelpBox(position, $"No public bool fields/properties found in {targetType.Name}", MessageType.Warning);
                return;
            }

            string currentValue = property.stringValue;
            int currentIndex = memberNames.IndexOf(currentValue);
            if (currentIndex < 0) currentIndex = 0;

            // 绘制下拉菜单
            int newIndex = EditorGUI.Popup(position, label.text, currentIndex, displayNames.ToArray());
            property.stringValue = memberNames[newIndex];
        }
    }
#endif
}
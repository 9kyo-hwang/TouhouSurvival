using UnityEditor;
using UnityEngine;

namespace Unchord.Editor
{
    [CustomPropertyDrawer(typeof(SerializedGameplayAttributeBase))]
    public class SerializedGameplayAttributeBaseDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            SerializedProperty pKey = property.FindPropertyRelative("attributeName");
            SerializedProperty pVal = property.FindPropertyRelative("baseValue");

            float spaceUnit = rect.width / 16.0f;
            float fieldSpace = spaceUnit * 7.5f;
            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = fieldSpace / 2.0f;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, fieldSpace, rect.height), pKey, new GUIContent("Attribute Name"));
            EditorGUI.PropertyField(new Rect(rect.x + fieldSpace + spaceUnit, rect.y, fieldSpace, rect.height), pVal, new GUIContent("Base Value"));
            EditorGUIUtility.labelWidth = oldLabelWidth;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
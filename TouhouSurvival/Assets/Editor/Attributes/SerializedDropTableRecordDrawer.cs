using UnityEditor;
using UnityEngine;

namespace Unchord.Editor
{
    [CustomPropertyDrawer(typeof(SerializedDropTableRecord))]
    public class SerializedDropTableRecordDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            SerializedProperty pName = property.FindPropertyRelative("itemName");
            SerializedProperty pRatio = property.FindPropertyRelative("dropRatio01");

            float spaceUnit = rect.width / 16.0f;
            float fieldSpace = spaceUnit * 7.5f;
            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = fieldSpace / 2.0f;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, fieldSpace, rect.height), pName, new GUIContent("Item Name"));
            EditorGUI.PropertyField(new Rect(rect.x + fieldSpace + spaceUnit, rect.y, fieldSpace, rect.height), pRatio, new GUIContent("Drop Ratio (0~1)"));
            EditorGUIUtility.labelWidth = oldLabelWidth;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
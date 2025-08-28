using UnityEditor;
using UnityEngine;

namespace Unchord.Editor
{
    [CustomPropertyDrawer(typeof(SerializedLevelUpExp))]
    public class SerializedLevelUpExpDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect rect = new Rect(position.x, position.y, position.width, position.height);

            SerializedProperty pLev = property.FindPropertyRelative("nextLevel");
            SerializedProperty pReq = property.FindPropertyRelative("expRequirement");

            float spaceUnit = rect.width / 16.0f;
            float fieldSpace = spaceUnit * 7.5f;
            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = fieldSpace / 2.0f;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, fieldSpace, rect.height), pLev, new GUIContent("Next Level"));
            EditorGUI.PropertyField(new Rect(rect.x + fieldSpace + spaceUnit, rect.y, fieldSpace, rect.height), pReq, new GUIContent("Exp Requirement"));
            EditorGUIUtility.labelWidth = oldLabelWidth;

            EditorGUI.EndProperty();
        }
    }
}
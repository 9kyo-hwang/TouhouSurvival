using UnityEditor;
using UnityEngine;
using System;

namespace Unchord.Editor
{
    [CustomPropertyDrawer(typeof(SerializedGameplayAttributeModifier))]
    public class SerializedGameplayAttributeModifierDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            SerializedProperty pLevel = property.FindPropertyRelative("level");
            SerializedProperty pKey = property.FindPropertyRelative("attributeName");
            SerializedProperty pVal = property.FindPropertyRelative("value");
            SerializedProperty pOpcode = property.FindPropertyRelative("operationMode");
            SerializedProperty pDesc = property.FindPropertyRelative("description");

            GameplayAttributeOperator opcode;

            if (!Enum.TryParse<GameplayAttributeOperator>(pOpcode.stringValue, out opcode))
            {
                opcode = GameplayAttributeOperator.Flat;
            }

            string modifierSummary = GetPropertyLabel(pLevel.intValue, pKey.stringValue, opcode);

            property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, modifierSummary);

            if (!property.isExpanded)
            {
                return;
            }

            float indentDepth = 15.0f;
            float spaceUnit = (rect.width - indentDepth) / 16.0f;
            float fieldSpace4 = spaceUnit * 4.0f;
            float fieldSpace6 = spaceUnit * 6.0f;
            float oldLabelWidth = EditorGUIUtility.labelWidth;
            float height = EditorGUIUtility.singleLineHeight;
            float dy = 2.0f;
            float yOffset = height + dy;
            EditorGUIUtility.labelWidth = fieldSpace6 / 3.0f;

            Rect origin = new Rect(rect.x + indentDepth, rect.y + yOffset, fieldSpace6 - dy, height);
            EditorGUI.PropertyField(origin, pLevel, new GUIContent("Level for Applying"));

            origin.y += yOffset;
            EditorGUI.PropertyField(origin, pKey, new GUIContent("Attribute Name"));

            origin.y += yOffset;
            origin.width = fieldSpace4;
            EditorGUI.PropertyField(origin, pVal, new GUIContent("Value"));

            origin.x = rect.x + indentDepth + fieldSpace4 + dy;
            origin.width = fieldSpace6 - fieldSpace4 - dy * 2.0f;

            opcode = (GameplayAttributeOperator)EditorGUI.EnumPopup(origin, opcode);
            pOpcode.stringValue = opcode.ToString();

            origin.x = rect.x + indentDepth + fieldSpace6 + dy;
            origin.y = rect.y + yOffset;
            origin.width = EditorGUIUtility.labelWidth;
            EditorGUI.LabelField(origin, "Description");

            origin.x += EditorGUIUtility.labelWidth;
            origin.width = rect.width - indentDepth - fieldSpace6 - EditorGUIUtility.labelWidth - dy;
            origin.height = rect.height + yOffset * 2 - dy;
            pDesc.stringValue = EditorGUI.TextArea(origin, pDesc.stringValue);

            EditorGUIUtility.labelWidth = oldLabelWidth;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float dy = 2.0f;
            float yOffset = EditorGUIUtility.singleLineHeight + dy;

            if (property.isExpanded)
                return yOffset * 4.0f - dy;
            else
                return yOffset - dy;
        }

        private string GetPropertyLabel(int level, string attributeName, GameplayAttributeOperator opcode)
        {
            string unit = string.Empty;

            switch (opcode)
            {
                case GameplayAttributeOperator.Flat:
                    break;
                case GameplayAttributeOperator.PercentAdd:
                    unit = "%p";
                    break;
                case GameplayAttributeOperator.PercentMul:
                    unit = "%";
                    break;
                default:
                    UnityEngine.Debug.Assert(false, "invalid enum entered.");
                    break;
            }

            if (attributeName.Equals(string.Empty))
            {
                return $"Modifier on Level {level} (<undefined>)";
            }
            
            return $"Modifier on Level {level} ({attributeName}{unit})";
        }
    }
}
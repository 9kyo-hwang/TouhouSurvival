using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Unchord.Editor
{
    public abstract class UnchordEditorWindow : EditorWindow
    {
        protected int DrawSerializedList<T>(string name, SerializedProperty list, int prevCount)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(name);
            int nextCount = EditorGUILayout.IntField("Length", prevCount);
            EditorGUILayout.EndHorizontal();

            Type type = typeof(T);

            if (!list.isArray)
                throw new ArgumentException("argument is not array type.");

            if (!type.Name.Equals(list.arrayElementType))
                throw new ArgumentException("array element type is mismatched.");

            while (nextCount > list.arraySize)
            {
                list.InsertArrayElementAtIndex(list.arraySize);
            }

            while (list.arraySize > 0 && nextCount < list.arraySize)
            {
                list.DeleteArrayElementAtIndex(list.arraySize - 1);
            }

            for (int i = 0; i < list.arraySize; ++i)
            {
                SerializedProperty element = list.GetArrayElementAtIndex(i);

                EditorGUI.indentLevel += 1;
                DrawSerializedStruct<T>(element);
                EditorGUI.indentLevel -= 1;
            }

            return nextCount;
        }

        protected void DrawSerializedStruct<T>(SerializedProperty data)
        {
            Type type = typeof(T);

            if (!type.Name.Equals(data.type))
                return;

            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance;
            FieldInfo[] fieldInfos = type.GetFields(flag);

            for (int i = 0; i < fieldInfos.Length; ++i)
            {
                SerializedProperty property = data.FindPropertyRelative(fieldInfos[i].Name);
                EditorGUILayout.PropertyField(property);
            }
        }
    }
}
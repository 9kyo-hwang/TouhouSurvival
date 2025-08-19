using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Unchord.Editor
{
    [Serializable]
    public class AttributeEditorWindow : UnchordEditorWindow
    {
        private SerializedObject _serialEditorWindow;

        private SerializedProperty _serialBases;
        private SerializedProperty _serialModifiers;

        [SerializeField] private List<SerializedGameplayAttributeBase> _bases;
        [SerializeField] private List<SerializedGameplayAttributeModifier> _modifiers;

        private string _log;

        private int _basesCount;
        private int _modifiersCount;

        [MenuItem("Touhou/Attribute Table Window")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<AttributeEditorWindow>();
        }

        private void Awake()
        {
            _bases = new List<SerializedGameplayAttributeBase>(0);
            _modifiers = new List<SerializedGameplayAttributeModifier>(0);
        }

        private void OnEnable()
        {
            _serialEditorWindow = new SerializedObject(this);

            _serialBases = _serialEditorWindow.FindProperty("_bases");
            _serialModifiers = _serialEditorWindow.FindProperty("_modifiers");
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            float wNav = 45.0f;
            float hNav = 20.0f;
            if (OnNavigatorClicked("Save", wNav, hNav))
            {
                _log = "File Saved.";
                SaveFile();
            }
            if (OnNavigatorClicked("Load", wNav, hNav))
            {
                _log = "File Loaded.";
                LoadFile();
            }
            GUILayout.Label(_log ?? string.Empty, GUILayout.MinHeight(hNav), GUILayout.MaxHeight(hNav));
            GUILayout.EndHorizontal();

            _serialEditorWindow.Update();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            _basesCount = base.DrawSerializedList<SerializedGameplayAttributeBase>("base attribute table", _serialBases, _basesCount);
            _modifiersCount = base.DrawSerializedList<SerializedGameplayAttributeModifier>("modifiers", _serialModifiers, _modifiersCount);

            _serialEditorWindow.ApplyModifiedProperties();
        }

        private bool OnNavigatorClicked(string label, float w, float h)
        {
            return GUILayout.Button(label,
                GUILayout.MinWidth(w),
                GUILayout.MaxWidth(w),
                GUILayout.MinHeight(h),
                GUILayout.MaxHeight(h));
        }

        private void SaveFile()
        {
            string root = Application.streamingAssetsPath;
            string path = root + "/test.multicsv";

            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            MultiCSVWriter wr = new MultiCSVWriter(fs);

            wr.TryWriteTable<SerializedGameplayAttributeBase>(_bases, "PlayerBases");
            wr.TryWriteTable<SerializedGameplayAttributeModifier>(_modifiers, "PlayerModifiers");
            wr.Close();
            fs.Close();
        }

        private void LoadFile()
        {

        }
    }
}
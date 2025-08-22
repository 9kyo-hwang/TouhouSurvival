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

        private string _log = string.Empty;
        private int _logContinuousCount;

        // For usage toggle buttons
        private bool _useBaseValueDict = true;
        private bool _useModifierTable = true;
        private bool _useExpTable = false;

        // DEPRECATED: 현재 MultiCSVReader가 Alias를 얻어오기는 어려운 구조로 구현되어 있어서 현재 사용하지 않음.
        // Aliases of csv table
        //private string _aliasBaseValueDict = string.Empty;
        //private string _aliasModifierTable = string.Empty;
        //private string _aliasExpTable = string.Empty;

        private string _assetPathRelative = string.Empty;
        private Vector2 _scrollPosition = Vector2.zero;

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
            _serialEditorWindow.Update();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            GUILayout.BeginHorizontal();
            float wNav = 45.0f;
            float hNav = 20.0f;
            if (OnNavigatorClicked("Save", wNav, hNav))
            {
                if (SaveFile())
                {
                    PublishLog("File saved.");
                }
                else
                {
                    PublishLog("Saving file failed.");
                }
            }
            if (OnNavigatorClicked("Load", wNav, hNav))
            {
                if (LoadFile())
                {
                    PublishLog("File loaded.");
                }
                else
                {
                    PublishLog("Loading file failed.");
                }
            }
            DrawLog();
            GUILayout.EndHorizontal();

            string root = Application.streamingAssetsPath;
            int pathSuffixLength = 16;
            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 116;
            _assetPathRelative = EditorGUILayout.TextField(new GUIContent($"...{root.Substring(root.Length - pathSuffixLength, pathSuffixLength)}/"), _assetPathRelative);
            EditorGUIUtility.labelWidth = oldLabelWidth;

            //_modifiersCount = base.DrawSerializedList<SerializedGameplayAttributeModifier>("modifiers", _serialModifiers, _modifiersCount);

            DrawSeparator();

            // Draw Base Value Dictionary
            _useBaseValueDict = EditorGUILayout.BeginToggleGroup(new GUIContent("Base Value Table"), _useBaseValueDict);

            // DEPRECATED
            //_aliasBaseValueDict = EditorGUILayout.TextField(new GUIContent("Alias"), _aliasBaseValueDict, GUILayout.MaxWidth(300));

            EditorGUILayout.PropertyField(_serialBases, new GUIContent("Base Value Dictionary"), true);
            EditorGUILayout.EndToggleGroup();

            DrawSeparator();

            // Draw Modifier Elements
            _useModifierTable = EditorGUILayout.BeginToggleGroup(new GUIContent("Attribute Modifier Table"), _useModifierTable);

            // DEPRECATED
            //_aliasModifierTable = EditorGUILayout.TextField(new GUIContent("Alias"), _aliasModifierTable, GUILayout.MaxWidth(300));

            EditorGUILayout.PropertyField(_serialModifiers, true);
            EditorGUILayout.EndToggleGroup();

            DrawSeparator();

            // Draw Exp Table
            _useExpTable = EditorGUILayout.BeginToggleGroup(new GUIContent("Exp Table"), _useExpTable);

            // DEPRECATED
            //_aliasExpTable = EditorGUILayout.TextField(new GUIContent("Alias"), _aliasExpTable, GUILayout.MaxWidth(300));

            EditorGUILayout.EndToggleGroup();

            EditorGUILayout.EndScrollView();

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

        private bool SaveFile()
        {
            string path = Application.streamingAssetsPath + "/" + _assetPathRelative;
            string directory = Path.GetDirectoryName(path);

            FileStream fs = null;
            MultiCSVWriter wr = null;

            try
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
                wr = new MultiCSVWriter(fs);

                if (_useBaseValueDict)
                {
                    wr.TryWriteTable<SerializedGameplayAttributeBase>(_bases, "BaseValueTable");

                    // DEPRECATED
                    //wr.TryWriteTable<SerializedGameplayAttributeBase>(_bases, _aliasBaseValueDict);
                }

                if (_useModifierTable)
                {
                    wr.TryWriteTable<SerializedGameplayAttributeModifier>(_modifiers, "ModifierTable");

                    // DEPRECATED
                    //wr.TryWriteTable<SerializedGameplayAttributeModifier>(_modifiers, _aliasModifierTable);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                wr?.Close();
                fs?.Close();
            }
        }

        private bool LoadFile()
        {
            string path = Application.streamingAssetsPath + "/" + _assetPathRelative;
            string directory = Path.GetDirectoryName(path);

            FileStream fs = null;
            MultiCSVReader rd = null;

            try
            {
                if (!File.Exists(path))
                {
                    return false;
                }

                fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                rd = new MultiCSVReader(fs);

                List<SerializedGameplayAttributeBase> bases;
                List<SerializedGameplayAttributeModifier> modifiers;

                if (rd.TryParseTable<SerializedGameplayAttributeBase>(out bases))
                {
                    _bases = bases;
                    _serialBases = _serialEditorWindow.FindProperty("_bases");
                    _useBaseValueDict = true;
                }
                else
                {
                    _bases.Clear();
                    _useBaseValueDict = false;
                }

                if (rd.TryParseTable<SerializedGameplayAttributeModifier>(out modifiers))
                {
                    _modifiers = modifiers;
                    _serialModifiers = _serialEditorWindow.FindProperty("_modifiers");
                    _useModifierTable = true;
                }
                else
                {
                    _modifiers.Clear();
                    _useModifierTable = false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                rd?.Close();
                fs?.Close();
            }
        }

        private void DrawSeparator(int height = 1)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, height);
            rect.height = height;
            EditorGUI.DrawRect(rect, Color.gray);
        }

        private void PublishLog(string message)
        {
            if (message == null || message.Equals(string.Empty))
            {
                _logContinuousCount = 0;
                _log = string.Empty;
            }
            else if (_log.Equals(message))
            {
                _logContinuousCount++;
            }
            else
            {
                _logContinuousCount = 0;
                _log = message;
            }
        }

        private void DrawLog()
        {
            string log = _log;

            if (!log.Equals(string.Empty) && _logContinuousCount > 0)
            {
                log = $"({_logContinuousCount + 1}) {log.Trim()}";
            }

            GUILayoutOption minHeight = GUILayout.MinHeight(EditorGUIUtility.singleLineHeight);
            GUILayoutOption maxHeight = GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight);
            GUILayoutOption maxWidth = GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth);

            GUILayout.Label(log, minHeight, maxHeight, maxWidth);
        }
    }
}
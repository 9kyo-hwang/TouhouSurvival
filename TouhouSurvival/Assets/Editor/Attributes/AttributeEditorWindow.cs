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
        private SerializedObject _serialObject;

        private SerializedProperty _serialBases;
        private SerializedProperty _serialModifiers;
        private SerializedProperty _serialExpTable;

        private static AttributeEditorWindowMember s_member;
        [SerializeField] private AttributeEditorWindowMember _member;

        [MenuItem("Touhou/Attribute Table Window")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<AttributeEditorWindow>();
        }

        private void OnEnable()
        {
            _member = s_member;

            if (_member == null)
            {
                _member = new AttributeEditorWindowMember();
                _member.menuBar.SetContents(new string[] { "Save", "Load" }, new Action[] { OnClickSaveButton, OnClickLoadButton });
            }

            _serialObject = new SerializedObject(this);
            _serialBases = _serialObject.FindProperty("_member.bases");
            _serialModifiers = _serialObject.FindProperty("_member.modifiers");
            _serialExpTable = _serialObject.FindProperty("_member.expTable");
        }

        private void OnDisable()
        {
            s_member = _member;
        }

        private void OnGUI()
        {
            _serialObject.Update();

            _member.windowScrollPosition = EditorGUILayout.BeginScrollView(_member.windowScrollPosition);

            GUILayout.BeginHorizontal();
            _member.menuBar.OnGUI();
            _member.logField.OnGUI();
            GUILayout.EndHorizontal();

            _member.pathBrowser.OnGUI();

            DrawSeparator();

            // Draw Base Value Dictionary
            _member.useBaseValueDict = EditorGUILayout.BeginToggleGroup(new GUIContent("Base Value Table"), _member.useBaseValueDict);
            EditorGUILayout.PropertyField(_serialBases, new GUIContent("Base Value Dictionary"), true);
            EditorGUILayout.EndToggleGroup();

            DrawSeparator();

            // Draw Modifier Elements
            _member.useModifierTable = EditorGUILayout.BeginToggleGroup(new GUIContent("Attribute Modifier Table"), _member.useModifierTable);
            EditorGUILayout.PropertyField(_serialModifiers, true);
            EditorGUILayout.EndToggleGroup();

            DrawSeparator();

            // Draw Exp Table
            _member.useExpTable = EditorGUILayout.BeginToggleGroup(new GUIContent("Exp Table"), _member.useExpTable);
            EditorGUILayout.PropertyField(_serialExpTable, true);
            EditorGUILayout.EndToggleGroup();

            EditorGUILayout.EndScrollView();

            _serialObject.ApplyModifiedProperties();
        }

        private bool OnNavigatorClicked(string label, float w, float h)
        {
            return GUILayout.Button(label,
                GUILayout.MinWidth(w),
                GUILayout.MaxWidth(w),
                GUILayout.MinHeight(h),
                GUILayout.MaxHeight(h));
        }

        private void OnClickSaveButton()
        {
            if (SaveFile())
            {
                _member.logField.Publish("File saved.");
            }
            else
            {
                _member.logField.Publish("Saving file failed.");
            }
        }

        private void OnClickLoadButton()
        {
            if (LoadFile())
            {
                _member.logField.Publish("File loaded.");
            }
            else
            {
                _member.logField.Publish("Loading file failed.");
            }
        }

        private bool SaveFile()
        {
            string path = _member.pathBrowser.ConfirmedPath;
            string directory = Path.GetDirectoryName(path);

            FileStream fs = null;
            MultiCSVWriter wr = null;

            try
            {
                if (!Path.GetExtension(path).Equals(".multicsv"))
                {
                    return false;
                }

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
                wr = new MultiCSVWriter(fs);

                if (_member.useBaseValueDict)
                {
                    wr.TryWriteTable<SerializedGameplayAttributeBase>(_member.bases, "BaseValueTable");
                }

                if (_member.useModifierTable)
                {
                    wr.TryWriteTable<SerializedGameplayAttributeModifier>(_member.modifiers, "ModifierTable");
                }

                if (_member.useExpTable)
                {
                    wr.TryWriteTable<SerializedLevelUpExp>(_member.expTable, "ExpTable");
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
            string path = _member.pathBrowser.ConfirmedPath;
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
                List<SerializedLevelUpExp> expTable;

                if (rd.TryParseTable<SerializedGameplayAttributeBase>(out bases))
                {
                    _member.bases = bases;
                    _member.useBaseValueDict = true;
                    _serialBases = _serialObject.FindProperty("_member.bases");
                }
                else
                {
                    _member.bases.Clear();
                    _member.useBaseValueDict = false;
                }

                if (rd.TryParseTable<SerializedGameplayAttributeModifier>(out modifiers))
                {
                    _member.modifiers = modifiers;
                    _member.useModifierTable = true;
                    _serialModifiers = _serialObject.FindProperty("_member.modifiers");
                }
                else
                {
                    _member.modifiers.Clear();
                    _member.useModifierTable = false;
                }

                if (rd.TryParseTable<SerializedLevelUpExp>(out expTable))
                {
                    _member.expTable = expTable;
                    _member.useExpTable = true;
                    _serialExpTable = _serialObject.FindProperty("_member.expTable");
                }
                else
                {
                    _member.expTable.Clear();
                    _member.useExpTable = false;
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
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Unchord.Editor
{
    [Serializable]
    public class ShopShelfEditorWindow : UnchordEditorWindow
    {
        private SerializedObject _serialObject;

        private SerializedProperty _serialPaths;

        private static ShopShelfEditorWindowMember s_member;
        [SerializeField] ShopShelfEditorWindowMember _member;

        [MenuItem("Touhou/Shop Shelf Window")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<ShopShelfEditorWindow>();
        }

        private void OnEnable()
        {
            _member = s_member;

            if (_member == null)
            {
                _member = new ShopShelfEditorWindowMember();
                _member.menuBar.SetContents(new string[] { "Save", "Load" }, new Action[] { OnClickSaveButton, OnClickLoadButton });
            }

            _serialObject = new SerializedObject(this);
            _serialPaths = _serialObject.FindProperty("_member.paths");
        }

        private void OnDisable()
        {
            s_member = _member;
        }

        private void OnGUI()
        {
            _serialObject.Update();

            _member.windowScrollPosition = EditorGUILayout.BeginScrollView(_member.windowScrollPosition);

            EditorGUILayout.BeginHorizontal();
            _member.menuBar.OnGUI();
            _member.logField.OnGUI();
            EditorGUILayout.EndHorizontal();

            _member.pathBrowser.OnGUI();

            base.DrawSeparator();

            EditorGUILayout.PropertyField(_serialPaths, new GUIContent("Shop Item Path Relative"), true);

            EditorGUILayout.EndScrollView();

            _serialObject.ApplyModifiedProperties();
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

                wr.TryWriteTable<SerializedShopItem>(_member.paths, "ShopItemShelf");

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

                List<SerializedShopItem> paths;
                
                if (rd.TryParseTable<SerializedShopItem>(out paths))
                {
                    _member.paths = paths;
                    _serialPaths = _serialObject.FindProperty("_member.paths");
                }
                else
                {
                    _member.paths.Clear();
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
using UnityEditor;
using UnityEngine;

namespace Unchord.Editor
{
    public class SecondEditorWindow : EditorWindow
    {
        private string _path;

        [MenuItem("Window/SecondEditorWindow")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<SecondEditorWindow>();
        }

        private void OnGUI()
        {
            _path = EditorGUILayout.TextField("Path", _path);

            if (GUILayout.Button("Save"))
            {
                Debug.Log("SecondEditorWindow.Save.OK");
            }
        }
    }
}
using System;
using UnityEditor;
using UnityEngine;

namespace Unchord.Editor
{
    public class MenuBar : UnchordEditorGUI
    {
        private string[] _contents;
        private Action[] _actions;
        private int _contentsCount;
        private float _buttonWidth;
        private float _buttonHeight;

        public MenuBar(float buttonWidth = 45.0f, float buttonHeight = 20.0f)
        {
            _contentsCount = 0;
            _buttonWidth = buttonWidth;
            _buttonHeight = buttonHeight;
        }

        public override void OnGUI()
        {
            for (int i = 0; i < _contentsCount; ++i)
            {
                if (GUILayout.Button(_contents[i],
                    GUILayout.MinWidth(_buttonWidth),
                    GUILayout.MaxWidth(_buttonWidth),
                    GUILayout.MinHeight(_buttonHeight),
                    GUILayout.MaxHeight(_buttonHeight)))
                {
                    _actions[i]?.Invoke();
                }
            }
        }

        public void SetContents(string[] contents, Action[] actions)
        {
            UnityEngine.Debug.Assert(contents != null && actions != null);
            UnityEngine.Debug.Assert(contents.Length == actions.Length);
            UnityEngine.Debug.Assert(contents.Length > 0);

            _contentsCount = contents.Length;
            _contents = contents;
            _actions = actions;
        }
    }
}
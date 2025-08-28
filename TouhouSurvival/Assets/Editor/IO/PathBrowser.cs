using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Unchord.Editor
{
    public class PathBrowser : UnchordEditorGUI
    {
        public string ConfirmedPath
        {
            get
            {
                ConfirmPathInput();
                return _root + "/" + _pathBuffer;
            }
        }

        private bool _usePathBrowser;
        private Vector2 _scrollPosition;

        private string _root;
        private int _suffixLength;

        private List<char> _pathField;
        private Stack<int> _pathTokenLengths;
        private int _pathTokenLengthSum;

        private int _selectedIndex;

        private string _pathInput;
        private string _pathBuffer;
        private bool _isPathExists;
        private bool _isPathEditing;

        private string[] _defaultPaths0;
        private string[] _defaultPaths1;
        private string[] _nextPaths;
        private bool _shouldUpdateBrowserContents;

        public PathBrowser(string root, int suffixLength)
        {
            _usePathBrowser = false;
            _scrollPosition = Vector2.zero;

            _root = root;
            _suffixLength = suffixLength;

            _pathField = new List<char>(256);
            _pathTokenLengths = new Stack<int>(16);
            _pathTokenLengthSum = 0;

            _selectedIndex = -1;

            _pathInput = string.Empty;
            _pathBuffer = string.Empty;

            _defaultPaths0 = new string[0];
            _defaultPaths1 = new string[1] { "../" };
            _nextPaths = _defaultPaths0;
            _shouldUpdateBrowserContents = true;

            _pathField.AddRange(root);
            _pathField.Add('/');
        }

        public override void OnGUI()
        {
            // Input Field
            float oldWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 7.25f * _suffixLength;
            _pathInput = EditorGUILayout.TextField($"...{_root.Substring(_root.Length - _suffixLength, _suffixLength)}/", _pathInput);
            EditorGUIUtility.labelWidth = oldWidth;

            _isPathEditing |= (_pathInput != _pathBuffer);

            if (_isPathEditing)
            {
                switch (Event.current.keyCode)
                {
                    case KeyCode.Return:
                        ConfirmPathInput();
                        break;
                    case KeyCode.Space:
                        break;
                    default:
                        break;
                }
            }

            // Browser
            string pathBrowserLabel = GetPathBrowserLabel();
            _usePathBrowser = EditorGUILayout.BeginFoldoutHeaderGroup(_usePathBrowser, pathBrowserLabel);

            if (_shouldUpdateBrowserContents)
            {
                _shouldUpdateBrowserContents = false;
                _nextPaths = GetPathBrowserContents();
            }

            if (_usePathBrowser)
            {
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(EditorGUIUtility.singleLineHeight * 5.0f));

                for (int i = 0; i < _nextPaths.Length; ++i)
                {
                    Rect buttonRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);

                    if (GUI.Button(buttonRect, _nextPaths[i], EditorStyles.label))
                    {
                        if (i == _selectedIndex)
                        {
                            OnPathBrowserSelected(_nextPaths[i]);
                        }
                        else
                        {
                            _selectedIndex = i;
                        }
                    }

                    if (i == _selectedIndex)
                    {
                        EditorGUI.DrawRect(buttonRect, new Color(0.3f, 0.5f, 1.0f, 0.2f));
                    }
                }

                EditorGUILayout.EndScrollView();
            }
            else
            {
                _selectedIndex = -1;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        public void ConfirmPathInput()
        {
            PopAllTokens();
            _pathBuffer = PushTokens(_pathInput);
            _shouldUpdateBrowserContents = true;
            _isPathEditing = false;
        }

        private string[] GetPathBrowserContents()
        {
            string path = new string(_pathField.ToArray());

            string[] nextDirectories = null;
            string[] nextFiles = null;
            string[] nextPaths = null;

            if (Directory.Exists(path))
            {
                _isPathExists = true;
                nextDirectories = Directory.GetDirectories(path);
                nextFiles = Directory.GetFiles(path, "*.multicsv");
            }
            else
            {
                _isPathExists = false;
                return _defaultPaths1;
            }

            int offset = _pathTokenLengths.Count > 0 ? 1 : 0;

            nextPaths = new string[nextDirectories.Length + nextFiles.Length + offset];

            if (nextPaths.Length > 0)
            {
                nextPaths[0] = "../";
            }

            for (int i = 0; i < nextDirectories.Length; ++i)
            {
                int j = i + offset;
                nextPaths[j] = Path.GetFileName(nextDirectories[i]) + "/";
            }

            offset += nextDirectories.Length;

            for (int i = 0; i < nextFiles.Length; ++i)
            {
                int j = i + offset;
                nextPaths[j] = Path.GetFileName(nextFiles[i]);
            }

            return nextPaths;
        }

        private void OnPathBrowserSelected(string content)
        {
            GUI.FocusControl(null);

            if (content.Equals("../"))
            {
                // Upper Directory
                _selectedIndex = -1;
                PopToken();

                string relPath = Path.GetRelativePath(Application.streamingAssetsPath, new string(_pathField.ToArray())).Replace("\\", "/");

                if (relPath.Equals("."))
                    _pathInput = string.Empty;
                else
                    _pathInput = relPath;

                _pathBuffer = _pathInput;
                _shouldUpdateBrowserContents = true;
            }
            else if (content.EndsWith("/"))
            {
                // Sub Directory
                _selectedIndex = -1;
                PushToken(content);
                _pathInput = Path.GetRelativePath(Application.streamingAssetsPath, new string(_pathField.ToArray())).Replace("\\", "/");
                _pathBuffer = _pathInput;
                _shouldUpdateBrowserContents = true;
            }
            else
            {
                // File
                _pathInput = Path.GetRelativePath(Application.streamingAssetsPath, new string(_pathField.ToArray()) + content).Replace("\\", "/");
                _pathBuffer = _pathInput;
            }
        }

        private void PushToken(string token)
        {
            UnityEngine.Debug.Assert(token != null);
            UnityEngine.Debug.Assert(token.Length > 0);

            _pathField.AddRange(token);
            _pathTokenLengths.Push(token.Length);
            _pathTokenLengthSum += token.Length;
        }

        private string PushTokens(string tokensConcatenated)
        {
            UnityEngine.Debug.Assert(tokensConcatenated != null);

            string[] tokens = tokensConcatenated.Replace('\\', '/').Split('/');

            for (int i = 0; i < tokens.Length - 1; ++i)
            {
                if (tokens[i].Length == 0)
                    continue;
                PushToken(tokens[i] + '/');
            }

            char[] tokensRefined = new char[_pathTokenLengthSum];
            _pathField.CopyTo(_pathField.Count - _pathTokenLengthSum, tokensRefined, 0, _pathTokenLengthSum);

            string directory = new string(tokensRefined);
            string file = tokens[tokens.Length - 1];

            return directory + file;
        }

        private void PopToken()
        {
            UnityEngine.Debug.Assert(_pathTokenLengths.Count > 0);

            int length = _pathTokenLengths.Pop();
            _pathField.RemoveRange(_pathField.Count - length, length);
            _pathTokenLengthSum -= length;
        }

        private void PopAllTokens()
        {
            while (_pathTokenLengths.Count > 0)
                PopToken();
        }

        private string GetPathBrowserLabel()
        {
            string label = "Path Browser";

            if (_isPathEditing)
                label += "*";

            if (!_isPathExists)
                label += " (not exist folder)";

            return label;
        }
    }
}
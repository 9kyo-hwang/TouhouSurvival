using UnityEditor;
using UnityEngine;

namespace Unchord.Editor
{
    public class FirstEditorWindow : EditorWindow
    {
        private string _label = "Enter the Text...";
        private bool _groupEnabled;
        private bool _myBool = true;
        private float _myFloat = 1.23f;

        [MenuItem("Window/Attributes")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<FirstEditorWindow>();
        }

        private void Awake()
        {
            Debug.Log("Awake");
        }

        private void OnEnable()
        {
            Debug.Log("OnEnable");
        }

        private void Start()
        {
            Debug.Log("Start");
        }

        /// <summary>
        /// https://docs.unity3d.com/kr/560/Manual/editor-EditorWindows.html
        /// 
        /// 창의 GUI 구현
        /// 창의 실제 콘텐츠는 OnGUI 함수를 구현하여 렌더링합니다.
        /// 인게임 GUI(GUI 및 GUILayout)에 사용하는 것과 동일한 UnityGUI 클래스를 사용할 수 있습니다.
        /// 또한 EditorGUI 및 EditorGUILayout 에디터 전용 클래스에 있는 GUI 컨트롤도 몇 개 더 제공됩니다.
        /// 이 클래스는 일반 클래스에 이미 사용 가능한 컨트롤에 추가되므로 원하는 경우 믹스 앤 매치를 할 수 있습니다.
        /// </summary>
        private void OnGUI()
        {
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);

            _label = EditorGUILayout.TextField("My Text Field", _label);

            _groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", _groupEnabled);
            _myBool = EditorGUILayout.Toggle("Toggle", _myBool);
            _myFloat = EditorGUILayout.Slider("Slider", _myFloat, -3.0f, 3.0f);
            EditorGUILayout.EndToggleGroup();
        }
    }
}
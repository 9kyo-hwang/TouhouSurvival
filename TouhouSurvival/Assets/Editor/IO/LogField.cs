using UnityEditor;

namespace Unchord.Editor
{
    public class LogField : UnchordEditorGUI
    {
        private string _message;
        private int _messageContinuousCount;

        public LogField()
        {
            _message = string.Empty;
            _messageContinuousCount = 0;
        }

        public override void OnGUI()
        {
            if (_message.Equals(string.Empty) || _messageContinuousCount == 0)
            {
                EditorGUILayout.LabelField(_message);
            }
            else
            {
                EditorGUILayout.LabelField($"({_messageContinuousCount + 1}) {_message}");
            }
        }

        public void Clear()
        {
            _message = string.Empty;
            _messageContinuousCount = 0;
        }

        public void Publish(string message)
        {
            if (message == null)
            {
                message = string.Empty;
            }

            if (_message.Equals(message))
            {
                _messageContinuousCount++;
            }
            else
            {
                _message = message;
                _messageContinuousCount = 0;
            }
        }
    }
}
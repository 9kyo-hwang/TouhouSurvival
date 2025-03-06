using UnityEngine;

namespace Unchord
{
    public class PlayerCamera : MonoBehaviour
    {
        [Range(0.01f, 30.0f)]
        public float traceSpeed = 5.0f;

        [Range(0.0f, 10.0f)]
        public float limitRadius = 2.0f;

        private GameManager _gameManager;

        private Vector2 _prevPosition;
        private Vector2 _nextPosition;
        private Vector2 _camPosition;

        private bool _prevAimed;
        private bool _nextAimed;

        private float _finalDistance;

        private void Awake()
        {
            _gameManager = GameManager.Instance;
        }

        private void Update()
        {
            AimTarget();
            //TraceTarget();
        }

        private void AimTarget()
        {
            if (!_gameManager.Player)
                return;

            float camZ = _gameManager.MainCamera.transform.position.z;

            _camPosition = _gameManager.Player.transform.position;

            _gameManager.MainCamera.transform.position = new Vector3(_camPosition.x, _camPosition.y, camZ);
        }

        private void TraceTarget()
        {
            _prevAimed = _nextAimed;
            _nextAimed = _gameManager.Player;

            if (!_nextAimed)
                return;

            float camZ = _gameManager.MainCamera.transform.position.z;

            _prevPosition = _nextPosition;
            _nextPosition = _gameManager.Player.transform.position;

            if (!_prevAimed)
            {
                _prevPosition = _nextPosition;
                _camPosition = _nextPosition;
                _gameManager.MainCamera.transform.position = new Vector3(_camPosition.x, _camPosition.y, camZ);
                return;
            }

            // TODO: 위치 보간 코드를 이 곳에 삽입합니다.
            if (_prevPosition == _nextPosition)
                _finalDistance = Mathf.Lerp(_finalDistance, 0.0f, traceSpeed * Time.deltaTime);
            else
                _finalDistance = Mathf.Lerp(_finalDistance, limitRadius * 1.001f, traceSpeed * Time.deltaTime);

            _camPosition = _nextPosition + (_camPosition - _nextPosition).normalized * _finalDistance;
            _gameManager.MainCamera.transform.position = new Vector3(_camPosition.x, _camPosition.y, camZ);
        }
    }
}
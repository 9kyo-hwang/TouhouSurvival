using UnityEngine;

namespace Unchord
{
    public class PlayerCamera : MonoBehaviour
    {
        public float rMax = 3.0f;
        public float sMax = 5.0f;

        public float w = 0.2f;

        private GameManager _gameManager;

        private Vector2 _prevPosition;
        private Vector2 _moveSum;

        private Transform _target;

        private void Awake()
        {
            _gameManager = GameManager.Instance;
        }

        private void FixedUpdate()
        {
            if (_gameManager.Player == null)
            {
                _target = null;
                return;
            }
            else if (_target == null)
            {
                SetTarget(_gameManager.Player.transform);
            }

            TraceTarget(this.transform, _target);
        }

        private void LateUpdate()
        {
            if (_target == null)
            {
                return;
            }

            SetPosition(this.transform, _target);
        }

        private void SetTarget(Transform target)
        {
            _prevPosition = target.position;
            _target = target;
        }

        private void TraceTarget(Transform camera, Transform target)
        {
            Vector2 deltaPosition = (Vector2)target.position - _prevPosition;
            _prevPosition = target.position;

            float currentSpeed = deltaPosition.magnitude / Time.fixedDeltaTime;

            if (currentSpeed > 0.0f)
            {
                _moveSum += deltaPosition;
                _moveSum *= (1.0f - w);

                float r = currentSpeed * (rMax / sMax);
                r = Mathf.Round(r * 10.0f) / 10.0f;

                if (r > rMax)
                    r = rMax;

                if (_moveSum.magnitude > r)
                    _moveSum = _moveSum.normalized * r;
            }
            else
            {
                UnityEngine.Debug.Assert(w > 0.0f && w <= 1.0f);

                _moveSum *= (1.0f - w);
            }
        }

        private void SetPosition(Transform camera, Transform target)
        {
            Vector3 position = camera.position;
            position.x = target.position.x - _moveSum.x;
            position.y = target.position.y - _moveSum.y;

            Debug.Log($"distance == {_moveSum.magnitude}");

            camera.position = position;
        }
    }
}
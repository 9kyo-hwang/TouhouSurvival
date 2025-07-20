using UnityEngine;

namespace Unchord
{
    public class PlayerCamera : MonoBehaviour
    {
        public float maxRadius = 1.5f; // 타겟이 카메라 중심에서 이 반지름 내에 반드시 존재함.

        // TODO: 코드 분석 필요.
        // 타겟의 현재 이동 속도가 clampingTargetSpeedThreshold에 도달할 때 타겟이 카메라 중심에서 maxRadius 만큼 떨어지도록 코드를 설계하는 것이 목표였다.
        // 현재 계산컨대, 타겟의 이동 속도가 radiusMax * wIncrease / (Time.fixedDeltaTime * (1 - wIncrease))에 도달했을 때 maxRadius에 도달하는 것으로 계산됨.
        // 계산을 면밀히 검토해 볼 필요 있음.
        public float clampingTargetSpeedThreshold = 5.0f;

        [Range(0.01f, 1.0f)]
        public float wIncrease = 0.065f;

        [Range(0.01f, 1.0f)]
        public float wDecrease = 0.055f;

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
                UnityEngine.Debug.Assert(wIncrease > 0.0f && wIncrease <= 1.0f);

                _moveSum += deltaPosition;
                _moveSum *= (1.0f - wIncrease);

                float r = currentSpeed * (maxRadius / clampingTargetSpeedThreshold);
                r = Mathf.Round(r * 10.0f) / 10.0f;

                if (r > maxRadius)
                    r = maxRadius;

                if (_moveSum.magnitude > r)
                    _moveSum = _moveSum.normalized * r;
            }
            else
            {
                UnityEngine.Debug.Assert(wDecrease > 0.0f && wDecrease <= 1.0f);

                _moveSum *= (1.0f - wDecrease);
            }
        }

        private void SetPosition(Transform camera, Transform target)
        {
            Vector3 position = camera.position;
            position.x = target.position.x - _moveSum.x;
            position.y = target.position.y - _moveSum.y;

            camera.position = position;
        }
    }
}
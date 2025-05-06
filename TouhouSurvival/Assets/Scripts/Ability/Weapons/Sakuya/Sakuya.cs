using System.Collections;
using Unchord;
using UnityEngine;
using UnityEngine.Pool;

namespace Unchord
{
    public class Sakuya : MonoBehaviour
    {
        private Vector3 _direction;
        private float _speed;
        private float _duration;

        public void Initialize(Vector3 playerPosition, Vector3 targetPosition, float throwAngleOffset, float speed, float duration)
        {
            _direction = Projectile.GetTargetDirectionVector(playerPosition, targetPosition, throwAngleOffset);
            _speed = speed;
            _duration = duration;
            
            Move();
        }

        private void Move()
        {
            LinearProjectile projectile = GetComponent<LinearProjectile>();
            projectile.transform.position = GameManager.Instance.Player.transform.position;
            projectile.ProjectileSpeed = _speed;
            projectile.ProjectileDirection = _direction;
            projectile.OriginEulerAngle = Vector2.SignedAngle(Vector2.right, _direction);
            projectile.RotationSpeed = 0.0f;

            StartCoroutine(Timeout());
        }

        private IEnumerator Timeout()
        {
            yield return new WaitForSeconds(_duration);

            GetComponentInParent<LinearProjectile>(includeInactive: true)
                .FlagTable[AbilityComponent.FLAG_SHOULD_DESTROY] = true;
        }
    }
}


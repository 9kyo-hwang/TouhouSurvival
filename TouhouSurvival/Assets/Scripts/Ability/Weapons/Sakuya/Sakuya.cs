using Unchord;
using UnityEngine;
using UnityEngine.Pool;

namespace Unchord
{
    public class Sakuya : MonoBehaviour
    {
        private ObjectPool<GameObject> _pool;
        private Vector3 _direction;
        private float _speed;

        public void Initialize(ObjectPool<GameObject> pool, 
            Vector3 playerPosition, Vector3 targetPosition, float throwAngleOffset,
            float speed)
        {
            _pool = pool;
            _direction = Projectile.GetTargetDirectionVector(playerPosition, targetPosition, throwAngleOffset);
            _speed = speed;
            
            Move();
        }

        private void Move()
        {
            GameObject sakuyaGameObject = _pool.Get();
            
            LinearProjectile projectile = sakuyaGameObject.GetComponent<LinearProjectile>();
            projectile.transform.localPosition = Vector3.zero;
            projectile.OriginPosition = projectile.transform.position;
            projectile.ProjectileSpeed = _speed;
            projectile.ProjectileDirection = _direction;
        }
    }
}


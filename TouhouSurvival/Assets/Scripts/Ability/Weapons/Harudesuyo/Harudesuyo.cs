using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace Unchord
{
    public class Harudesuyo : MonoBehaviour
    {
        private float _delay;                           // 생성 후 떨어지기까지 걸리는 지연시간
        private Vector3 _rotation;                   // 폭탄 이미지 회전값
        private Vector3 _spawnPoint;                    // 폭탄 생성 지점
        private Vector3 _dropPoint;                     // 폭탄 낙하 지점
        private ObjectPool<Harudesuyo> _pool;
        private ObjectPool<GameObject> _explosionPool;  // Controller에서 Pool 주소값을 넘겨받아 생성
        
        public float bombFallSpeed = 5f;                // 폭탄 떨어지는 속도
        
        public void Initialize(float fallDelay, Vector3 rotation, Vector3 spawnPoint, Vector3 dropPoint, ObjectPool<Harudesuyo> pool, ObjectPool<GameObject> explosionPool)
        {
            _delay = fallDelay;
            _rotation = rotation;
            _spawnPoint = spawnPoint;
            _dropPoint = dropPoint;
            _pool = pool;
            _explosionPool = explosionPool;
            
            StartCoroutine(FallBomb());
        }

        // _delay 시간 후 dropPoint를 향해 폭탄 낙하
        private IEnumerator FallBomb()
        {
            LinearProjectile projectile = GetComponent<LinearProjectile>();
            projectile.transform.position = _spawnPoint;
            projectile.transform.rotation = Quaternion.Euler(_rotation + new Vector3(0f, 0f, 90f));
            projectile.OriginPosition = _spawnPoint;
            projectile.ProjectileSpeed = 0.0f;
            projectile.ProjectileDirection = Vector2.zero;
            
            yield return new WaitForSeconds(_delay);
            
            projectile.ProjectileDirection = Projectile.GetTargetDirectionVector(_spawnPoint, _dropPoint, 0.0f);
            projectile.ProjectileSpeed = bombFallSpeed;
            projectile.OriginEulerAngle = Vector2.SignedAngle(Vector2.right, projectile.ProjectileDirection);

            float journeyLength = (_spawnPoint - _dropPoint).sqrMagnitude;
            while (journeyLength > (projectile.transform.position - _spawnPoint).sqrMagnitude)
            {
                yield return null;
            }

            PlayExplosionEffect();
        }

        private void PlayExplosionEffect()
        {
            _pool.Release(this);
            
            GameObject effect = _explosionPool.Get();   // Animation Play
            effect.transform.position = _dropPoint;
            effect.GetComponent<DotProjectile>().OriginPosition = _dropPoint;
        }
    }
}
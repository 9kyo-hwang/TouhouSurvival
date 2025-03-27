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
        private float _radius;                          // 폭발 범위
        private float _damage;                          // 폭발 데미지
        private Quaternion _rotation;                   // 폭탄 이미지 회전값
        private Vector3 _spawnPoint;                    // 폭탄 생성 지점
        private Vector3 _dropPoint;                     // 폭탄 낙하 지점
        private ObjectPool<Harudesuyo> _pool;           // 생성 후 Pool에 반환시키기 위해 참조만 들고 있음
        private ObjectPool<GameObject> _explosionPool;  // Controller에서 Pool 주소값을 넘겨받아 생성
        
        public GameObject bombImage;                    // 떨어지는 폭탄 이미지
        public float bombFallSpeed = 5f;                // 폭탄 떨어지는 속도
        
        public void Initialize(float fallDelay, float explosionRadius, float explosionDamage, 
            Quaternion rotation, Vector3 spawnPoint, Vector3 dropPoint, 
            ObjectPool<Harudesuyo> bombPool, ObjectPool<GameObject> explosionPool)
        {
            _delay = fallDelay;
            _radius = explosionRadius;
            _damage = explosionDamage;
            _rotation = rotation;
            _spawnPoint = spawnPoint;
            _dropPoint = dropPoint;
            _pool = bombPool;
            _explosionPool = explosionPool;
            
            bombImage.SetActive(true);
            StartCoroutine(Fall());
        }

        // _delay 시간 후 dropPoint를 향해 폭탄 낙하
        private IEnumerator Fall()
        {
            yield return new WaitForSeconds(_delay);

            LinearProjectile projectile = GetComponent<LinearProjectile>();
            projectile.transform.localPosition = Vector3.zero;
            projectile.transform.rotation = _rotation;
            projectile.OriginPosition = _spawnPoint;
            projectile.ProjectileSpeed = bombFallSpeed;

            projectile.ProjectileDirection = Projectile.GetTargetDirectionVector(_spawnPoint, _dropPoint, 0.0f);
        }

        private void Explode()
        {
            GameObject effect = _explosionPool.Get();
            effect.transform.position = _dropPoint;
            effect.GetComponent<HarudesuyoExplosionEffect>().Play();
            
            Collider2D[] hitObjects = Physics2D.OverlapCircleAll(_dropPoint, _radius, LayerMask.GetMask("Enemy"));
            foreach (Collider2D hit in hitObjects)
            {
                Enemy enemy = hit.GetComponentInParent<Enemy>();
                if (enemy)
                {
                    Player player = gameObject.GetComponentInParent<Player>();
                    enemy.TakeDamage(_damage, player, effect);
                }
            }
            
            bombImage.SetActive(false);
        }

        private void OnDisable()
        {
            // TODO: 폭탄 이미지 및 기타 상태 초기화
            bombImage.SetActive(false);
        }
    }
}
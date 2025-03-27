using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace Unchord
{
    public class Harudesuyo : MonoBehaviour
    {
        private float _fallTime;
        private float _radius;
        private float _damage;
        private Transform _target;
        private ObjectPool<Harudesuyo> _pool;           // 생성 후 Pool에 반환시키기 위해 참조만 들고 있음
        private ObjectPool<GameObject> _explosionPool;  // Controller에서 Pool 주소값을 넘겨받아 생성
        
        public GameObject projectile;   // 떨어지는 미사일 이미지

        public void Initialize(float fallTime, float radius, float damage, Transform target, ObjectPool<Harudesuyo> pool, ObjectPool<GameObject> explosionPool)
        {
            _fallTime = fallTime;
            _radius = radius;
            _damage = damage;
            _target = target;
            _pool = pool;
            _explosionPool = explosionPool;
            
            projectile.SetActive(true);
            StartCoroutine(Fall());
        }

        // _fallTime에 걸쳐 폭탄이 떨어짐
        private IEnumerator Fall()
        {
            Vector3 startPos = projectile.transform.position;
            Vector3 targetPos = _target.position;
            
            float journeyLength = Vector3.Distance(startPos, targetPos);
            float journeyTime = _fallTime;
            float startTime = Time.time;

            while (projectile.transform.position.y < targetPos.y)
            {
                float distCovered = (Time.time - startTime) * (journeyLength / journeyTime);
                float fractionOfJourney = distCovered / journeyLength;
                
                projectile.transform.position = Vector3.Lerp(startPos, targetPos, fractionOfJourney);
                yield return null;
            }
            
            projectile.transform.position = targetPos;
            Explode();
        }

        private void Explode()
        {
            GameObject effect = _explosionPool.Get();
            effect.transform.position = projectile.transform.position;
            effect.GetComponent<HarudesuyoExplosionEffect>().Play();
            
            Collider2D[] hitObjects = Physics2D.OverlapCircleAll(effect.transform.position, _radius, LayerMask.GetMask("Enemy"));
            foreach (Collider2D hit in hitObjects)
            {
                Enemy enemy = hit.GetComponentInParent<Enemy>();
                if (enemy)
                {
                    enemy.TakeDamage(_damage, enemy, effect);
                }
            }
            
            // 폭발 완료 후 오브젝트 풀에 반환
            _pool.Release(this);
            projectile.SetActive(false);
        }

        private void OnDisable()
        {
            // TODO: 폭탄 이미지 및 기타 상태 초기화
            projectile.SetActive(false);
        }
    }
}
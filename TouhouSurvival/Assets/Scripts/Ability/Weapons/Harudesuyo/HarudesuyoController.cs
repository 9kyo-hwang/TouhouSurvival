using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unchord;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Unchord
{
    public class HarudesuyoController : WeaponComponent
    {
        [Header("Prefab Settings")] 
        public Harudesuyo harudesuyoPrefab; // 떨어지는 폭탄
        public GameObject explosionPrefab;  // 폭발 이펙트
        
        private const float SpawnDelayVariationRatio = 0.2f;
        private const float SpawnHorizontalDirectionOffset = 96f;
        private const float SpawnVerticalDirectionOffset = 270f;
        
        #region Harudesuyo Bomb Pool
        private ObjectPool<Harudesuyo> _bombPool;
        private void ActionOnDestroyProjectile(Harudesuyo obj)
        {
            Destroy(obj.gameObject);
        }

        private void ActionOnReleaseProjectile(Harudesuyo obj)
        {
            obj.gameObject.SetActive(false);
        }

        private void ActionOnGetProjectile(Harudesuyo obj)
        {
            obj.gameObject.SetActive(true);
        }

        private Harudesuyo ProjectileCreateFunc()
        {
            return Instantiate(harudesuyoPrefab);
        }
        #endregion
        
        #region Harudesuyo Explosion Pool
        private ObjectPool<GameObject> _explosionPool;
        private void ActionOnDestroyExplosion(GameObject obj)
        {
            // Destroy(obj);    // 아무런 동작을 수행하지 않음
        }

        private void ActionOnReleaseExplosion(GameObject obj)
        {
            obj.gameObject.SetActive(false);
        }

        private void ActionOnGetExplosion(GameObject obj)
        {
            FlagComponent flagTable = obj.GetComponent<FlagComponent>();
            flagTable.SetFlagFalseWithoutEvent(FLAG_SHOULD_DESTROY);
            
            obj.gameObject.SetActive(true);
            
            Animator animator = obj.GetComponent<Animator>();
            animator.Play("HarudesuyoExplosion", -1, 0.0f);
        }

        private GameObject ExplosionCreateFunc()
        {
            GameObject explosion = Instantiate(explosionPrefab.gameObject);
            
            CollisionEventEmitter emitter = explosion.transform.Find("Colliders/Circle Collider 2D")
                .GetComponent<CollisionEventEmitter>();
            emitter.onTriggerEnter2D += (sender, args) =>
            {
                GameObject enemyObject = args.targetObject;
                if (!enemyObject)
                {
                    return;
                }

                Enemy enemy = enemyObject.GetComponentInParent<Enemy>();
                if (!enemy)
                {
                    return;
                }

                enemy.TakeDamage(
                    Attributes[HarudesuyoAttributeType.BombExplosionDamage].CurrentValue,
                    GetComponentInParent<Player>(),
                    explosion
                );
            };

            FlagComponent flagTable = explosion.GetComponent<FlagComponent>();
            flagTable.AddEventTrue(FLAG_SHOULD_DESTROY, component =>
            {
                _explosionPool.Release(component.gameObject);
            });

            return explosion;
        }
        #endregion

        protected override void Awake()
        {
            base.Awake();

            _bombPool = new ObjectPool<Harudesuyo>(
                createFunc: ProjectileCreateFunc,
                actionOnGet: ActionOnGetProjectile,
                actionOnRelease: ActionOnReleaseProjectile,
                actionOnDestroy: ActionOnDestroyProjectile,
                collectionCheck: true,
                defaultCapacity: 10,
                maxSize: 50);

            _explosionPool = new ObjectPool<GameObject>(
                createFunc: ExplosionCreateFunc,
                actionOnGet: ActionOnGetExplosion,
                actionOnRelease: ActionOnReleaseExplosion,
                actionOnDestroy: ActionOnDestroyExplosion,
                collectionCheck: true,
                defaultCapacity: 10,
                maxSize: 50
            );
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void UseWeapon()
        {
            base.UseWeapon();
            
            foreach (Vector3 dropPoint in GetDropPoints())
            {
                StartCoroutine(SpawnBomb(dropPoint));
            }
        }

        private Vector3[] GetDropPoints()
        {
            int numDropPoint = (int)Attributes[HarudesuyoAttributeType.NumDropPoints].CurrentValue;
            Vector3[] points = new Vector3[numDropPoint];
            float bombingRange = Attributes[HarudesuyoAttributeType.BombingRange].CurrentValue;

            for (int i = 0; i < numDropPoint; ++i)
            {
                points[i] = new Vector3(
                    transform.position.x + Random.Range(-bombingRange, bombingRange),
                    transform.position.y + Random.Range(-bombingRange, bombingRange),
                    transform.position.z
                );
            }

            return points;
        }

        private IEnumerator SpawnBomb(Vector3 dropPoint)
        {
            Vector3 spawnPoint = GetBombSpawnPoint();
            Quaternion rotation = GetBombRotation(spawnPoint);
            float fallDelay = Attributes[HarudesuyoAttributeType.BombFallDelay].CurrentValue;
            float explosionRadius = Attributes[HarudesuyoAttributeType.BombExplosionRadius].CurrentValue;
            float explosionDamage = Attributes[HarudesuyoAttributeType.BombExplosionDamage].CurrentValue;
            
            Harudesuyo bomb = _bombPool.Get();
            bomb.Initialize(fallDelay, explosionRadius, explosionDamage, rotation, spawnPoint, dropPoint, _bombPool, _explosionPool);

            float spawnDelay = Attributes[HarudesuyoAttributeType.BombSpawnDelay].CurrentValue;
            float randomFactor = Random.Range(1f - SpawnDelayVariationRatio, 1f + SpawnDelayVariationRatio);
            yield return new WaitForSeconds(spawnDelay * randomFactor);
        }

        private Vector3 GetBombSpawnPoint()
        {
            return Random.Range(0, 3) switch
            {
                0 => new Vector3(transform.position.x - SpawnHorizontalDirectionOffset, transform.position.y + SpawnVerticalDirectionOffset, transform.position.z),
                1 => new Vector3(transform.position.x, transform.position.y + SpawnVerticalDirectionOffset, transform.position.z),
                2 => new Vector3(transform.position.x + SpawnHorizontalDirectionOffset, transform.position.y + SpawnVerticalDirectionOffset, transform.position.z),
                _ => Vector3.zero
            };
        }

        private Quaternion GetBombRotation(Vector3 spawnPoint)
        {
            if (spawnPoint.x < transform.position.x) return Quaternion.Euler(0f, 0f, 45f);  // 좌측 상공
            if (spawnPoint.x > transform.position.x) return Quaternion.Euler(0f, 0f, -45f); // 우측 상공
            return Quaternion.Euler(0f, 0f, 0f);
        }
    }
}
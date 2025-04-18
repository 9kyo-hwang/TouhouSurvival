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
        private const float SpawnHorizontalDirectionOffset = 5f;
        private const float SpawnVerticalDirectionOffset = 10f;
        
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
            return Instantiate(harudesuyoPrefab, transform);
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

            float explosionScale = Attributes[HarudesuyoAttributeType.BombExplosionRadius].CurrentValue;
            obj.transform.localScale = new Vector3(explosionScale, explosionScale, 1);
            obj.gameObject.SetActive(true);
            
            Animator animator = obj.GetComponent<Animator>();
            animator.Play("FireballExplosion", -1, 0.0f);   // TODO: Harudesuyo 전용으로 변경
        }

        private GameObject ExplosionCreateFunc()
        {
            GameObject explosion = Instantiate(explosionPrefab.gameObject, transform);
            
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

        public override void UseWeapon()
        {
            // 1. 스킬 쿨다운마다 자동 발동
            StartCoroutine(SpawnBombCoroutine());
        }

        private Vector3[] GetDropPoints()
        {
            int numDropPoint = (int)Attributes[HarudesuyoAttributeType.NumDropPoints].CurrentValue;
            Vector3[] points = new Vector3[numDropPoint];
            float bombingRange = Attributes[HarudesuyoAttributeType.BombingRange].CurrentValue;

            for (int i = 0; i < numDropPoint; ++i)
            {
                points[i] = new Vector3(
                    _player.transform.position.x + Random.Range(-bombingRange, bombingRange),
                    _player.transform.position.y + Random.Range(-bombingRange, bombingRange),
                    _player.transform.position.z
                );
                
                Debug.Log($"Drop Point: {points[i].x}, {points[i].y}, {points[i].z}");
            }

            return points;
        }

        private IEnumerator SpawnBombCoroutine()
        {
            _isCooltimePaused = true;
            
            foreach (Vector3 dropPoint in GetDropPoints())
            {
                // 2. spawnDelay 간격으로 폭탄을 생성
                SpawnBomb(dropPoint);
                
                float spawnDelay = Attributes[HarudesuyoAttributeType.BombSpawnDelay].CurrentValue;
                float randomFactor = Random.Range(1f - SpawnDelayVariationRatio, 1f + SpawnDelayVariationRatio);
                yield return new WaitForSeconds(spawnDelay * randomFactor);
            }
            
            _isCooltimePaused = false;
        }
        
        private void SpawnBomb(Vector3 dropPoint)
        {
            Vector3 bombSpawnOffset = GetBombSpawnPointOffset();
            Vector3 spawnPoint = bombSpawnOffset + dropPoint;
            float angle = Vector2.SignedAngle(Vector2.right, -bombSpawnOffset);
            Vector3 rotation = Vector3.forward * angle;  // TODO: 각도 수정
            float fallDelay = Attributes[HarudesuyoAttributeType.BombFallDelay].CurrentValue;
            
            Harudesuyo bomb = _bombPool.Get();
            bomb.Initialize(fallDelay, rotation, spawnPoint, dropPoint, _bombPool, _explosionPool);
        }

        private Vector3 GetBombSpawnPointOffset()
        {
            return Random.Range(0, 3) switch
            {
                0 => new Vector3(-SpawnHorizontalDirectionOffset, SpawnVerticalDirectionOffset, 0),
                1 => new Vector3(0, SpawnVerticalDirectionOffset, 0),
                2 => new Vector3(SpawnHorizontalDirectionOffset, SpawnVerticalDirectionOffset, 0),
                _ => Vector3.zero
            };
        }
    }
}
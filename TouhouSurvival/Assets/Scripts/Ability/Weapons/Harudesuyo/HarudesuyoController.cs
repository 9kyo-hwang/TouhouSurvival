using System;
using System.Collections;
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
        public GameObject projectilePrefab; // 떨어지는 폭탄
        public GameObject explosionPrefab;  // 폭발 이펙트
        
        private const float SpawnDelayVariationRatio = 0.2f;
        
        #region Harudesuyo Projectile Pool
        private ObjectPool<GameObject> _projectilePool;
        private void ActionOnDestroyProjectile(GameObject obj)
        {
            Destroy(obj);
        }

        private void ActionOnReleaseProjectile(GameObject obj)
        {
            obj.SetActive(false);
        }

        private void ActionOnGetProjectile(GameObject obj)
        {
            obj.SetActive(true);
        }

        private GameObject ProjectileCreateFunc()
        {
            return Instantiate(projectilePrefab);
        }
        #endregion
        
        #region Harudesuyo Explosion Pool
        private ObjectPool<GameObject> _explosionPool;
        private void ActionOnDestroyExplosion(GameObject obj)
        {
            Destroy(obj);
        }

        private void ActionOnReleaseExplosion(GameObject obj)
        {
            obj.SetActive(false);
        }

        private void ActionOnGetExplosion(GameObject obj)
        {
            obj.SetActive(true);
        }

        private GameObject ExplosionCreateFunc()
        {
            return Instantiate(explosionPrefab);
        }
        #endregion

        protected override void Awake()
        {
            base.Awake();

            _projectilePool = new ObjectPool<GameObject>(
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

            Collider2D[] enemies = GetEnemiesOrNull();
            if (enemies == null)
            {
                return;
            }
            
            StartCoroutine(Spawn(enemies));
        }

        private Collider2D[] GetEnemiesOrNull()
        {
            float radius = Attributes[HarudesuyoAttributeType.Radius].CurrentValue;
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, radius);

            if (enemies.Length == 0)
            {
                return null;
            }
            
            float targetCount = Attributes[HarudesuyoAttributeType.TargetCount].CurrentValue;
            return enemies.Take((int)targetCount).ToArray();
        }

        IEnumerator Spawn(Collider2D[] targets)
        {
            foreach(Collider2D enemy in targets)
            {
                if (enemy)
                {
                    GameObject harudesuyo = _projectilePool.Get();
                    harudesuyo.transform.position = enemy.transform.position;
                    // TODO: initialize
                }

                float spawnDelay = Attributes[HarudesuyoAttributeType.BombSpawnDelay].CurrentValue;
                float randomFactor = Random.Range(1f - SpawnDelayVariationRatio, 1f + SpawnDelayVariationRatio);
                yield return new WaitForSeconds(spawnDelay * randomFactor);
            }
        }
    }
}
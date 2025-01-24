using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Unchord
{
    public class Fireball : WeaponComponent
    {
        [Header("Shooting Properties")]
        public float baseShootingCooldown;
        public float baseProjectileSize;
        public float baseEulerAngleError;
        public float baseBurstDelay;
        public int baseBurstCount;

        [Header("Projectile Properties")]
        public LinearProjectile projectilePrefab;
        public float baseProjectileSpeed;
        public float baseProjectileDamage;
        public float baseProjectileKnockbackForce;

        [Header("Explosion Properties")]
        public CollisionEventEmitter explosionPrefab;
        public float baseExplosionSize;
        public float baseExplosionDuration;
        public float baseExplosionKnockbackForce;
        public float baseExplosionDamage;

        [Header("Object Pool Setting")]
        public int projectilePoolCapacity;
        public int explosionPoolCapacity;

        private ObjectPool<LinearProjectile> projectilePool;
        private ObjectPool<CollisionEventEmitter> explosionPool;

        private List<LinearProjectile> _enableProjectiles;
        private List<CollisionEventEmitter> _destroyExplosionReserves;

        [Header("Test Flag")]
        public bool flag_shoot;

        private void Awake()
        {
            projectilePool = new ObjectPool<LinearProjectile>(
                OnCreateProjectile,
                OnGetProjectile,
                OnReleaseProjectile,
                OnDestroyProjectile,
                true,
                projectilePoolCapacity,
                100);
            explosionPool = new ObjectPool<CollisionEventEmitter>(
                OnCreateExplosion,
                OnGetExplosion,
                OnReleaseExplosion,
                OnDestroyExplosion,
                true,
                explosionPoolCapacity,
                100);

            _enableProjectiles = new List<LinearProjectile>(projectilePoolCapacity);
            _destroyExplosionReserves = new List<CollisionEventEmitter>(explosionPoolCapacity);
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            if (flag_shoot)
            {
                flag_shoot = false;
                float x = UnityEngine.Random.value;
                float y = UnityEngine.Random.value;
                Shoot(new Vector2(x, y).normalized);
            }

            for (int i = _enableProjectiles.Count - 1; i >= 0; --i)
            {
                LinearProjectile projectile = _enableProjectiles[i];

                if (projectile.ShouldDestroy)
                {
                    _enableProjectiles.RemoveAt(i);
                    projectilePool.Release(projectile);
                }
            }

            for (int i = _destroyExplosionReserves.Count - 1; i >= 0; --i)
            {
                CollisionEventEmitter explosion = _destroyExplosionReserves[i];
                explosionPool.Release(explosion);
            }

            _destroyExplosionReserves.Clear();
        }

        private void Shoot(Vector2 direction)
        {
            float max = baseEulerAngleError;
            float min = -baseEulerAngleError;
            float eulerAngleRange = min + (max - min) * UnityEngine.Random.value;

            LinearProjectile projectile = projectilePool.Get();
            projectile.transform.localPosition = Vector3.zero;
            projectile.ProjectileSpeed = 3.0f;
            projectile.ProjectileEulerAngle = eulerAngleRange;
        }

        private LinearProjectile OnCreateProjectile()
        {
            GameObject gameObject = GameObject.Instantiate(projectilePrefab.gameObject);
            gameObject.transform.parent = transform;

            CollisionEventEmitter emitter = gameObject.GetComponent<CollisionEventEmitter>();
            emitter.AddHandler(OnProjectileEnter, CollisionEventType.OnTriggerEnter2D);
            emitter.AddHandler(OnProjectileStay, CollisionEventType.OnTriggerStay2D);
            emitter.AddHandler(OnProjectileExit, CollisionEventType.OnTriggerExit2D);
            
            return gameObject.GetComponent<LinearProjectile>();
        }

        private void OnGetProjectile(LinearProjectile projectile)
        {
            projectile.gameObject.SetActive(true);
            _enableProjectiles.Add(projectile);
        }

        private void OnReleaseProjectile(LinearProjectile projectile)
        {
            projectile.gameObject.SetActive(false);
            _enableProjectiles.Remove(projectile);
        }

        private void OnDestroyProjectile(LinearProjectile projectile)
        {
            // NOTE: This block is intentionally no operation.
        }

        private CollisionEventEmitter OnCreateExplosion()
        {
            GameObject gameObject = GameObject.Instantiate(explosionPrefab.gameObject);
            gameObject.transform.parent = transform;

            CollisionEventEmitter emitter = gameObject.GetComponent<CollisionEventEmitter>();
            emitter.AddHandler(OnExplosionEnter, CollisionEventType.OnTriggerEnter2D);
            emitter.AddHandler(OnExplosionStay, CollisionEventType.OnTriggerStay2D);
            emitter.AddHandler(OnExplosionExit, CollisionEventType.OnTriggerExit2D);

            return emitter;
        }

        private void OnGetExplosion(CollisionEventEmitter explosion)
        {
            explosion.gameObject.SetActive(true);
        }

        private void OnReleaseExplosion(CollisionEventEmitter explosion)
        {
            explosion.gameObject.SetActive(false);
        }

        private void OnDestroyExplosion(CollisionEventEmitter explosion)
        {
            // NOTE: This block is intentionally no operation.
        }

        private void OnProjectileEnter(GameObject projectile, Collider2D collider)
        {
            Debug.Log("Enter Projectile");
            
            // TODO: 데미지 이벤트 구조체를 만들어 타겟에게 반환합니다.
            // Pawn enemy = collider.gameObject.GetComponent<Pawn>();
            // enemy.TakeDamage(/* event structure here. */);

            Projectile proj = projectile.GetComponent<Projectile>();
            proj.ShouldDestroy = true;

            CollisionEventEmitter explosion = explosionPool.Get();
            explosion.transform.parent = transform;
            explosion.transform.position = projectile.transform.position;
        }

        private void OnProjectileStay(GameObject projectile, Collider2D collider)
        {
            // NOTE: This block is intentionally no operation.
        }

        private void OnProjectileExit(GameObject projectile, Collider2D collider)
        {
            // NOTE: This block is intentionally no operation.
        }

        private void OnExplosionEnter(GameObject explosion, Collider2D collider)
        {
            Debug.Log("Enter Explosion");

            // TODO: 데미지 이벤트 구조체를 만들어 타겟에게 반환합니다.
            // Pawn enemy = collider.gameObject.GetComponent<Pawn>();
            // enemy.TakeDamage(/* event structure here. */);

            CollisionEventEmitter emitter = explosion.GetComponent<CollisionEventEmitter>();

            if (!_destroyExplosionReserves.Contains(emitter))
                _destroyExplosionReserves.Add(emitter);
        }

        private void OnExplosionStay(GameObject projectile, Collider2D collider)
        {
            // NOTE: This block is intentionally no operation.
        }

        private void OnExplosionExit(GameObject projectile, Collider2D collider)
        {
            // NOTE: This block is intentionally no operation.
        }
    }
}
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
        private List<CollisionEventEmitter> _enableExplosions;

        [Header("Test Flag")]
        public bool flag_shoot;

        protected override void Awake()
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
            _enableExplosions = new List<CollisionEventEmitter>(explosionPoolCapacity);
        }

        private void Start()
        {
            
        }

        protected override void Update()
        {
            if (flag_shoot)
            {
                flag_shoot = false;
                UseWeapon();
            }

            for (int i = _enableProjectiles.Count - 1; i >= 0; --i)
            {
                LinearProjectile projectile = _enableProjectiles[i];

                if (projectile.FlagTable[AbilityComponent.FLAG_SHOULD_DESTROY])
                {
                    _enableProjectiles.RemoveAt(i);
                    projectilePool.Release(projectile);
                }
            }

            for (int i = _enableExplosions.Count - 1; i >= 0; --i)
            {
                CollisionEventEmitter explosion = _enableExplosions[i];

                if (explosion.FlagTable[AbilityComponent.FLAG_SHOULD_DESTROY])
                {
                    _enableExplosions.RemoveAt(i);
                    explosionPool.Release(explosion);
                }
            }
        }

        protected override void UseWeapon()
        {
            base.UseWeapon();

            LinearProjectile projectile = projectilePool.Get();
            projectile.transform.localPosition = Vector3.zero;
            projectile.ProjectileSpeed = 3.0f;

            // NOTE: Calculate nearest enemy here.
            /* Transform nearestEnemy = calculate nearest enemy here. */
            Vector3 playerPosition = Vector3.zero;
            Vector3 enemyPosition = Vector3.one;
            projectile.ProjectileDirection = Projectile.GetTargetDirectionVector(playerPosition, enemyPosition, baseEulerAngleError);
        }

        private LinearProjectile OnCreateProjectile()
        {
            GameObject gameObject = GameObject.Instantiate(projectilePrefab.gameObject);
            gameObject.transform.parent = transform;

            CollisionEventEmitter emitter = gameObject.GetComponent<CollisionEventEmitter>();
            emitter.AddHandler(OnProjectileEnter, CollisionEventType.OnTriggerEnter2D);

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
            proj.FlagTable[AbilityComponent.FLAG_SHOULD_DESTROY] = true;

            CollisionEventEmitter explosion = explosionPool.Get();
            explosion.transform.parent = transform;
            explosion.transform.position = projectile.transform.position;
        }

        private void OnExplosionEnter(GameObject explosion, Collider2D collider)
        {
            Debug.Log("Enter Explosion");

            // TODO: 데미지 이벤트 구조체를 만들어 타겟에게 반환합니다.
            // Pawn enemy = collider.gameObject.GetComponent<Pawn>();
            // enemy.TakeDamage(/* event structure here. */);

            CollisionEventEmitter emitter = explosion.GetComponent<CollisionEventEmitter>();
        }
    }
}
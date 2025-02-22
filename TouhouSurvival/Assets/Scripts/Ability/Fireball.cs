using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Unchord
{
    public class Fireball : WeaponComponent
    {
        private static int s_fireballProjectileFlyingHash = Animator.StringToHash("FireballProjectileFlying");
        private static int s_fireballExplosionHash = Animator.StringToHash("FireballExplosion");

        [Header("Shooting Properties")]
        public float baseShootingCooldown;
        public float baseProjectileSize = 1.0f;
        public float baseEulerAngleError;
        public float baseBurstDelay;
        public int baseBurstCount;

        [Header("Projectile Properties")]
        public GameObject projectilePrefab;
        public float baseProjectileSpeed;
        public float baseProjectileDamage;
        public float baseProjectileKnockbackForce;

        [Header("Explosion Properties")]
        public GameObject explosionPrefab;
        public float baseExplosionSize = 1.0f;
        public float baseExplosionDuration;
        public float baseExplosionKnockbackForce;
        public float baseExplosionDamage;

        [Header("Object Pool Setting")]
        public int projectilePoolCapacity;
        public int explosionPoolCapacity;

        private ObjectPool<GameObject> _projectilePool;
        private ObjectPool<GameObject> _explosionPool;

        protected override void Awake()
        {
            _projectilePool = new ObjectPool<GameObject>(
                OnCreateProjectile,
                OnGetProjectile,
                OnReleaseProjectile,
                OnDestroyProjectile,
                true,
                projectilePoolCapacity,
                100);
            _explosionPool = new ObjectPool<GameObject>(
                OnCreateExplosion,
                OnGetExplosion,
                OnReleaseExplosion,
                OnDestroyExplosion,
                true,
                explosionPoolCapacity,
                100);
        }

        protected override void UseWeapon()
        {
            base.UseWeapon();

            GameObject nearestEnemy = Spawner.GetNearestEnemyOrNull(_player.transform.position);

            if (nearestEnemy == null)
                return;

            GameObject projectileObject = _projectilePool.Get();

            LinearProjectile projectile = projectileObject.GetComponent<LinearProjectile>();
            projectile.transform.localPosition = Vector3.zero;
            projectile.ProjectileSpeed = 3.0f;

            Vector3 playerPosition = _player.transform.position;
            Vector3 enemyPosition = nearestEnemy.transform.position;
            projectile.ProjectileDirection = Projectile.GetTargetDirectionVector(playerPosition, enemyPosition, baseEulerAngleError);
        }

        private GameObject OnCreateProjectile()
        {
            GameObject gameObject = GameObject.Instantiate(projectilePrefab.gameObject);
            gameObject.transform.parent = transform;

            CollisionEventEmitter emitter = gameObject.transform.Find("Colliders/Circle Collider 2D").GetComponent<CollisionEventEmitter>();
            emitter.AddHandler(OnProjectileEnter, CollisionEventType.OnTriggerEnter2D);

            FlagComponent flagTable = gameObject.GetComponent<FlagComponent>();
            flagTable.AddEventTrue(AbilityComponent.FLAG_SHOULD_DESTROY, OnProjectileDestroyFlagSetTrue);

            return gameObject;
        }

        private void OnGetProjectile(GameObject projectile)
        {
            FlagComponent flagTable = projectile.GetComponent<FlagComponent>();
            flagTable.SetFlagFalseWithoutEvent(AbilityComponent.FLAG_SHOULD_DESTROY);

            projectile.gameObject.SetActive(true);

            Animator animator = projectile.GetComponent<Animator>();
            animator.Play(s_fireballProjectileFlyingHash);
        }

        private void OnReleaseProjectile(GameObject projectile)
        {
            projectile.SetActive(false);
        }

        private void OnDestroyProjectile(GameObject projectile)
        {
            // NOTE: This block is intentionally no operation.
        }

        private void OnProjectileDestroyFlagSetTrue(FlagComponent flagTable)
        {
            _projectilePool.Release(flagTable.gameObject);
        }

        private GameObject OnCreateExplosion()
        {
            GameObject gameObject = GameObject.Instantiate(explosionPrefab.gameObject);
            gameObject.transform.parent = transform;

            CollisionEventEmitter emitter = gameObject.transform.Find("Colliders/Circle Collider 2D").GetComponent<CollisionEventEmitter>();
            emitter.AddHandler(OnExplosionStay, CollisionEventType.OnTriggerStay2D);

            FlagComponent flagTable = gameObject.GetComponent<FlagComponent>();
            flagTable.AddEventTrue(AbilityComponent.FLAG_SHOULD_DESTROY, OnExplosionDestroyFlagSetTrue);

            return gameObject;
        }

        private void OnGetExplosion(GameObject explosion)
        {
            FlagComponent flagTable = explosion.GetComponent<FlagComponent>();
            flagTable.SetFlagFalseWithoutEvent(AbilityComponent.FLAG_SHOULD_DESTROY);

            explosion.gameObject.SetActive(true);

            Animator animator = explosion.GetComponent<Animator>();
            animator.Play(s_fireballExplosionHash);
        }

        private void OnReleaseExplosion(GameObject explosion)
        {
            explosion.gameObject.SetActive(false);
        }

        private void OnDestroyExplosion(GameObject explosion)
        {
            // NOTE: This block is intentionally no operation.
        }

        private void OnExplosionDestroyFlagSetTrue(FlagComponent flagTable)
        {
            _explosionPool.Release(flagTable.gameObject);
        }

        private void OnProjectileEnter(GameObject projectile, Collider2D collider)
        {
            Debug.Log("Enter Projectile");
            // TODO: 데미지 이벤트 구조체를 만들어 타겟에게 반환합니다.
            
            // Pawn enemy = collider.gameObject.GetComponent<Pawn>();
            // enemy.TakeDamage(/* event structure here. */);

            LinearProjectile proj = projectile.GetComponentInParent<LinearProjectile>();
            proj.FlagTable[AbilityComponent.FLAG_SHOULD_DESTROY] = true;

            GameObject explosion = _explosionPool.Get();
            explosion.transform.parent = transform;
            explosion.transform.position = projectile.transform.position;
        }

        private void OnExplosionStay(GameObject explosion, Collider2D collider)
        {
            Debug.Log("Enter Explosion");
            // TODO: 데미지 이벤트 구조체를 만들어 타겟에게 반환합니다.
            
            // Pawn enemy = collider.gameObject.GetComponent<Pawn>();
            // enemy.TakeDamage(/* event structure here. */);

            CollisionEventEmitter emitter = explosion.GetComponent<CollisionEventEmitter>();
        }
    }
}
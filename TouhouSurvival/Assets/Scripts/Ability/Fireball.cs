using UnityEngine;
using UnityEngine.Pool;

namespace Unchord
{
    public class Fireball : WeaponComponent
    {
        private static int s_fireballProjectileFlyingHash = Animator.StringToHash("FireballProjectileFlying");
        private static int s_fireballExplosionHash = Animator.StringToHash("FireballExplosion");

        public FireballAttributeSet Attributes { get; private set; }

        [Header("Prefab Settings")]
        public GameObject projectilePrefab;
        public GameObject explosionPrefab;

        private ObjectPool<GameObject> _projectilePool;
        private ObjectPool<GameObject> _explosionPool;

        protected override void Awake()
        {
            base.Awake();

            Attributes = GetComponent<FireballAttributeSet>();

            _projectilePool = new ObjectPool<GameObject>(
                OnCreateProjectile,
                OnGetProjectile,
                OnReleaseProjectile,
                OnDestroyProjectile,
                true,
                4,
                100);
            _explosionPool = new ObjectPool<GameObject>(
                OnCreateExplosion,
                OnGetExplosion,
                OnReleaseExplosion,
                OnDestroyExplosion,
                true,
                4,
                100);
        }

        protected override void UseWeapon()
        {
            base.UseWeapon();

            GameObject nearestEnemy = Spawner.GetNearestEnemyOrNull(_player.transform.position);

            if (!nearestEnemy)
                return;

            GameObject projectileObject = _projectilePool.Get();

            LinearProjectile projectile = projectileObject.GetComponent<LinearProjectile>();
            projectile.transform.localPosition = Vector3.zero;
            projectile.OriginPosition = projectile.transform.position;
            projectile.ProjectileSpeed = 3.0f;

            Vector3 playerPosition = _player.transform.position;
            Vector3 enemyPosition = nearestEnemy.transform.position;
            GameplayAttribute attrEulerAngleError = Attributes[FireballAttributeType.ShootingEulerAngleError];

            projectile.ProjectileDirection = Projectile.GetTargetDirectionVector(playerPosition, enemyPosition, attrEulerAngleError.CurrentValue);
        }

        private GameObject OnCreateProjectile()
        {
            GameObject projectile = GameObject.Instantiate(projectilePrefab.gameObject, transform, true);

            CollisionEventEmitter emitter = projectile.transform.Find("Colliders/Circle Collider 2D").GetComponent<CollisionEventEmitter>();
            emitter.AddHandler(OnProjectileEnter, CollisionEventType.OnTriggerEnter2D);

            FlagComponent flagTable = projectile.GetComponent<FlagComponent>();
            flagTable.AddEventTrue(AbilityComponent.FLAG_SHOULD_DESTROY, OnProjectileDestroyFlagSetTrue);

            return projectile;
        }

        private void OnGetProjectile(GameObject projectile)
        {
            FlagComponent flagTable = projectile.GetComponent<FlagComponent>();
            flagTable.SetFlagFalseWithoutEvent(AbilityComponent.FLAG_SHOULD_DESTROY);

            projectile.gameObject.SetActive(true);

            float scale = Attributes[FireballAttributeType.ProjectileSize].CurrentValue;
            projectile.transform.localScale = new Vector3(scale, scale, 1.0f);

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
            GameObject explosion = GameObject.Instantiate(explosionPrefab.gameObject, transform, true);

            CollisionEventEmitter emitter = explosion.transform.Find("Colliders/Circle Collider 2D").GetComponent<CollisionEventEmitter>();
            emitter.AddHandler(OnExplosionStay, CollisionEventType.OnTriggerStay2D);

            FlagComponent flagTable = explosion.GetComponent<FlagComponent>();
            flagTable.AddEventTrue(AbilityComponent.FLAG_SHOULD_DESTROY, OnExplosionDestroyFlagSetTrue);

            return explosion;
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

            LinearProjectile proj = projectile.GetComponentInParent<LinearProjectile>(true);
            proj.FlagTable[AbilityComponent.FLAG_SHOULD_DESTROY] = true;

            GameObject explosionObject = _explosionPool.Get();
            explosionObject.transform.parent = transform;
            explosionObject.transform.position = projectile.transform.position;

            DotProjectile explosion = explosionObject.GetComponent<DotProjectile>();
            explosion.OriginPosition = explosionObject.transform.position;
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
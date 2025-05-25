using UnityEngine;
using UnityEngine.Pool;

namespace Unchord
{
    public class MoscosSpell : SpellComponent
    {
        private static int s_fireballProjectileFlyingHash = Animator.StringToHash("FireballProjectileFlying");
        private static int s_fireballExplosionHash = Animator.StringToHash("FireballExplosion");

        [Header("Prefab Settings")]
        public GameObject projectilePrefab;
        public GameObject explosionPrefab;

        private int _count = 0;
        private float _cooldown = 0.0f;
        private float _angle;

        private ObjectPool<GameObject> _projectilePool;
        private ObjectPool<GameObject> _explosionPool;

        protected override void Awake()
        {
            base.Awake();

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

        protected override void Update()
        {
            base.Update();

            if (!base.IsCooldownPaused)
                return;

            if (_count == 0)
            {
                base.IsCooldownPaused = false;
                return;
            }

            if (_cooldown > 0.0f)
            {
                _cooldown -= Time.deltaTime;
                return;
            }

            _count--;
            _cooldown += base.AttributeBase[MoscowSpellAttributeType.BurstDelay].CurrentValue;

            int directionCount = (int)base.AttributeBase[MoscowSpellAttributeType.DirectionCount].CurrentValue;
            float dAngle = 2 * Mathf.PI / (float)directionCount;
            float rAngle = Mathf.Deg2Rad * base.AttributeBase[MoscowSpellAttributeType.RotationAnglePerTick].CurrentValue;

            _angle = (_angle + rAngle) % (2 * Mathf.PI);

            for (int i = 0; i < directionCount; ++i)
            {
                float angle = (_angle + (float)i * dAngle) % (2 * Mathf.PI);
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);

                Vector2 direction = new Vector2(cos, sin);

                GameObject projectileObject = _projectilePool.Get();

                LinearProjectile projectile = projectileObject.GetComponent<LinearProjectile>();
                projectile.transform.position = GameManager.Instance.Player.transform.position;
                projectile.ProjectileSpeed = base.AttributeBase[MoscowSpellAttributeType.ProjectileSpeed].CurrentValue;
                projectile.ProjectileDirection = direction;
                projectile.OriginEulerAngle = angle * Mathf.Rad2Deg;
            }
        }

        public override void UseSpell()
        {
            base.IsCooldownPaused = true;

            _cooldown = 0.0f;
            _count = (int)base.AttributeBase[MoscowSpellAttributeType.BurstCount].CurrentValue - 1;
        }

        private GameObject OnCreateProjectile()
        {
            GameObject projectile = GameObject.Instantiate(projectilePrefab.gameObject, GameManager.Instance.ProjectileContainer, true);

            CollisionEventEmitter emitter = projectile.transform.Find("Colliders/Circle Collider 2D").GetComponent<CollisionEventEmitter>();
            emitter.onTriggerEnter2D += OnProjectileEnter;

            FlagComponent flagTable = projectile.GetComponent<FlagComponent>();
            flagTable.AddEventTrue(AbilityComponent.FLAG_SHOULD_DESTROY, OnProjectileDestroyFlagSetTrue);

            return projectile;
        }

        private void OnGetProjectile(GameObject projectile)
        {
            FlagComponent flagTable = projectile.GetComponent<FlagComponent>();
            flagTable.SetFlagFalseWithoutEvent(AbilityComponent.FLAG_SHOULD_DESTROY);

            projectile.gameObject.SetActive(true);

            float scale = AttributeBase[MoscowSpellAttributeType.ProjectileSize].CurrentValue;
            projectile.transform.localScale = new Vector3(scale, scale, 1.0f);

            Animator animator = projectile.GetComponent<Animator>();
            animator.Play(s_fireballProjectileFlyingHash, -1, 0.0f);
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
            UnityEngine.Debug.Assert(flagTable.GetComponent<Projectile>() != null);

            GameObject explosionObject = _explosionPool.Get();
            explosionObject.transform.position = flagTable.transform.position;

            DotProjectile explosion = explosionObject.GetComponent<DotProjectile>();
            explosion.transform.position = explosionObject.transform.position;

            _projectilePool.Release(flagTable.gameObject);
        }

        private GameObject OnCreateExplosion()
        {
            GameObject explosion = GameObject.Instantiate(explosionPrefab.gameObject, GameManager.Instance.ProjectileContainer, true);

            CollisionEventEmitter emitter = explosion.transform.Find("Colliders/Circle Collider 2D").GetComponent<CollisionEventEmitter>();
            emitter.onTriggerEnter2D += OnExplosionEnter;

            FlagComponent flagTable = explosion.GetComponent<FlagComponent>();
            flagTable.AddEventTrue(AbilityComponent.FLAG_SHOULD_DESTROY, OnExplosionHit);

            return explosion;
        }

        private void OnGetExplosion(GameObject explosion)
        {
            FlagComponent flagTable = explosion.GetComponent<FlagComponent>();
            flagTable.SetFlagFalseWithoutEvent(AbilityComponent.FLAG_SHOULD_DESTROY);

            explosion.gameObject.SetActive(true);

            Animator animator = explosion.GetComponent<Animator>();
            animator.Play(s_fireballExplosionHash, -1, 0.0f);
        }

        private void OnReleaseExplosion(GameObject explosion)
        {
            explosion.gameObject.SetActive(false);
        }

        private void OnDestroyExplosion(GameObject explosion)
        {
            // NOTE: This block is intentionally no operation.
        }

        private void OnProjectileEnter(object collisionEventEmitter, CollisionEventArgs args)
        {
            GameObject enemyObject = args.targetObject;
            Enemy enemy = enemyObject.GetComponentInParent<Enemy>(true);

            UnityEngine.Debug.Assert(enemy != null);

            if (enemy.AttributeBase[EnemyAttributeType.Health].CurrentValue > 0.0f)
            {
                float damage = this.AttributeBase[MoscowSpellAttributeType.ProjectileDamage].CurrentValue;
                enemy.TakeDamage(damage, null, null);
            }

            GameObject projectileObject = args.eventSource;
            LinearProjectile projectile = projectileObject.GetComponentInParent<LinearProjectile>(true);

            UnityEngine.Debug.Assert(projectile != null);

            projectile.FlagTable[AbilityComponent.FLAG_SHOULD_DESTROY] = true;
        }

        private void OnExplosionEnter(object collisionEventEmitter, CollisionEventArgs args)
        {
            GameObject enemyObject = args.targetObject;
            Enemy enemy = enemyObject.GetComponentInParent<Enemy>();

            UnityEngine.Debug.Assert(enemy != null);

            if (enemy.AttributeBase[EnemyAttributeType.Health].CurrentValue > 0.0f)
            {
                float damage = this.AttributeBase[MoscowSpellAttributeType.ExplosionDamage].CurrentValue;
                enemy.TakeDamage(damage, null, null);
            }
        }

        private void OnExplosionHit(FlagComponent flagTable)
        {
            _explosionPool.Release(flagTable.gameObject);
        }
    }
}
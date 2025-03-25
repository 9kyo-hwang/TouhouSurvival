using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.Rendering.GPUSort;

namespace Unchord
{
    // TODO: 넉백 방향, 플레이어를 기준으로 플레이어에서 멀어지는 방향.
    public class NegPosOrb : WeaponComponent
    {
        private static int s_projectileFlyingHash = Animator.StringToHash("ProjectileFlying");

        [Header("Prefab Settings")]
        public GameObject projectilePrefab;

        private ObjectPool<GameObject> _projectilePool;

        protected override void Awake()
        {
            base.Awake();

            _projectilePool = new ObjectPool<GameObject>(
                OnCreateProjectile,
                OnGetProjectile,
                OnReleaseProjectile,
                OnDestroyProjectile,
                true,
                10,
                100);
        }

        protected override void UseWeapon()
        {
            base.UseWeapon();

            StartCoroutine(ShootCoroutine());
        }

        private IEnumerator ShootCoroutine()
        {
            _isCooltimePaused = true;

            int burstCount = (int)Attributes[NegPosOrbAttributeType.ShootingBurstCount].CurrentValue;
            float burstDelay = Attributes[NegPosOrbAttributeType.ShootingBurstDelay].CurrentValue;

            for (int i = burstCount - 1; i >= 0; --i)
            {
                GameObject nearestEnemy = Spawner.GetNearestEnemyOrNull(_player.transform.position);

                if (nearestEnemy)
                    Shoot(nearestEnemy);

                if (i > 0)
                    yield return new WaitForSeconds(burstDelay);
            }

            _isCooltimePaused = false;
        }

        private void Shoot(GameObject nearestEnemyObject)
        {
            GameObject projectileObject = _projectilePool.Get();

            LinearProjectile projectile = projectileObject.GetComponent<LinearProjectile>();
            projectile.transform.localPosition = Vector3.zero;
            projectile.OriginPosition = projectile.transform.position;
            projectile.ProjectileSpeed = 3.0f;

            Vector3 playerPosition = _player.transform.position;
            Vector3 enemyPosition = nearestEnemyObject.transform.position;
            GameplayAttribute attrEulerAngleError = Attributes[NegPosOrbAttributeType.ShootingEulerAngleError];

            projectile.ProjectileDirection = Projectile.GetTargetDirectionVector(playerPosition, enemyPosition, attrEulerAngleError.CurrentValue);

            StartCoroutine(ElapseProjectileTimeout(projectileObject));
        }

        private IEnumerator ElapseProjectileTimeout(GameObject projectileObject)
        {
            float duration = Attributes[NegPosOrbAttributeType.ProjectileDuration].CurrentValue;

            yield return new WaitForSeconds(duration);

            LinearProjectile projectile = projectileObject.GetComponentInParent<LinearProjectile>(true);

            UnityEngine.Debug.Assert(projectile != null);

            projectile.FlagTable[AbilityComponent.FLAG_SHOULD_DESTROY] = true;
        }

        private GameObject OnCreateProjectile()
        {
            GameObject projectile = GameObject.Instantiate(projectilePrefab.gameObject, transform, true);

            CollisionEventEmitter emitter = projectile.transform.Find("Colliders/Circle Collider 2D").GetComponent<CollisionEventEmitter>();
            emitter.onTriggerStay2D += OnProjectileStay;

            FlagComponent flagTable = projectile.GetComponent<FlagComponent>();
            flagTable.AddEventTrue(AbilityComponent.FLAG_SHOULD_DESTROY, OnProjectileTimeout);

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
            animator.Play(s_projectileFlyingHash, -1, 0.0f);
        }

        private void OnReleaseProjectile(GameObject projectile)
        {
            projectile.SetActive(false);
        }

        private void OnDestroyProjectile(GameObject projectile)
        {
            // NOTE: This block is intentionally no operation.
        }

        private void OnProjectileStay(object collisionEventEmitter, CollisionEventArgs args)
        {
            GameObject enemyObject = args.targetObject;
            Enemy enemy = enemyObject.GetComponentInParent<Enemy>(true);

            UnityEngine.Debug.Assert(enemy != null);

            if (enemy.Attributes[EnemyAttributeType.Health].CurrentValue > 0.0f)
            {
                float damage = this.Attributes[FireballAttributeType.ProjectileDamage].CurrentValue;
                enemy.TakeDamage(damage, null, null);
            }
        }

        private void OnProjectileTimeout(FlagComponent flagTable)
        {
            LinearProjectile projectile = flagTable.GetComponent<LinearProjectile>();

            UnityEngine.Debug.Assert(projectile != null);

            _projectilePool.Release(projectile.gameObject);
        }
    }
}
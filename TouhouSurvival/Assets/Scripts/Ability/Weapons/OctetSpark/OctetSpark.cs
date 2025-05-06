using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace Unchord
{
    public class OctetSpark : WeaponComponent
    {
        private const string FLAG_END_OF_SPARK_EMIT = "EndOfSparkEmit";

        private static int s_sparkEmitStart = Animator.StringToHash("SparkEmitStart");

        [Header("Prefab Settings")]
        public GameObject sparkPrefab;

        private ObjectPool<GameObject> _sparkPool;

        protected override void Awake()
        {
            base.Awake();

            _sparkPool = new ObjectPool<GameObject>(
                OnCreateSpark,
                OnGetSpark,
                OnReleaseSpark,
                OnDestroySpark,
                true,
                1,
                4);
        }

        public override void UseWeapon()
        {
            GameObject nearestEnemy = Player.GetNearestEnemyOrNull();

            if (!nearestEnemy)
                return;

            GameObject sparkObject = _sparkPool.Get();

            Vector3 playerPosition = Player.transform.position;
            Vector3 enemyPosition = nearestEnemy.transform.position;
            Vector3 projectileDirection = Projectile.GetTargetDirectionVector(playerPosition, enemyPosition, 0.0f);

            PlayerTrackingProjectile projectile = sparkObject.GetComponent<PlayerTrackingProjectile>();
            projectile.DeltaPosition = Vector2.zero;
            projectile.OriginEulerAngle = Vector2.SignedAngle(Vector2.right, projectileDirection);

            StartCoroutine(EmitCoroutine(sparkObject));
        }

        private IEnumerator EmitCoroutine(GameObject sparkObject)
        {
            _isCooltimePaused = true;

            float duration = Attributes[OctetSparkAttributeType.SparkDuration].CurrentValue;
            yield return new WaitForSeconds(duration);

            FlagComponent flagTable = sparkObject.GetComponent<FlagComponent>();
            flagTable[OctetSpark.FLAG_END_OF_SPARK_EMIT] = true;

            _isCooltimePaused = false;
        }

        private GameObject OnCreateSpark()
        {
            GameObject spark = GameObject.Instantiate(sparkPrefab.gameObject, GameManager.Instance.ProjectileContainer, true);

            CollisionEventEmitter emitter = spark.transform.Find("Colliders/Box Collider 2D").GetComponent<CollisionEventEmitter>();
            emitter.onTriggerStay2D += OnSparkStay;

            FlagComponent flagTable = spark.GetComponent<FlagComponent>();

            flagTable.AddEventTrue(OctetSpark.FLAG_END_OF_SPARK_EMIT, OnSparkEmitAnimationEnd);
            flagTable.AddEventTrue(AbilityComponent.FLAG_SHOULD_DESTROY, OnSparkAnimationEnd);

            PlayerTrackingProjectile projectile = spark.GetComponent<PlayerTrackingProjectile>();
            projectile.DeltaPosition = Vector2.zero;

            return spark;
        }

        private void OnGetSpark(GameObject spark)
        {
            FlagComponent flagTable = spark.GetComponent<FlagComponent>();
            flagTable.SetFlagFalseWithoutEvent(OctetSpark.FLAG_END_OF_SPARK_EMIT);
            flagTable.SetFlagFalseWithoutEvent(AbilityComponent.FLAG_SHOULD_DESTROY);

            spark.gameObject.SetActive(true);

            float scale = Attributes[OctetSparkAttributeType.SparkWidth].CurrentValue;
            spark.transform.localScale = new Vector3(1.0f, scale, 1.0f);

            Animator animator = spark.GetComponent<Animator>();
            animator.SetBool("isEmitEnd", false);
            animator.Play(s_sparkEmitStart, -1, 0.0f);
        }

        private void OnReleaseSpark(GameObject spark)
        {
            spark.SetActive(false);
        }

        private void OnDestroySpark(GameObject spark)
        {
            // NOTE: This block is intentionally no operation.
        }

        private void OnSparkStay(object collisionEventEmitter, CollisionEventArgs args)
        {
            GameObject enemyObject = args.targetObject;
            Enemy enemy = enemyObject.GetComponentInParent<Enemy>(true);

            UnityEngine.Debug.Assert(enemy != null);

            if (enemy.Attributes[EnemyAttributeType.Health].CurrentValue > 0.0f)
            {
                float damage = this.Attributes[OctetSparkAttributeType.SparkDamage].CurrentValue;
                enemy.TakeDamage(damage, null, null);
            }
        }

        private void OnSparkEmitAnimationEnd(FlagComponent flagTable)
        {
            Animator animator = flagTable.GetComponent<Animator>();
            animator.SetBool("isEmitEnd", true);
        }

        private void OnSparkAnimationEnd(FlagComponent flagTable)
        {
            _sparkPool.Release(flagTable.gameObject);
        }
    }
}
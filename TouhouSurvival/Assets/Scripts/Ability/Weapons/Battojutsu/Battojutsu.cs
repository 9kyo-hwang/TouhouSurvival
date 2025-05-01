using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Unchord
{
    public class Battojutsu : WeaponComponent
    {
        private static int s_effectBeginAnimHash = Animator.StringToHash("BattojutsuEffect");

        [Header("Prefab Settings")]
        public GameObject effectPrefab;

        private ObjectPool<GameObject> _effectPool;

        protected override void Awake()
        {
            base.Awake();

            _effectPool = new ObjectPool<GameObject>(
                OnCreateEffect,
                OnGetEffect,
                OnReleaseEffect,
                OnDestroyEffect,
                true,
                4,
                10);
        }

        public override void UseWeapon()
        {
            GameObject effectObject = _effectPool.Get();

            Vector2 posPlayer = _player.transform.position;
            GameObject nearestEnemy = _player.GetNearestEnemyOrNull();

            if (nearestEnemy == null)
            {
                _effectPool.Release(effectObject);
                return;
            }

            Vector2 posEnemy = nearestEnemy.transform.position;

            float eulerAngleZ = Vector2.SignedAngle(Vector2.right, posEnemy - posPlayer);
            effectObject.transform.eulerAngles = Vector3.forward * eulerAngleZ;
        }

        private GameObject OnCreateEffect()
        {
            GameObject effect = GameObject.Instantiate(effectPrefab.gameObject);
            effect.transform.parent = GameManager.Instance.ProjectileContainer;
            effect.transform.localPosition = Vector2.zero;

            CollisionEventEmitter emitter = effect.transform.Find("Colliders/Damaging Collider").GetComponent<CollisionEventEmitter>();
            emitter.onTriggerEnter2D += OnEffectEnter;

            FlagComponent flagTable = effect.GetComponent<FlagComponent>();
            flagTable.AddEventTrue(AbilityComponent.FLAG_SHOULD_DESTROY, OnEffectDestroyFlagSetTrue);

            return effect;
        }

        private void OnGetEffect(GameObject effect)
        {
            FlagComponent flagTable = effect.GetComponent<FlagComponent>();
            flagTable.SetFlagFalseWithoutEvent(AbilityComponent.FLAG_SHOULD_DESTROY);

            effect.gameObject.SetActive(true);

            Animator animator = effect.GetComponent<Animator>();
            animator.Play(s_effectBeginAnimHash);
        }

        private void OnReleaseEffect(GameObject effect)
        {
            effect.gameObject.SetActive(false);
        }

        private void OnDestroyEffect(GameObject effect)
        {
            // NOTE: This block is intentionally no operation.
        }

        private void OnEffectDestroyFlagSetTrue(FlagComponent flagTable)
        {
            _effectPool.Release(flagTable.gameObject);
        }

        private void OnEffectEnter(object collisionEventEmitter, CollisionEventArgs args)
        {
            GameObject enemyObject = args.targetObject;
            Enemy enemy = enemyObject.GetComponentInParent<Enemy>();

            UnityEngine.Debug.Assert(enemy != null);

            if (enemy.Attributes[EnemyAttributeType.Health].CurrentValue > 0.0f)
            {
                float damage = this.Attributes[BattojutsuAttributeType.EffectDamage].CurrentValue;
                enemy.TakeDamage(damage, null, null);
            }
        }
    }
}
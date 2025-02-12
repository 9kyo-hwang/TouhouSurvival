using UnityEngine;
using UnityEngine.Pool;

namespace Unchord
{
    public class Battojutsu : WeaponComponent
    {
        [Header("Effect Properties")]
        public GameObject effectPrefab;
        public float baseDamage;
        public float baseKnockbackForce;
        public float baseEffectScale = 1.0f;

        [Header("Object Pool Properties")]
        public int effectPoolCapacity;

        private ObjectPool<GameObject> _effectPool;
        private int _effectBeginAnimHash;

        [Header("Test Flag")]
        public bool flag_shoot;

        protected override void Awake()
        {
            _effectPool = new ObjectPool<GameObject>(
                OnCreateEffect,
                OnGetEffect,
                OnReleaseEffect,
                OnDestroyEffect,
                true,
                effectPoolCapacity,
                10);

            _effectBeginAnimHash = Animator.StringToHash("BattojutsuEffect");
        }

        protected override void Update()
        {
            if (flag_shoot)
            {
                flag_shoot = false;
                UseWeapon();
            }
        }

        protected override void UseWeapon()
        {
            base.UseWeapon();

            GameObject effectObject = _effectPool.Get();

            Vector2 posPlayer = GameManager.Instance.Player.transform.position; // TODO: Remove this line after injecting _player variable and enable below line.
            // Vector2 posPlayer = _player.transform.position;
            GameObject nearestEnemy = Spawner.GetNearestEnemyOrNull(posPlayer);

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
            effect.transform.parent = transform;
            effect.transform.localPosition = Vector2.zero;

            CollisionEventEmitter emitter = effect.transform.Find("Colliders/Damaging Collider").GetComponent<CollisionEventEmitter>();
            emitter.AddHandler(OnEffectEnter, CollisionEventType.OnTriggerEnter2D);

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
            animator.Play(_effectBeginAnimHash);
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

        private void OnEffectEnter(GameObject target, Collider2D collider)
        {
            // TODO: 데미지 이벤트 구조체를 만들어 타겟에게 반환합니다.
            // Pawn enemy = collider.gameObject.GetComponent<Pawn>();
            // enemy.TakeDamage(/* event structure here. */);
        }
    }
}
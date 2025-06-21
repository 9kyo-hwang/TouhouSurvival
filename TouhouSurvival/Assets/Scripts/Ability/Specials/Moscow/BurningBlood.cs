using UnityEngine;

namespace Unchord
{
    // 1-2
    public class BurningBlood : SpecialAbilityComponent
    {
        public float CurrentHealthRegeneration { get; set; } = 0.0f;

        private CircleCollider2D _collider;
        private CollisionEventEmitter _collisionEvent;
        private bool _isEventRegistered;

        private float _lastBleedTime;
        
        protected override void Awake()
        {
            base.Awake();

            _collider = GetComponent<CircleCollider2D>();
            _collisionEvent = GetComponent<CollisionEventEmitter>();
        }

        protected override void Start()
        {
            base.Start();

            _lastBleedTime = 0.0f;

            _collisionEvent.onTriggerStay2D += OnBlood;
            _isEventRegistered = true;

            _collider.radius = base.AttributeBase["BloodAreaSize"].CurrentValue;
        }

        protected override void Update()
        {
            base.Update();

            float currentTime = GameManager.Instance.AbsolutePlaytime;
            float cooldown = base.AttributeBase["BloodCooldown"].CurrentValue;

            if (currentTime - _lastBleedTime < cooldown)
                return;

            _lastBleedTime = currentTime;

            if (!_isEventRegistered)
            {
                _collisionEvent.onTriggerStay2D += OnBlood;
                _isEventRegistered = true;
            }
        }

        private void OnBlood(object sender, CollisionEventArgs args)
        {
            if (_isEventRegistered)
            {
                _isEventRegistered = false;
                _collisionEvent.onTriggerStay2D -= OnBlood;
            }

            Enemy enemy = args.targetObject.GetComponentInParent<Enemy>();

            UnityEngine.Debug.Assert(enemy != null);

            float damage = CurrentHealthRegeneration * base.AttributeBase["BloodDamage"].CurrentValue;

            enemy.TakeDamage(damage, null, null);
        }
    }
}
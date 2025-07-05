using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    // 1-2
    public class BurningBlood : SpecialAbilityComponent
    {
        private CircleCollider2D _collider;
        private CollisionEventEmitter _collisionEvent;
        
        private float _bleedTime;
        private bool _shouldBleed;
        private List<Enemy> _enteredEnemies;

        protected override void Awake()
        {
            base.Awake();

            _collider = GetComponent<CircleCollider2D>();
            _collisionEvent = GetComponent<CollisionEventEmitter>();

            _enteredEnemies = new List<Enemy>(16);
        }

        protected override void Start()
        {
            base.Start();

            _collisionEvent.onTriggerEnter2D += OnEnterArea;
            _collisionEvent.onTriggerExit2D += OnExitArea;
            
            _collider.radius = base.AttributeBase["BloodAreaSize"].CurrentValue;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!_shouldBleed)
                return;

            _shouldBleed = false;

            _collider.radius = base.AttributeBase["BloodAreaSize"].CurrentValue;

            for (int i = _enteredEnemies.Count - 1; i >= 0; --i)
            {
                Enemy enemy = _enteredEnemies[i];

                float healthRegen = base.Player.AttributeBase[PlayerAttributeType.HpRegen].CurrentValue;
                float damage = healthRegen * base.AttributeBase["BloodDamage"].CurrentValue;

                enemy.TakeDamage(damage);
            }
        }

        protected override void Update()
        {
            base.Update();

            float currentTime = GameManager.Instance.AbsolutePlaytime;

            if (currentTime < _bleedTime)
                return;

            _bleedTime += base.AttributeBase["BloodCooldown"].CurrentValue;

            _shouldBleed = true;
        }

        private void OnEnterArea(object sender, CollisionEventArgs args)
        {
            Enemy enemy = args.targetObject.GetComponentInParent<Enemy>();

            UnityEngine.Debug.Assert(enemy != null);

            _enteredEnemies.Add(enemy);
        }

        private void OnExitArea(object sender, CollisionEventArgs args)
        {
            Enemy enemy = args.targetObject.GetComponentInParent<Enemy>();

            UnityEngine.Debug.Assert(enemy != null);

            _enteredEnemies.Remove(enemy);
        }
    }
}
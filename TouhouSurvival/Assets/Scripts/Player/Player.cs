using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Unchord
{
    public class Player : Pawn
    {
        private Vector2 _movementVector;
        public PlayerAttributeSet AttributeSet { get; private set; }
        public PlayerLevelSystem LevelSystem { get; private set; }
        public SpecialAbilityComponent SpecialAbility { get; private set; }
        
        private AbilitySelectUIHandler _abilitySelectUI;
        private AbilityManager _abilityManager;

        public Transform WeaponTransform  { get; private set; }
        public Transform PassiveTransform { get; private  set; }
        public Transform SpellTransform { get; private  set; }

        private float _currentHealth;

        private float _lastSpellUsingTime;
        private float _currentSpellGauge;

        protected override void Awake()
        {
            base.Awake();
            
            AttributeSet = gameObject.GetComponent<PlayerAttributeSet>();
            LevelSystem = new PlayerLevelSystem();
            SpecialAbility = GetComponent<SpecialAbilityComponent>();

            _abilitySelectUI = new AbilitySelectUIHandler();
            _abilityManager = GetComponent<AbilityManager>();
            
            WeaponTransform = transform.Find($"Abilities/Weapons");
            PassiveTransform = transform.Find($"Abilities/Passives");
            SpellTransform = transform.Find($"Abilities/Spells");
        }

        protected override void Start()
        {
            base.Start();
            
            AttributeSet.Initialize(LevelSystem);
            LevelSystem.Initialize(AttributeSet);
            LevelSystem.OnLevelUp += (sender, args) =>
            {
                GameManager.Instance.BlockingEvent.Publish(
                    _abilitySelectUI.WaitForSelection(_abilityManager, SpecialAbility, args.CurrentLevel)
                );
            };

            SpecialAbility.Subscribe(
                this.AttributeSet,
                _abilityManager.MainWeapon.Attributes,
                _abilityManager.MainSpell.Attributes);

            _currentHealth = AttributeSet[PlayerAttributeType.Health].CurrentValue;

            _lastSpellUsingTime = float.MinValue;
            _currentSpellGauge = 0.0f;
        }

        protected override void Update()
        {
            base.Update();

            Animator.SetFloat("Health", AttributeSet[PlayerAttributeType.Health].CurrentValue);
            Animator.SetBool("IsMove", _movementVector.magnitude > 0.0f);

            UpdateCurrentSpellGauge();

            if (Input.GetKeyDown(KeyCode.F4))
            {
                Debug.Log("Get 1 Exp.");
                LevelSystem.Experience += 1;
            }
        }

        private void FixedUpdate()
        {
            float movementSpeed = AttributeSet[PlayerAttributeType.MovementSpeed].CurrentValue;
            Vector2 next = _movementVector * (movementSpeed * Time.fixedDeltaTime);
            Rigidbody.MovePosition(Rigidbody.position + next);
        }

        protected override void LateUpdate()
        {
            Animator.SetFloat("Speed", _movementVector.magnitude);
            if (_movementVector.x != 0)
            {
                float angle = _movementVector.x < 0 ? 0.0f : 180.0f;
                Renderers.eulerAngles = Vector3.up * angle;
                Colliders.eulerAngles = Vector3.up * angle;
            }
        }
        
        private void OnMove(InputValue value)
        {
            // Input Setting에서 이미 값을 Normalized된 상태로 받도록 세팅됨
            Debug.Log("OnMove");
            _movementVector = value.Get<Vector2>();
        }

        private void OnSpell(InputValue value)
        {
            Debug.Log("OnSpell");
            float currentTime = GameManager.Instance.AbsolutePlaytime;
            float cooldown = this.AttributeSet[PlayerAttributeType.SpellCooldown].CurrentValue;

            if (SpellTransform.childCount == 0 ||
                _lastSpellUsingTime + cooldown > currentTime ||
                _currentSpellGauge < 1.0f
            )
            {
                return;
            }

            _lastSpellUsingTime = currentTime;

            SpellComponent spell = SpellTransform.GetChild(0).GetComponent<SpellComponent>();
            _currentSpellGauge -= 1.0f;
            spell.UseSpell();
        }

        private void UpdateCurrentSpellGauge()
        {
            float delta = 1 / AttributeSet[PlayerAttributeType.SpellAutoRechargeTime].CurrentValue;
            float dt = Time.deltaTime;
            float max = (int)AttributeSet[PlayerAttributeType.MaxSpellCount].CurrentValue;

            _currentSpellGauge = Mathf.Clamp(_currentSpellGauge + delta * dt, 0.0f, max);
        }

        public GameObject GetNearestEnemyOrNull()
        {
            Vector2 originPosition = transform.position;
            GameObject selected = null;
            List<GameObject> spawnedEnemies = GameManager.Instance.SpawnedEnemies;

            for (int i = spawnedEnemies.Count - 1; i >= 0; --i)
            {
                if (!spawnedEnemies[i])
                {
                    spawnedEnemies.RemoveAt(i);
                    continue;
                }

                if (!selected)
                {
                    selected = spawnedEnemies[i];
                    continue;
                }

                Vector2 diffTarget = (Vector2)spawnedEnemies[i].transform.position - originPosition;
                Vector2 diffSelected = (Vector2)selected.transform.position - originPosition;

                if (diffTarget.sqrMagnitude < diffSelected.sqrMagnitude)
                {
                    selected = spawnedEnemies[i];
                }
            }

            return selected;
        }

        public override float TakeDamage(float damageAmount, Pawn eventInstigator, GameObject damageCauser)
        {
            if (!AttributeSet)
            {
                Debug.Assert(false, "Player has no attribute set");
                return 0f;
            }

            GameplayAttribute maxHealth = AttributeSet[PlayerAttributeType.Health];
            GameplayAttribute defense = AttributeSet[PlayerAttributeType.Defense];

            float currentHealth = _currentHealth;
            float currentDefense = defense.CurrentValue;
            damageAmount -= currentDefense;

            _currentHealth = Mathf.Clamp(_currentHealth - damageAmount, 0.0f, maxHealth.CurrentValue);
            float newHealth = _currentHealth;
            
            Debug.Log($"플레이어가 {damageAmount} 피해를 입었습니다. 체력: {currentHealth} -> {newHealth}");
            return damageAmount;
        }
    }
}

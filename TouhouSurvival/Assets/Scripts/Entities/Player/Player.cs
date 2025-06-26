using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Unchord
{
    public class Player : Pawn
    {
        public PlayerLevelSystem LevelSystem { get; private set; }

        public AttributeBaseSet AttributeBase { get; private set; }
        private AttributeModifierSet _attributeModifier;
        
        private AbilitySelectUIHandler _abilitySelectUI;
        private AbilityManager _abilityManager;

        #region Ability Component Containers
        public Transform WeaponTransform  { get; private set; }
        public Transform PassiveTransform { get; private  set; }
        public Transform SpellTransform { get; private  set; }
        public Transform SpecialTransform0 { get; private set; }
        public Transform SpecialTransform1 { get; private set; }
        #endregion

        #region Xlsx File Path (relative path from 'StreamingAssets')
        public string attributeXlsxPathRelative;
        public string expXlsxPathRelative; // TODO: 아직 미구현, 추후 사용 예정.
        #endregion

        public float CurrentHealth => _currentHealth;
        public float _currentHealth;

        private Vector2 _movementVector;

        private float _lastSpellUsingTime;
        private float _currentSpellGauge;

        protected override void Awake()
        {
            base.Awake();

            LevelSystem = new PlayerLevelSystem();

            string[] attrCsvPaths = AttributeUtility.ConvertXlsxToCsv(attributeXlsxPathRelative);
            this.AttributeBase = AttributeBaseSet.LoadFromFile(attrCsvPaths[0]);
            this._attributeModifier = AttributeModifierSet.LoadFromFile(attrCsvPaths[1]);
            
            _abilitySelectUI = new AbilitySelectUIHandler();
            _abilityManager = GetComponent<AbilityManager>();
            
            WeaponTransform = transform.Find($"Abilities/Weapons");
            PassiveTransform = transform.Find($"Abilities/Passives");
            SpellTransform = transform.Find($"Abilities/Spells");
            SpecialTransform0 = transform.Find($"Abilities/Specials0");
            SpecialTransform1 = transform.Find($"Abilities/Specials1");
        }

        protected override void Start()
        {
            base.Start();

            LevelSystem.ExpGainIncreaseAttribute = AttributeBase[PlayerAttributeType.ExpGain];
            LevelSystem.OnLevelUp += this.OnLevelUp;
            LevelSystem.OnExperienceChanged += this.OnExpChanged;

            AttributeBase[PlayerAttributeType.HpMax].OnAttributeChanged += this.OnHealthChanged;

            _currentHealth = AttributeBase[PlayerAttributeType.HpMax].CurrentValue;

            _lastSpellUsingTime = float.MinValue;
            _currentSpellGauge = 0.0f;

            UIManager uiManager = UIManager.Instance;
            uiManager.GameCanvas.SetPlayerLevel(LevelSystem.Level);
            uiManager.GameCanvas.SetExpGauge(LevelSystem.Experience, LevelSystem.TotalExperienceForCurrentLevel);

            WorldUIManager wuiManager = WorldUIManager.Instance;
            wuiManager.SetPlayerHealthPosition(transform.position + Vector3.up * 0.7f);
            wuiManager.SetPlayerHealthValue(AttributeBase[PlayerAttributeType.HpMax].CurrentValue, 10.0f);
        }

        private void OnLevelUp(object sender, LevelUpEventArgs args)
        {
            UIManager um = UIManager.Instance;
            GameManager gm = GameManager.Instance;

            AttributeBase.ApplyModifiers(_attributeModifier[args.CurrentLevel]);

            um.GameCanvas.SetPlayerLevel(args.CurrentLevel);
            gm.BlockingEvent.Publish(_abilitySelectUI.WaitForSelection(_abilityManager, args.CurrentLevel));
        }

        private void OnExpChanged(object sender, ExperienceChangedEventArgs args)
        {
            UIManager um = UIManager.Instance;

            um.GameCanvas.SetExpGauge(args.CurrentExperience, args.TotalExperience);
        }

        private void OnHealthChanged(object sender, AttributeChangedEventArgs args)
        {
            WorldUIManager wum = WorldUIManager.Instance;

            wum.SetPlayerHealthValue(AttributeBase[PlayerAttributeType.HpMax].CurrentValue, 10.0f);
        }

        protected override void Update()
        {
            base.Update();

            Animator.SetFloat("Health", AttributeBase[PlayerAttributeType.HpMax].CurrentValue);
            Animator.SetBool("IsMove", _movementVector.magnitude > 0.0f);

            UpdateCurrentSpellGauge();

            WorldUIManager wuiManager = WorldUIManager.Instance;
            wuiManager.SetPlayerHealthPosition(transform.position + Vector3.up * 0.7f);

            if (Input.GetKeyDown(KeyCode.F4))
            {
                Debug.Log("Get 1 Exp.");
                LevelSystem.Experience += 1;
            }

            if (Input.GetKeyDown(KeyCode.F5))
            {
                _currentHealth -= 1.0f;
            }

            if (Input.GetKeyDown(KeyCode.F6))
            {
                _currentHealth += 1.0f;
            }
        }

        private void FixedUpdate()
        {
            float movementSpeed = AttributeBase[PlayerAttributeType.Speed].CurrentValue;
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
            float cooldown = this.AttributeBase[PlayerAttributeType.SpellCooldown].CurrentValue;

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
            float delta = 1 / AttributeBase[PlayerAttributeType.SpellAutoRechargeTime].CurrentValue;
            float dt = Time.deltaTime;
            float max = (int)AttributeBase[PlayerAttributeType.MaxSpellCount].CurrentValue;

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
            if (AttributeBase == null)
            {
                Debug.Assert(false, "Player has no attribute set");
                return 0f;
            }

            GameplayAttribute maxHealth = AttributeBase[PlayerAttributeType.HpMax];
            GameplayAttribute defense = AttributeBase[PlayerAttributeType.Armor];

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

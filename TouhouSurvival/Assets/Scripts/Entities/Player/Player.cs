using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Unchord
{
    public class Player : Pawn
    {
        private static int s_playerDeadHash = Animator.StringToHash("PlayerDead");

        public ExpRequirementDictionary LevelSystem { get; private set; }

        public AttributeBaseSet AttributeBase { get; private set; }
        private AttributeModifierSet _attributeModifier;

        public bool IsStarted { get; private set; } = false;
        
        private AbilitySelectUIHandler _abilitySelectUI;
        private AbilityManager _abilityManager;

        #region Player's Icon for SelectCharacterCanvas GUI
        public string descCharacterName;
        public Sprite iconCharacter;
        public Sprite iconMainWeapon;
        public Sprite iconSpell;
        public Sprite iconSpecial;
        public Sprite iconPreview;
        public string descMainWeapon;
        public string descSpecialAbility;
        #endregion

        #region Ability Component Containers
        public Transform WeaponTransform  { get; private set; }
        public Transform PassiveTransform { get; private  set; }
        public Transform SpellTransform { get; private  set; }
        public Transform SpecialTransform0 { get; private set; }
        public Transform SpecialTransform1 { get; private set; }
        #endregion

        [Tooltip("Root path is UnityEngine.Application.streamingAssetsPath. Relative path must be start with slash(/) character.")]
        public string dataFilePathRelative;

        public float CurrentHealth => _currentHealth;
        private float _currentHealth;

        private Vector2 _movementVector;

        private float _lastSpellUsingTime;
        private float _currentSpellGauge;

        protected override void Awake()
        {
            base.Awake();

            FileStream fs = new FileStream(Application.streamingAssetsPath + this.dataFilePathRelative, FileMode.Open, FileAccess.Read, FileShare.Read);
            MultiCSVReader rd = new MultiCSVReader(fs);

            this.AttributeBase = new AttributeBaseSet(rd);
            this._attributeModifier = new AttributeModifierSet(rd);
            this.LevelSystem = new ExpRequirementDictionary(rd);

            rd.Close();
            fs.Close();

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

            AttributeBase[PlayerAttributeType.HpMax].onAttributeChanged += this.OnMaxHealthChanged;

            _currentHealth = AttributeBase[PlayerAttributeType.HpMax].CurrentValue;

            _lastSpellUsingTime = float.MinValue;
            _currentSpellGauge = 0.0f;

            UIManager uiManager = UIManager.Instance;
            uiManager.GameCanvas.SetPlayerLevel(LevelSystem.Level);
            uiManager.GameCanvas.SetExpGauge(LevelSystem.Experience, LevelSystem.TotalExperienceForCurrentLevel);

            WorldUIManager wuiManager = WorldUIManager.Instance;
            wuiManager.SetPlayerHealthPosition(transform.position + Vector3.up * 0.7f);
            wuiManager.SetPlayerHealthValue(AttributeBase[PlayerAttributeType.HpMax].CurrentValue, 10.0f);

            IsStarted = true;
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

        private void OnMaxHealthChanged(object sender, AttributeChangedEventArgs args)
        {
            WorldUIManager wum = WorldUIManager.Instance;

            float h = _currentHealth;
            float hmax = args.NewValue;

            wum.SetPlayerHealthValue(h, hmax);
        }

        private void OnHealthChanged(object sender, EventArgs args)
        {
            // NOTE: args 파라미터는 현재 사용하지 않음.

            WorldUIManager wum = WorldUIManager.Instance;

            float h = _currentHealth;
            float hmax = AttributeBase[PlayerAttributeType.HpMax].CurrentValue;

            wum.SetPlayerHealthValue(h, hmax);
        }

        protected override void Update()
        {
            base.Update();

            Animator.SetBool("IsDead", _currentHealth <= 0.0f);
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
                TakeTrueDamage(1.0f);
            }

            if (Input.GetKeyDown(KeyCode.F6))
            {
                TakeTrueDamage(-1.0f);
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

        public void AddSpellGauge(float amount)
        {
            UnityEngine.Debug.Assert(amount >= 0.0f);

            float max = (int)AttributeBase[PlayerAttributeType.MaxSpellCount].CurrentValue;

            _currentSpellGauge = Mathf.Min(_currentSpellGauge + amount, max);
        }

        private void UpdateCurrentSpellGauge()
        {
            float delta = 1 / AttributeBase[PlayerAttributeType.SpellAutoRechargeTime].CurrentValue;
            float dt = Time.deltaTime;
            float max = (int)AttributeBase[PlayerAttributeType.MaxSpellCount].CurrentValue;

            _currentSpellGauge = Mathf.Clamp(_currentSpellGauge + delta * dt, 0.0f, max);
        }

        public bool IsDeadAnimationEnd()
        {
            AnimatorStateInfo state = base.Animator.GetCurrentAnimatorStateInfo(0);

            return (state.shortNameHash == s_playerDeadHash && state.normalizedTime >= 1.0f);
        }

        public void Resurrect()
        {
            _currentHealth = AttributeBase[PlayerAttributeType.HpMax].CurrentValue;
        }

        public GameObject GetNearestEnemyOrNull()
        {
            Vector2 originPosition = transform.position;
            GameObject selected = null;
            List<GameObject> spawnedEnemies = GameManager.Instance.PhaseRuntimeCommonData.spawnedObjects;

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

        public void AddHealth(float amount)
        {
            UnityEngine.Debug.Assert(amount >= 0.0f);

            GameplayAttribute maxHealth = AttributeBase[PlayerAttributeType.HpMax];

            _currentHealth = Mathf.Min(_currentHealth + amount, maxHealth.CurrentValue);
        }

        public override float TakeDamage(float damageAmount)
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

            // TODO: 이벤트 변수로 빼는 방안을 고려함.
            OnHealthChanged(this, null);
            
            Debug.Log($"플레이어가 {damageAmount} 피해를 입었습니다. 체력: {currentHealth} -> {newHealth}");
            return damageAmount;
        }

        public override float TakeTrueDamage(float damageAmount)
        {
            GameplayAttribute maxHealth = AttributeBase[PlayerAttributeType.HpMax];

            float currentHealth = _currentHealth;
            _currentHealth = Mathf.Clamp(_currentHealth - damageAmount, 0.0f, maxHealth.CurrentValue);

            // TODO: 이벤트 변수로 빼는 방안을 고려함.
            OnHealthChanged(this, null);

            Debug.Log($"플레이어가 {damageAmount} 고정 피해를 입었습니다. 체력: {currentHealth} -> {_currentHealth}");
            return damageAmount;
        }
    }
}

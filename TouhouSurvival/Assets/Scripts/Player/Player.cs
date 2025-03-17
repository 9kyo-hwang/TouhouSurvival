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
        
        private const int INITIAL_SAMPLE_POOL_CAPACITY = 32;

        public List<string> weaponSet;
        public List<string> passiveSet;

        public int maxWeaponCount = 6;
        public int maxPassiveCount = 6;

        private List<AbilityComponent> _samplePool;
        private Dictionary<Transform, AbilitySamplingOptions> _samplingOptions;

        public Transform WeaponTransform  { get; private set; }
        public Transform PassiveTransform { get; private  set; }
        public int EnabledWeaponCount => _samplingOptions[WeaponTransform].enabledCount;
        public int EnabledPassiveCount => _samplingOptions[PassiveTransform].enabledCount;
        
        protected override void Awake()
        {
            base.Awake();
            
            AttributeSet = gameObject.GetComponent<PlayerAttributeSet>();
            WeaponTransform = transform.Find($"Abilities/Weapons");
            PassiveTransform = transform.Find($"Abilities/Passives");
            
            _samplePool = new List<AbilityComponent>(INITIAL_SAMPLE_POOL_CAPACITY);
            _samplingOptions = new Dictionary<Transform, AbilitySamplingOptions>(2)
            {
                { WeaponTransform, new AbilitySamplingOptions(maxWeaponCount) },
                { PassiveTransform, new AbilitySamplingOptions(maxPassiveCount) }
            };

            CreateAbilities();

            // AttributeSet.onLevelUp = OnLevelUp;

            AttributeSet.OnLevelUp += (level, remainingExp, requiredExp) =>
            {
                GameManager.Instance.BlockingEvent.Publish(OnLevelUp());
            };
        }

        protected override void Start()
        {
            base.Start();

            int level = (int)AttributeSet.Level;
        }

        protected override void Update()
        {
            base.Update();
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

        public override float TakeDamage(float damageAmount, Pawn eventInstigator, GameObject damageCauser)
        {
            if (!AttributeSet)
            {
                Debug.Assert(false, "Player has no attribute set");
                return 0f;
            }

            GameplayAttribute health = AttributeSet[PlayerAttributeType.Health];
            GameplayAttribute defense = AttributeSet[PlayerAttributeType.Defense];
            
            float currentHealth = health.CurrentValue;
            float currentDefense = defense.CurrentValue;
            damageAmount -= currentDefense;
            
            health.CurrentValue -= damageAmount;
            float newHealth = health.CurrentValue;
            
            Debug.Log($"플레이어가 {damageAmount} 피해를 입었습니다. 체력: {currentHealth} -> {newHealth}");
            return damageAmount;
        }

        private IEnumerator OnLevelUp()
        {
            LevelUpCanvas levelUpCanvas = UIManager.Instance.LevelUpCanvas;
            levelUpCanvas.Clear();

            List<AbilityComponent> sampledAbilities = SampleAbility(3);
            foreach (AbilityComponent sampledAbility in sampledAbilities)
            {
                levelUpCanvas.Add(sampledAbility);
            }

            levelUpCanvas.Show();
            yield return new WaitWhile(() => levelUpCanvas.SelectedIndex < 0);
            levelUpCanvas.Hide();

            if (sampledAbilities.Count == 0)
                yield break;

            int selectedIndex = levelUpCanvas.SelectedIndex;
            AbilityComponent selectedAbility = sampledAbilities[selectedIndex];

            selectedAbility.Attributes.Level += 1;
            selectedAbility.SortSiblingIndex();
            selectedAbility.gameObject.SetActive(true);

            int siblingIndex = selectedAbility.transform.GetSiblingIndex();

            GameCanvas gameCanvas = UIManager.Instance.GameCanvas;
            gameCanvas.EnableWeaponSlot(siblingIndex);
            gameCanvas.SetWeaponIcon(siblingIndex, selectedAbility.DisplayIcon);
            gameCanvas.SetWeaponLevel(siblingIndex, selectedAbility.Attributes.Level);
        }
        
        #region Ability Pool Management
        
        private class AbilitySamplingOptions
        {
            public int enabledCount;
            public int maxLevelReachedCount;
            public int maxAbilityCount;
            public List<AbilityComponent> samplePool;

            public AbilitySamplingOptions(int maxAbilityCount)
            {
                enabledCount = 0;
                maxLevelReachedCount = 0;
                this.maxAbilityCount = maxAbilityCount;
                samplePool = new List<AbilityComponent>(INITIAL_SAMPLE_POOL_CAPACITY / 2);
            }

            public void HideAllFrom(List<AbilityComponent> globalSamplePool)
            {
                for (int i = globalSamplePool.Count - 1; i >= 0; --i)
                {
                    if (!samplePool.Contains(globalSamplePool[i]))
                        continue;

                    globalSamplePool.RemoveAt(i);
                }
            }

            public void RevokeAllAt(List<AbilityComponent> globalSamplePool)
            {
                for (int i = 0; i < samplePool.Count; ++i)
                {
                    if (globalSamplePool.Contains(samplePool[i]))
                        continue;

                    globalSamplePool.Add(samplePool[i]);
                }
            }
        }

        private void CreateAbilities()
        {
            CreateAbility(AbilityType.Weapon, weaponSet[0], true, 1);

            for (int i = 1; i < weaponSet.Count; ++i)
                CreateAbility(AbilityType.Weapon, weaponSet[i], false, 0);

            for (int i = 0; i < passiveSet.Count; ++i)
                CreateAbility(AbilityType.Passive, passiveSet[i], false, 0);
        }

        private void CreateAbility(AbilityType abilityType, string abilityName, bool initialActive = false, int initialLevel = 0)
        {
            string strAbilityType = abilityType.ToString();
            string resourcePath = $"Prefabs/Abilities/{strAbilityType}s/{abilityName}/{abilityName}";
            AbilityComponent abilityComponent = Resources.Load<AbilityComponent>(resourcePath);

            if (!abilityComponent)
                return;
            abilityComponent = Instantiate(abilityComponent);

            Debug.Assert(initialLevel >= 0);

            Transform abilityContainer = transform.Find($"Abilities/{strAbilityType}s");

            abilityComponent.transform.SetParent(abilityContainer, true);
            abilityComponent.transform.localPosition = Vector3.zero;
            abilityComponent.Subscribe(this);
            _samplePool.Add(abilityComponent);
            _samplingOptions[abilityContainer].samplePool.Add(abilityComponent);
            abilityComponent.gameObject.SetActive(initialActive);
            abilityComponent.Attributes.Level = initialLevel;
            
            if (initialActive)
            {
                abilityComponent.SortSiblingIndex();
                GameCanvas gameCanvas = UIManager.Instance.GameCanvas;
                gameCanvas.SetWeaponIcon(abilityComponent.transform.GetSiblingIndex(), abilityComponent.DisplayIcon);
                gameCanvas.SetWeaponLevel(abilityComponent.transform.GetSiblingIndex(), abilityComponent.Attributes.Level);
            }
        }

        public List<AbilityComponent> SampleAbility(int samplingCount)
        {
            UnityEngine.Debug.Assert(samplingCount > 0);

            List<AbilityComponent> sampleAbilities = new List<AbilityComponent>(samplingCount);

            for (int i = _samplePool.Count - 1; i >= 0; --i)
            {
                int j = UnityEngine.Random.Range(0, i + 1);

                (_samplePool[i], _samplePool[j]) = (_samplePool[j], _samplePool[i]);

                if (i < samplingCount)
                {
                    sampleAbilities.Add(_samplePool[i]);
                }
            }

            UnityEngine.Debug.Assert(sampleAbilities != null && sampleAbilities.Capacity == samplingCount);
            return sampleAbilities;
        }

        public void OnChangeAbilityLevel(AbilityComponent abilityComponent, int prevLevel, int nextLevel)
        {
            UnityEngine.Debug.Assert(prevLevel != nextLevel);
            UnityEngine.Debug.Assert(prevLevel >= 0);
            UnityEngine.Debug.Assert(nextLevel >= 0);

            int maxLevel = abilityComponent.Attributes.MaxLevel;

            AbilitySamplingOptions options = _samplingOptions[abilityComponent.transform.parent];

            if (prevLevel < nextLevel)
            {
                if (prevLevel == 0)
                {
                    options.enabledCount++;
                }
                if (prevLevel < maxLevel && maxLevel <= nextLevel)
                {
                    ++options.maxLevelReachedCount;
                    _samplePool.Remove(abilityComponent);

                    if (options.maxLevelReachedCount == options.maxAbilityCount ||
                        options.maxLevelReachedCount == options.samplePool.Count)
                    {
                        options.HideAllFrom(_samplePool);
                    }
                }
            }
            else
            {
                if (nextLevel == 0)
                {
                    options.enabledCount--;
                }
                if (nextLevel < maxLevel && maxLevel <= prevLevel)
                {
                    _samplePool.Add(abilityComponent);

                    if (options.maxLevelReachedCount == options.maxAbilityCount ||
                        options.maxLevelReachedCount == options.samplePool.Count)
                    {
                        options.RevokeAllAt(_samplePool);
                    }

                    --options.maxLevelReachedCount;
                }
            }
        }
        #endregion
    }
}


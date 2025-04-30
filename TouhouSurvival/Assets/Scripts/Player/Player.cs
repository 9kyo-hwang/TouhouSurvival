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
        
        private const int INITIAL_SAMPLE_POOL_CAPACITY = 32;

        public List<string> weaponSet;
        public List<string> passiveSet;

        public int maxWeaponCount = 6;
        public int maxPassiveCount = 6;

        private List<AbilityComponent> _samplePool;
        private Dictionary<Transform, AbilitySamplingOptions> _samplingOptions;
        
        private AbilitySelectUIHandler _abilitySelectUI;

        public Transform WeaponTransform  { get; private set; }
        public Transform PassiveTransform { get; private  set; }
        public int EnabledWeaponCount => _samplingOptions[WeaponTransform].enabledCount;
        public int EnabledPassiveCount => _samplingOptions[PassiveTransform].enabledCount;
        
        protected override void Awake()
        {
            base.Awake();
            
            AttributeSet = gameObject.GetComponent<PlayerAttributeSet>();
            LevelSystem = new PlayerLevelSystem();
            _abilitySelectUI = new AbilitySelectUIHandler();
            
            WeaponTransform = transform.Find($"Abilities/Weapons");
            PassiveTransform = transform.Find($"Abilities/Passives");
            
            _samplePool = new List<AbilityComponent>(INITIAL_SAMPLE_POOL_CAPACITY);
            _samplingOptions = new Dictionary<Transform, AbilitySamplingOptions>(2)
            {
                { WeaponTransform, new AbilitySamplingOptions(maxWeaponCount) },
                { PassiveTransform, new AbilitySamplingOptions(maxPassiveCount) }
            };
        }

        protected override void Start()
        {
            base.Start();
            
            AttributeSet.Initialize(LevelSystem);
            LevelSystem.Initialize(AttributeSet);
            LevelSystem.OnLevelUp += (sender, args) =>
            {
                GameManager.Instance.BlockingEvent.Publish(
                    _abilitySelectUI.WaitForSelection(SampleAbility(3))
                );
            };
            
            CreateAbilities();
        }

        protected override void Update()
        {
            base.Update();

            Animator.SetFloat("Health", AttributeSet[PlayerAttributeType.Health].CurrentValue);
            Animator.SetBool("IsMove", _movementVector.magnitude > 0.0f);
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
            foreach(string weaponName in weaponSet)
            {
                if (weaponName == weaponSet[0])
                {
                    CreateAbility(AbilityType.Weapon, weaponName, true, 1);
                    UIManager.Instance.GameCanvas.EnableWeaponSlot(0);
                    continue;
                }
                
                CreateAbility(AbilityType.Weapon, weaponName);
            }

            foreach (string passiveName in passiveSet)
            {
                CreateAbility(AbilityType.Passive, passiveName);
            }
        }

        private void CreateAbility(string abilityType, string abilityName, bool active = false, int level = 0)
        {
            string path = $"Prefabs/Abilities/{abilityType}s/{abilityName}/{abilityName}";
            AbilityComponent ability = Resources.Load<AbilityComponent>(path);
            if (!ability)
            {
                return;
            }

            Transform container = transform.Find($"Abilities/{abilityType}s");
            ability = Instantiate(ability, container, true);
            ability.gameObject.SetActive(true);

            Debug.Assert(level >= 0);

            ability.transform.localPosition = Vector3.zero;
            ability.Subscribe(this);
            _samplePool.Add(ability);
            _samplingOptions[container].samplePool.Add(ability);
            ability.Attributes.Level = level;
            
            if (active)
            {
                ability.SortSiblingIndex();
                GameCanvas gameCanvas = UIManager.Instance.GameCanvas;
                gameCanvas.SetWeaponIcon(ability.transform.GetSiblingIndex(), ability.DisplayIcon);
                gameCanvas.SetWeaponLevel(ability.transform.GetSiblingIndex(), ability.Attributes.Level);
            }

            ability.gameObject.SetActive(active);
        }

        public List<AbilityComponent> SampleAbility(int samplingCount)
        {
            UnityEngine.Debug.Assert(samplingCount > 0);

            for (int i = _samplePool.Count - 1; i >= 0; --i)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (_samplePool[i], _samplePool[j]) = (_samplePool[j], _samplePool[i]);
            }

            return _samplePool.GetRange(0, samplingCount);
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


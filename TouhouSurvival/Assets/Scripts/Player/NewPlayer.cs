using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Unchord
{
    public class NewPlayer : Pawn
    {
        private Vector2 _movementVector;
        public PlayerAttributeSet AttributeSet { get; private set; }
        public NewPlayerAttributeSet NewAttributeSet { get; private set; }
        
        private const int INITIAL_SAMPLE_POOL_CAPACITY = 32;

        public List<string> weaponSet;
        public List<string> passiveSet;

        public int maxWeaponCount = 6;
        public int maxPassiveCount = 6;

        private List<AbilityComponent> _samplePool;
        private Dictionary<Transform, AbilitySamplingOptions> _samplingOptions;

        protected override void Awake()
        {
            base.Awake();
            
            //AttributeSet = gameObject.GetComponent<PlayerAttributeSet>();
            NewAttributeSet = gameObject.GetComponent<NewPlayerAttributeSet>();
            
            _samplePool = new List<AbilityComponent>(INITIAL_SAMPLE_POOL_CAPACITY);
            _samplingOptions = new Dictionary<Transform, AbilitySamplingOptions>(2);
            _samplingOptions.Add(transform.Find($"Abilities/Weapons"), new AbilitySamplingOptions(maxWeaponCount));
            _samplingOptions.Add(transform.Find($"Abilities/Passives"), new AbilitySamplingOptions(maxPassiveCount));
         
            CreateAbilities();
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }

        private void FixedUpdate()
        {
            // Vector2 nextPosition = _movementVector * (AttributeSet.GetAttribute("Speed") * Time.fixedDeltaTime);
            Vector2 next = _movementVector * (NewAttributeSet.GetAttributeValue(PlayerAttributeType.Speed.ToString()) * Time.fixedDeltaTime);
            Rigidbody.MovePosition(Rigidbody.position + next);
        }

        private void LateUpdate()
        {
            Animator.SetFloat("Speed", _movementVector.magnitude);
            if (_movementVector.x != 0)
            {
                Renderer.flipX = _movementVector.x > 0;  // TODO: 임시로 좌우 반전
            };
        }
        
        private void OnMove(InputValue value)
        {
            // Input Setting에서 이미 값을 Normalized된 상태로 받도록 세팅됨
            _movementVector = value.Get<Vector2>();
        }

        public override float TakeDamage(float damageAmount, Pawn eventInstigator, GameObject damageCauser)
        {
            if (!AttributeSet)
            {
                Debug.Assert(false, "Player has no attribute set");
                return 0f;
            }
            
            // float currentHealth = AttributeSet.GetAttribute("Health");
            // AttributeSet.ModifyAttribute("Health", -damageAmount);
            // float newHealth = AttributeSet.GetAttribute("Health");

            float currentHp = NewAttributeSet.GetAttributeValue(PlayerAttributeType.Health.ToString());
            NewAttributeSet.ModifyAttributeValue(PlayerAttributeType.Health.ToString(), -damageAmount);
            float newHp = NewAttributeSet.GetAttributeValue(PlayerAttributeType.Health.ToString());
            
            Debug.Log($"플레이어가 {damageAmount} 피해를 입었습니다. 체력: {currentHp} -> {newHp}");
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

            if (abilityComponent == null)
                return;
            else
                abilityComponent = GameObject.Instantiate<AbilityComponent>(abilityComponent);

            Debug.Assert(initialLevel >= 0);

            Transform abilityContainer = transform.Find($"Abilities/{strAbilityType}");

            abilityComponent.transform.SetParent(abilityContainer, true);
            abilityComponent.transform.localPosition = Vector3.zero;
            abilityComponent.Subscribe(/* this */ null); // TODO: After integrating this function on Player class, remove null and remove comment in as parameter.
            _samplePool.Add(abilityComponent);
            _samplingOptions[abilityContainer].samplePool.Add(abilityComponent);
            abilityComponent.gameObject.SetActive(initialActive);
            abilityComponent.Level = initialLevel;
        }

        public List<AbilityComponent> SampleAbility(int samplingCount)
        {
            UnityEngine.Debug.Assert(samplingCount > 0);

            List<AbilityComponent> sampleAbilities = new List<AbilityComponent>(samplingCount);

            for (int i = _samplePool.Count; i > 0; --i)
            {
                int j = UnityEngine.Random.Range(0, i);

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

            int maxLevel = abilityComponent.maxLevel;

            AbilitySamplingOptions options = _samplingOptions[abilityComponent.transform];

            if (prevLevel < nextLevel)
            {
                if (prevLevel == 0)
                {
                    options.enabledCount++;
                }
                if (prevLevel < maxLevel && maxLevel <= nextLevel &&
                    ++options.maxLevelReachedCount == options.maxAbilityCount)
                {
                    options.HideAllFrom(_samplePool);
                }
            }
            else
            {
                if (nextLevel == 0)
                {
                    options.enabledCount--;
                }
                if (nextLevel < maxLevel && maxLevel <= prevLevel &&
                    options.maxLevelReachedCount-- == options.maxAbilityCount)
                {
                    options.RevokeAllAt(_samplePool);
                }
            }
        }
        #endregion
    }
}


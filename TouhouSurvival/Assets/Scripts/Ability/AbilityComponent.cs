using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    public abstract class AbilityComponent : MonoBehaviour
    {
        public const string FLAG_SHOULD_DESTROY = "ShouldDestroy";

        // TODO: Ability Pool Management region should be integrated on Player class.
        #region Ability Pool Management
        private const int INITIAL_SAMPLE_POOL_CAPACITY = 32;

        public List<string> weaponSet;
        public List<string> passiveSet;

        public int maxWeaponCount = 6;
        public int maxPassiveCount = 6;

        private List<AbilityComponent> _samplePool;
        private Dictionary<Transform, AbilitySamplingOptions> _samplingOptions;

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

        /* TODO: Write below codes on Player.Awake() function.
         * 
         * _samplePool = new List<AbilityComponent>(INITIAL_SAMPLE_POOL_CAPACITY);
         * _enableCountTable = new Dictionary<Transform, AbilitySamplingOptions>(2);
         * _enableCountTable.Add(transform.Find($"Abilities/Weapons"), new AbilitySamplingOptions(maxWeaponCount));
         * _enableCountTable.Add(transform.Find($"Abilities/Passives"), new AbilitySamplingOptions(maxPassiveCount));
         * 
         * CreateAbilities();
         */

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

                AbilityComponent temp = _samplePool[i];
                _samplePool[i] = _samplePool[j];
                _samplePool[j] = temp;

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

        public int Level
        {
            get => _level;
            set => SetLevel(value);
        }

        public float NormalizedLevel => (float)_level / maxLevel;

        public int maxLevel = 1;

        protected Player _player { get; private set; }
        private int _level;
        
        protected virtual void Awake()
        {

        }

        protected virtual void Update()
        {

        }

        public void Subscribe(Player player)
        {
            _player = player;
        }

        public int SortSiblingIndex()
        {
            Transform parent = transform.parent;
            int i = transform.GetSiblingIndex();

            while (i > 0)
            {
                --i;
                AbilityComponent temp = parent.GetChild(i).GetComponent<AbilityComponent>();

                if (temp.gameObject.activeSelf == true)
                {
                    i++;
                    break;
                }
            }

            transform.SetSiblingIndex(i);
            return i;
        }

        private void SetLevel(int level)
        {
            Debug.Assert(level >= 0);

            int prevLevel = _level;
            int nextLevel = level;

            for (int i = _level + 1; i <= level; ++i)
            {
                // TODO: Write code for updating player's stat. (level increasing)
            }

            for (int i = _level; i > level; --i)
            {
                // TODO: Write code for updating player's stat. (level decreasing)
            }

            _level = level;

            if (prevLevel != nextLevel)
            {
                // TODO: Enable this comment after integrating region Ability Pool Management from this class to Player class.
                // _player.OnChangeAbilityLevel(this, prevLevel, nextLevel);
            }
        }
    }
}
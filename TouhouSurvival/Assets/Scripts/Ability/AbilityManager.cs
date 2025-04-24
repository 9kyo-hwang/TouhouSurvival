using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    public class AbilityManager : MonoBehaviour
    {
        public const int MAX_WEAPON_COUNT = 6;
        public const int MAX_PASSIVE_COUNT = 6;
        public const int MAX_SPELL_COUNT = 1;

        public List<string> weaponSet;
        public List<string> passiveSet;
        public List<string> spellSet;

        private List<AbilitySet> _abilitySets;
        private List<AbilitySet> _abilitySamples;
        private List<AbilityComponent> _sampledPool;

        private void Awake()
        {
            _abilitySets = new List<AbilitySet>(3);
            _abilitySamples = new List<AbilitySet>(2);
            _sampledPool = new List<AbilityComponent>(4);

            Player player = GetComponent<Player>();
            Transform containerWeapons = player.transform.Find("Abilities/Weapons");
            Transform containerPassives = player.transform.Find("Abilities/Passives");
            Transform containerSpells = player.transform.Find("Abilities/Spells");

            _abilitySets.Add(new AbilitySet(player, containerWeapons, "Prefabs/Abilities/Weapons", weaponSet, MAX_WEAPON_COUNT));
            _abilitySets.Add(new AbilitySet(player, containerPassives, "Prefabs/Abilities/Passives", passiveSet, MAX_PASSIVE_COUNT));
            _abilitySets.Add(new AbilitySet(player, containerSpells, "Prefabs/Abilities/Spells", spellSet, MAX_SPELL_COUNT));

            _abilitySamples.Add(_abilitySets[0]);
            _abilitySamples.Add(_abilitySets[1]);
        }

        private void Start()
        {
            _abilitySets[0][0].Attributes.Level = 1;
            _abilitySets[2][0].Attributes.Level = 1;

            this.SortSelf();
        }

        private void Update()
        {

        }

        public List<AbilityComponent> SampleAbilities(int samplingCount)
        {
            UnityEngine.Debug.Assert(samplingCount > 0);

            int validSampleCount = 0;

            _sampledPool.Clear();

            for (int i = 0; i < _abilitySamples.Count; ++i)
            {
                AbilitySet abilitySet = _abilitySamples[i];

                int validAbilityCount = abilitySet.Count;

                if (abilitySet.ValidAbilityCount >= abilitySet.MaxValidAbilityCount)
                    validAbilityCount = abilitySet.ValidAbilityCount;

                for (int j = 0; j < validAbilityCount; ++j)
                {
                    AbilityComponent ability = abilitySet[j];

                    if (ability.Attributes.Level >= ability.Attributes.MaxLevel)
                        continue;

                    int k = UnityEngine.Random.Range(0, ++validSampleCount);

                    if (k >= samplingCount)
                        continue;
                    else if (_sampledPool.Count < samplingCount)
                        _sampledPool.Add(ability);
                    else
                        _sampledPool[k] = ability;
                }
            }

            return _sampledPool;
        }

        public void SortSelf()
        {
            for (int i = 0; i < _abilitySamples.Count; ++i)
            {
                _abilitySamples[i].SortSelf();
            }
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    public class AbilityManager : MonoBehaviour
    {
        public const int MAX_WEAPON_COUNT = 6;
        public const int MAX_PASSIVE_COUNT = 6;
        public const int MAX_SPELL_COUNT = 1;
        public const int MAX_SPECIAL_ABILITY_COUNT = 3;
        public const int MAX_SPECIAL_ABILITY_TREE_COUNT = 2;

        public AbilityComponent MainWeapon => _abilitySets[0][0];
        public AbilityComponent MainSpell => _abilitySets[2][0];

        public List<string> weaponSet;
        public List<string> passiveSet;
        public List<string> spellSet;
        public List<string> specialAbilitySet0;
        public List<string> specialAbilitySet1;

        private List<AbilitySet> _abilitySets;
        private List<AbilitySet> _abilitySamples;
        private List<AbilityComponent> _sampledPool;

        private void Awake()
        {
            _abilitySets = new List<AbilitySet>(3);
            _abilitySamples = new List<AbilitySet>(2);
            _sampledPool = new List<AbilityComponent>(4);
        }

        private void Start()
        {
            Player player = GetComponent<Player>();

            _abilitySets.Add(new AbilitySet(player.WeaponTransform, "Prefabs/Abilities/Weapons", weaponSet, MAX_WEAPON_COUNT));
            _abilitySets.Add(new AbilitySet(player.PassiveTransform, "Prefabs/Abilities/Passives", passiveSet, MAX_PASSIVE_COUNT));
            _abilitySets.Add(new AbilitySet(player.SpellTransform, "Prefabs/Abilities/Spells", spellSet, MAX_SPELL_COUNT));
            _abilitySets.Add(new AbilitySet(player.SpecialTransform0, "Prefabs/Abilities/Specials", specialAbilitySet0, MAX_SPECIAL_ABILITY_COUNT));
            _abilitySets.Add(new AbilitySet(player.SpecialTransform1, "Prefabs/Abilities/Specials", specialAbilitySet1, MAX_SPECIAL_ABILITY_COUNT));

            _abilitySamples.Add(_abilitySets[0]);
            _abilitySamples.Add(_abilitySets[1]);

            _abilitySets[0][0].LevelUp();
            _abilitySets[2][0].LevelUp();

            this.SortSelf();

            UIManager.Instance.GameCanvas.AddWeaponIcon(_abilitySets[0][0].DisplayIcon);
        }

        private void Update()
        {

        }

        public List<AbilityComponent> SampleAbilities(int samplingCount)
        {
            UnityEngine.Debug.Assert(samplingCount > 0);

            int validSampleCount = 0;

            _sampledPool.Clear();

            foreach (AbilitySet abilitySet in _abilitySamples)
            {
                int validAbilityCount = abilitySet.Count;

                if (abilitySet.ValidAbilityCount >= abilitySet.MaxValidAbilityCount)
                    validAbilityCount = abilitySet.ValidAbilityCount;

                for (int j = 0; j < validAbilityCount; ++j)
                {
                    AbilityComponent ability = abilitySet[j];

                    if (ability.CurrentLevel >= ability.MaxLevel)
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
            foreach (AbilitySet abilitySet in _abilitySamples)
            {
                abilitySet.SortSelf();
            }
        }

        public void UpdateSpecialAbilitySlot()
        {
            SpecialAbilityCanvas canvas = UIManager.Instance.SpecialAbilityCanvas;

            for (int i = 0; i < MAX_SPECIAL_ABILITY_TREE_COUNT; ++i)
            {
                int flag = 1;
                AbilitySet tree = _abilitySets[i + 3];

                for (int j = 0; j < MAX_SPECIAL_ABILITY_COUNT; ++j)
                {
                    int level = j + 1;

                    // NOTE: 플래그의 의미, 자세한 사항은 SelectionButtonSpecial.ButtonState 참조.
                    // 0b11 = Selected, 0b10 = Selectable, 0b00 = Lock
                    flag = ((flag << 1) | (tree[j].CurrentLevel > 0 ? 1 : 0)) & 3;

                    canvas.Selections[i, j].SetDescription(tree[j].DisplayDescription);
                    canvas.Selections[i, j].SetIcon(tree[j].DisplayIcon);
                    canvas.Selections[i, j].SetState((SelectionButtonSpecial.ButtonState)flag);
                    canvas.Selections[i, j].SetTooltipPivot(new Vector2(0.5f, 0.0f));
                }
            }
        }

        public void AddSpecialAbilityLevel(int treeIndex)
        {
            int idxTree = treeIndex / MAX_SPECIAL_ABILITY_COUNT;
            int idxHeight = treeIndex % MAX_SPECIAL_ABILITY_COUNT;

            AbilitySet tree = _abilitySets[idxTree + 3];

            tree[idxHeight].LevelUp();
        }
    }
}
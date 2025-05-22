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

            _abilitySets.Add(new AbilitySet(player, player.WeaponTransform, "Prefabs/Abilities/Weapons", weaponSet, MAX_WEAPON_COUNT));
            _abilitySets.Add(new AbilitySet(player, player.PassiveTransform, "Prefabs/Abilities/Passives", passiveSet, MAX_PASSIVE_COUNT));
            _abilitySets.Add(new AbilitySet(player, player.SpellTransform, "Prefabs/Abilities/Spells", spellSet, MAX_SPELL_COUNT));
            _abilitySets.Add(new AbilitySet(player, player.SpecialTransform0, "Prefabs/Abilities/Specials", specialAbilitySet0, MAX_SPECIAL_ABILITY_COUNT));
            _abilitySets.Add(new AbilitySet(player, player.SpecialTransform1, "Prefabs/Abilities/Specials", specialAbilitySet1, MAX_SPECIAL_ABILITY_COUNT));

            _abilitySamples.Add(_abilitySets[0]);
            _abilitySamples.Add(_abilitySets[1]);

            _abilitySets[0][0].Enable();    // 현재는 Ability의 레벨을 1로 세팅하는 것밖에 없음
            _abilitySets[2][0].Enable();

            UpdateAbilitySlot();
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

        private void SortSelf()
        {
            foreach (AbilitySet abilitySet in _abilitySamples)
            {
                abilitySet.SortSelf();
            }
        }

        public void UpdateAbilitySlot()
        {
            this.SortSelf();

            GameCanvas canvas = UIManager.Instance.GameCanvas;

            AbilitySet wSet = _abilitySets[0];
            AbilitySet pSet = _abilitySets[2];

            for (int i = 0; i < wSet.ValidAbilityCount; ++i)
            {
                canvas.EnableWeaponSlot(i);
                canvas.SetWeaponIcon(i, wSet[i].DisplayIcon);
                canvas.SetWeaponLevel(i, wSet[i].CurrentLevel);
            }

            // TODO: 패시브 구현 이후 아래 코드를 주석 해제합니다.
            //for (int i = 0; i < pSet.ValidAbilityCount; ++i)
            //{
            //    canvas.EnablePassiveSlot(i);
            //    canvas.SetPassiveIcon(i, pSet[i].DisplayIcon);
            //    canvas.SetPassiveLevel(i, pSet[i].CurrentLevel);
            //}
        }

        public void UpdateSpecialAbilitySlot()
        {
            SpecialAbilityCanvas canvas = UIManager.Instance.SpecialAbilityCanvas;

            AbilitySet tree0 = _abilitySets[3];
            AbilitySet tree1 = _abilitySets[4];

            int flag0 = 1;
            int flag1 = 1;

            for (int i = 0; i < MAX_SPECIAL_ABILITY_COUNT; ++i)
            {
                int level = i + 1;

                flag0 = ((flag0 << 1) | (tree0[i].CurrentLevel > 0 ? 1 : 0)) & 3;
                flag1 = ((flag1 << 1) | (tree1[i].CurrentLevel > 0 ? 1 : 0)) & 3;

                canvas.SetDescription(0, level, tree0[i].DisplayDescription);
                canvas.InitButton(0, level, (SpecialAbilityCanvas.SelectionState)flag0);

                canvas.SetDescription(1, level, tree1[i].DisplayDescription);
                canvas.InitButton(1, level, (SpecialAbilityCanvas.SelectionState)flag1);
            }
        }

        public void AddSpecialAbilityLevel(int treeIndex)
        {
            AbilitySet tree = _abilitySets[treeIndex + 3];

            int flag = 1;

            for (int i = 0; i < MAX_SPECIAL_ABILITY_COUNT; ++i)
            {
                flag = ((flag << 1) | (tree[i].CurrentLevel > 0 ? 1 : 0)) & 3;

                if (flag == (int)SpecialAbilityCanvas.SelectionState.Selectable)
                {
                    tree[i].LevelUp();
                    break;
                }
            }
        }
    }
}
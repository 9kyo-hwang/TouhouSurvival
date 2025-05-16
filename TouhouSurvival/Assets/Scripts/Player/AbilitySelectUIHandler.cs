using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    public class AbilitySelectUIHandler
    {
        public IEnumerator WaitForSelection(AbilityManager abilityManager, SpecialAbilityComponent specialAbility, int currentLevel)
        {
            switch (currentLevel)
            {
                case 5:
                case 10:
                case 15:
                case 20:
                    yield return WaitForSpecialAbility(specialAbility, currentLevel);
                    break;
            }

            yield return WaitForGeneralAbility(abilityManager);
        }

        private IEnumerator WaitForSpecialAbility(SpecialAbilityComponent specialAbility, int currentLevel)
        {
            SpecialAbilityCanvas canvas = null;

            canvas.Clear();

            for (int i = 0; i < SpecialAbilityComponent.c_TREE_COUNT; ++i)
            {
                for (int j = 1; j <= specialAbility.GetMaxLevel(i); ++j)
                {
                    canvas.SetDescription(i, j, specialAbility.GetDescription(i, j));
                    canvas.SetButtonMode(i, j, currentLevel);
                }
            }

            canvas.Show();
            yield return new WaitWhile(() => canvas.SelectedIndex < 0);
            canvas.Hide();

            int treeIndex = canvas.SelectedIndex;
            specialAbility.AddLevel(treeIndex);
        }

        private IEnumerator WaitForGeneralAbility(AbilityManager abilityManager)
        {
            LevelUpCanvas canvas = UIManager.Instance.LevelUpCanvas;
            List<AbilityComponent> sampledAbilities = abilityManager.SampleAbilities(3);

            canvas.Clear();

            foreach (AbilityComponent ability in sampledAbilities)
            {
                canvas.Add(ability);
            }

            canvas.Show();
            yield return new WaitWhile(() => canvas.SelectedIndex < 0);
            canvas.Hide();

            if (sampledAbilities.Count == 0)
            {
                yield break;
            }

            int selectedIndex = canvas.SelectedIndex;
            AbilityComponent selectedAbility = sampledAbilities[selectedIndex];

            selectedAbility.LevelUp();
            selectedAbility.gameObject.SetActive(true);
            abilityManager.UpdateAbilitySlot();
        }
    }
}
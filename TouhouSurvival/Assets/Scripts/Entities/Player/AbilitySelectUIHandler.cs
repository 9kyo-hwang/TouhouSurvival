using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    public class AbilitySelectUIHandler
    {
        public IEnumerator WaitForSelection(AbilityManager abilityManager, int currentLevel)
        {
            switch (currentLevel)
            {
                case 5:
                case 10:
                case 15:
                case 20:
                    yield return WaitForSpecialAbility(abilityManager, currentLevel);
                    break;
            }

            yield return WaitForGeneralAbility(abilityManager);
        }

        private IEnumerator WaitForSpecialAbility(AbilityManager abilityManager, int currentLevel)
        {
            SpecialAbilityCanvas canvas = UIManager.Instance.SpecialAbilityCanvas;

            canvas.Clear();
            abilityManager.UpdateSpecialAbilitySlot();

            canvas.Show();
            yield return new WaitWhile(() => canvas.SelectedIndex < 0);
            canvas.Hide();

            int treeIndex = canvas.SelectedIndex;
            abilityManager.AddSpecialAbilityLevel(treeIndex);
        }

        private IEnumerator WaitForGeneralAbility(AbilityManager abilityManager)
        {
            LevelUpCanvas lCanvas = UIManager.Instance.LevelUpCanvas;
            List<AbilityComponent> sampledAbilities = abilityManager.SampleAbilities(3);

            lCanvas.Clear();

            foreach (AbilityComponent ability in sampledAbilities)
            {
                lCanvas.Add(ability);
            }

            lCanvas.Show();
            yield return new WaitWhile(() => lCanvas.SelectedIndex < 0);
            lCanvas.Hide();

            if (sampledAbilities.Count == 0)
            {
                yield break;
            }

            int selectedIndex = lCanvas.SelectedIndex;
            AbilityComponent selectedAbility = sampledAbilities[selectedIndex];

            selectedAbility.LevelUp();
            selectedAbility.gameObject.SetActive(true);

            abilityManager.SortSelf();

            GameCanvas gCanvas = UIManager.Instance.GameCanvas;

            if (selectedAbility is WeaponComponent)
                gCanvas.AddWeaponIcon(selectedAbility.DisplayIcon);
            else if (selectedAbility is PassiveComponent)
                gCanvas.AddPassiveIcon(selectedAbility.DisplayIcon);
        }
    }
}
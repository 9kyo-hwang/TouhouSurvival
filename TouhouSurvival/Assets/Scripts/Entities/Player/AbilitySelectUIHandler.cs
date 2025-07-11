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

            int aCount = sampledAbilities.Count;

            if (aCount == 0)
            {
                yield return WaitForSelectionEmpty(lCanvas);
            }
            else
            {
                yield return WaitForSelection(lCanvas, sampledAbilities, abilityManager);
            }

        }

        private IEnumerator WaitForSelectionEmpty(LevelUpCanvas canvas)
        {
            canvas.AddNoEntry();

            canvas.Show();
            yield return new WaitWhile(() => canvas.SelectedIndex < 0);
            canvas.Hide();
        }

        private IEnumerator WaitForSelection(LevelUpCanvas canvas, List<AbilityComponent> abilities, AbilityManager abilityManager)
        {
            for (int i = 0; i < abilities.Count; ++i)
            {
                canvas.AddAbility(abilities[i]);
            }

            canvas.Show();
            yield return new WaitWhile(() => canvas.SelectedIndex < 0);
            canvas.Hide();

            int idxSelected = canvas.SelectedIndex;
            AbilityComponent selectedAbility = abilities[idxSelected];

            selectedAbility.LevelUp();
            selectedAbility.gameObject.SetActive(true);

            abilityManager.SortSelf();

            GameCanvas gCanvas = UIManager.Instance.GameCanvas;

            if (selectedAbility.CurrentLevel != 1)
                yield break;
            else if (selectedAbility is WeaponComponent)
                gCanvas.AddWeaponIcon(selectedAbility.DisplayIcon);
            else if (selectedAbility is PassiveComponent)
                gCanvas.AddPassiveIcon(selectedAbility.DisplayIcon);
        }
    }
}
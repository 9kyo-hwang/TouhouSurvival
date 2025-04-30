using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    public class AbilitySelectUIHandler
    {
        public IEnumerator WaitForSelection(List<AbilityComponent> sampledAbilities)
        {
            LevelUpCanvas canvas = UIManager.Instance.LevelUpCanvas;
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

            selectedAbility.Attributes.Level += 1;
            selectedAbility.SortSiblingIndex();
            selectedAbility.gameObject.SetActive(true);

            int siblingIndex = selectedAbility.transform.GetSiblingIndex();

            GameCanvas gameCanvas = UIManager.Instance.GameCanvas;
            gameCanvas.EnableWeaponSlot(siblingIndex);
            gameCanvas.SetWeaponIcon(siblingIndex, selectedAbility.DisplayIcon);
            gameCanvas.SetWeaponLevel(siblingIndex, selectedAbility.Attributes.Level);
        }
    }
}
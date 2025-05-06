using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    public class AbilitySelectUIHandler
    {
        public IEnumerator WaitForSelection(AbilityManager abilityManager)
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
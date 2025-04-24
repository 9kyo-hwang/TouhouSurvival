using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    public class AbilitySet : List<AbilityComponent>
    {
        // Ability Set의 원소 중 Level이 1 이상인 Ability의 수.
        public int ValidAbilityCount { get; private set; }
        public int MaxValidAbilityCount { get; private set; }

        public AbilitySet(Player player, Transform container, string prefabDirectory, List<string> abilitySet, int maxValidAbilityCount)
        : base(16)
        {
            for (int i = 0; i < abilitySet.Count; ++i)
            {
                string path = prefabDirectory + $"/{abilitySet[i]}/{abilitySet[i]}";

                AbilityComponent ability = Resources.Load<AbilityComponent>(path);

                UnityEngine.Debug.Assert(ability != null);

                ability = GameObject.Instantiate(ability);
                ability.gameObject.SetActive(false);

                ability.transform.SetParent(container, true);
                ability.transform.localPosition = Vector3.zero;
                ability.Subscribe(player);

                base.Add(ability);
            }

            this.MaxValidAbilityCount = maxValidAbilityCount;
        }

        public void SortSelf()
        {
            int i = 0;
            int j = 0;

            while (j < base.Count)
            {
                if (base[j].Attributes.Level == 0)
                    break;
                else
                    ++j;
            }

            for (i = j; i < base.Count; ++i)
            {
                if (base[i].Attributes.Level == 0)
                    continue;

                AbilityComponent temp = base[i];
                base[i] = base[j];
                base[j] = temp;

                ++j;
            }

            this.ValidAbilityCount = j;
        }
    }
}
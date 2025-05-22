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
            foreach (string abilityName in abilitySet)
            {
                string[] pathTokens = abilityName.Split("/");
                string finalAbilityName = pathTokens[pathTokens.Length - 1];

                string path = prefabDirectory + "/" + abilityName + $"/{finalAbilityName}";

                AbilityComponent ability = Resources.Load<AbilityComponent>(path);

                UnityEngine.Debug.Assert(ability != null);

                ability = Object.Instantiate(ability, container, true);
                ability.gameObject.SetActive(false);

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
                if (base[j].CurrentLevel == 0)
                    break;
                else
                    ++j;
            }

            for (i = j; i < base.Count; ++i)
            {
                if (base[i].CurrentLevel == 0)
                    continue;

                (base[i], base[j]) = (base[j], base[i]);

                ++j;
            }

            this.ValidAbilityCount = j;
        }
    }
}
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Unchord
{
    public class TestPlayer : MonoBehaviour
    {
        public AttributeBaseSet baseSet;
        public AttributeModifierSet modifierSet;

        public string path;

        public bool check1;
        public bool check2;

        private void Awake()
        {
            
        }

        private void Update()
        {
            if (check1)
            {
                check1 = false;

                FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                MultiCSVReader reader = new MultiCSVReader(stream);

                List<SerializedGameplayAttributeBase> serializedBase;
                List<SerializedGameplayAttributeModifier> serializedModifier;

                reader.TryParseTable<SerializedGameplayAttributeBase>(out serializedBase, "Entity");
                reader.TryParseTable<SerializedGameplayAttributeModifier>(out serializedModifier, "Entity");

                baseSet = SerializedGameplayAttributeBase.Convert(serializedBase);
                modifierSet = SerializedGameplayAttributeModifier.Convert(serializedModifier);
            }

            if (check2)
            {
                check2 = false;

                StringBuilder builder = new StringBuilder();

                builder.Clear();
                foreach (string attributeName in baseSet.Keys)
                {
                    builder.Append($"{attributeName} == {baseSet[attributeName]}\n");
                }
                Debug.Log(builder.ToString());

                builder.Clear();
                foreach (int targetLevel in modifierSet.Keys)
                {
                    builder.Append($"{targetLevel} ==\n");

                    GameplayAttributeModifier modifier = modifierSet[targetLevel];

                    while (modifier != null)
                    {
                        builder.Append($"  - {modifier.key}, {modifier.value}, {modifier.opcode.ToString()}\n");
                        modifier = modifier.next;
                    }
                }
                Debug.Log(builder.ToString());
            }
        }
    }
}
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Unchord
{
    public class AttributeBaseSet : Dictionary<string, GameplayAttribute>
    {
        // TODO: 기존에는 객체 생성을 위해 아래 함수를 호출했으나 이제 MultiCSVReader 매개변수를 갖는 생성자를 호출하도록 코드 구조를 변경해야 합니다. 이후 이 함수는 삭제합니다.
        public static AttributeBaseSet LoadFromFile(string csvFilePath)
        {
            AttributeBaseSet set = null;
            //AttributeBaseSet set = new AttributeBaseSet();
            Regex regex = new Regex(@"^[A-Za-z0-9_]+$");

            using (FileStream fs = new FileStream(csvFilePath, FileMode.Open, FileAccess.Read))
            using (StreamReader rd = new StreamReader(fs))
            {
                rd.ReadLine(); // Ignore header line.

                while (!rd.EndOfStream)
                {
                    string[] tokens = rd.ReadLine().Split(",");

                    string attributeType = tokens[0];
                    float baseValue;
                    float minValue;
                    float maxValue;

                    if (!regex.IsMatch(attributeType))
                        continue;

                    if (!float.TryParse(tokens[1], out baseValue))
                        baseValue = 0.0f;

                    if (!float.TryParse(tokens[2], out minValue))
                        minValue = float.MinValue;

                    if (!float.TryParse(tokens[3], out maxValue))
                        maxValue = float.MaxValue;

                    GameplayAttribute attribute = new GameplayAttribute(baseValue, minValue, maxValue);
                    set.Add(attributeType, attribute);
                }
            }

            return set;
        }

        private AttributeBaseSet()
        : base(capacity: 32)
        {

        }

        public AttributeBaseSet(MultiCSVReader reader, string aliasOrNull = null)
        : this()
        {
            List<SerializedGameplayAttributeBase> attrBase;

            if (!reader.TryParseTable<SerializedGameplayAttributeBase>(out attrBase, aliasOrNull))
            {
                UnityEngine.Debug.Assert(false, "Parsing SerializedGameplayAttributeBase type failed.");
                return;
            }

            for (int i = 0; i < attrBase.Count; ++i)
            {
                base.Add(attrBase[i].attributeName, new GameplayAttribute(attrBase[i].baseValue));
            }
        }

        public void ApplyModifiers(GameplayAttributeModifier modifier)
        {
            while (modifier != null)
            {
                this[modifier.key].AddModifier(modifier);
                modifier = modifier.next;
            }
        }
    }
}
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Unchord
{
    public class AttributeBaseSet : Dictionary<string, GameplayAttribute>
    {
        public static AttributeBaseSet LoadFromFile(string csvFilePath)
        {
            AttributeBaseSet set = new AttributeBaseSet();
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

        // only can instantiate class-scope.
        public AttributeBaseSet(int capacity = 1)
        : base(capacity)
        {
            // this block intentionally left blank.
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
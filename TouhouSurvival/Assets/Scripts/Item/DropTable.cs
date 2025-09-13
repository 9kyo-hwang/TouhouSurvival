using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    public class DropTable : Dictionary<ItemComponent, float>
    {
        private DropTable()
        : base(capacity: 8)
        {

        }

        public DropTable(MultiCSVReader reader, string aliasOrNull = null)
        : this()
        {
            List<SerializedDropTableRecord> drops;

            if (!reader.TryParseTable<SerializedDropTableRecord>(out drops, aliasOrNull))
            {
                UnityEngine.Debug.Assert(false, "Parsing SerializedDropTableRecord type failed.");
                return;
            }

            for (int i = 0; i < drops.Count; ++i)
            {
                ItemComponent prefab = Resources.Load<ItemComponent>($"Prefabs/Items/{drops[i].itemName}");

                base.Add(prefab, drops[i].dropRatio01);
            }
        }

        public void Generate(Vector2 position, float dx, float dy)
        {
            Transform container = GameManager.Instance.RuntimeContainer;

            foreach (ItemComponent item in base.Keys)
            {
                if (Random.value < base[item])
                {
                    float px = dx * (Random.value - 0.5f);
                    float py = dy * (Random.value - 0.5f);

                    Vector2 p = position + new Vector2(px, py);
                    Quaternion q = Quaternion.identity;

                    ItemComponent newItem = GameObject.Instantiate<ItemComponent>(item, p, q);
                    newItem.transform.SetParent(container, true);
                }
            }
        }
    }
}
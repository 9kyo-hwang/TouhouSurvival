using UnityEngine;

namespace Unchord
{
    public class MagnetObject : ItemComponent
    {
        [Range(2.0f, 100.0f)]
        public float radius = 20.0f;

        protected override void OnEnable()
        {
            base.OnEnable();

            // NOTE: 자석 아이템은 자기 자신을 끌어당겨서는 안 된다.
            base.allowMagnetCollecting = false;
        }

        public override void Use(Player player)
        {
            RaycastHit2D[] hits;

            hits = Physics2D.CircleCastAll(player.transform.position, radius, Vector2.zero);

            for (int i = 0; i < hits.Length; ++i)
            {
                ItemComponent item = hits[i].collider.gameObject.GetComponent<ItemComponent>();

                if (item == null || !item.allowMagnetCollecting)
                    continue;

                item.Use(player);
            }

            Destroy(this.gameObject);
        }
    }
}
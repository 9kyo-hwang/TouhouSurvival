using System;
using UnityEngine;

namespace Unchord
{
    public class ItemScanner : MonoBehaviour
    {
        [SerializeField] private float range;
        private RaycastHit2D[] _hits;
        private Player _player;

        private void Awake()
        {
            _player = gameObject.GetComponent<Player>();
        }

        private void FixedUpdate()
        {
            //float scanRangeMultiplier = _player.GetComponent<PlayerAttributeSet>().GetCurrentValue(PlayerAttributeType.ScanRangeMultiplier);
            _hits = Physics2D.CircleCastAll(_player.transform.position, range, Vector2.zero);
            UseItems();
        }

        private void UseItems()
        {
            foreach (RaycastHit2D hit in _hits)
            {
                ItemComponent item = hit.collider.GetComponent<ItemComponent>();

                if (item != null)
                    item.Use(_player);

                // NOTE: 이 곳에서 Destroy(item)을 수행하지 마세요. item.Use() 함수 안에서 구현하도록 합니다.
            }
        }
    }
}

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
            float scanRangeMultiplier = _player.AttributeSet[PlayerAttributeType.ScanRangeMultiplier].CurrentValue;
            float r = range * (1.0f + scanRangeMultiplier);

            _hits = Physics2D.CircleCastAll(_player.transform.position, r, Vector2.zero);
            
            for (int i = 0; i < _hits.Length; ++i)
            {
                Item scannedItem;

                if (_hits[i].collider.TryGetComponent<Item>(out scannedItem))
                {
                    scannedItem.UseItem(_player);
                }
            }
        }
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace Unchord
{
    public class IcicleFallController : WeaponComponent
    {
        [Header("Prefab Settings")] 
        public GameObject spearPrefab;   // 고드름 창
        public GameObject shardPrefab;    // 고드름 파편
        
        private IcicleSpearContainer _icicleSpearContainer;
        private IcicleShardContainer _icicleShardContainer;
        
        protected override void Awake()
        {
            base.Awake();

            _icicleShardContainer = new IcicleShardContainer(shardPrefab, transform, AttributeBase);
            _icicleSpearContainer = new IcicleSpearContainer(spearPrefab, transform, AttributeBase, _icicleShardContainer);
        }

        public override void UseWeapon()
        {
            // Cooldown마다 호출
            SpawnIcicleSpear();
        }

        private void SpawnIcicleSpear()
        {
            GameObject nearestGameObject = Player.GetNearestEnemyOrNull();
            if (nearestGameObject)
            {
                Vector3 startPosition = Player.transform.position;
                Vector3 endPosition = nearestGameObject.transform.position;
                float launchAngleOffset = AttributeBase[IcicleFallAttributeType.LaunchAngleOffset].CurrentValue;
                float speed = AttributeBase[IcicleFallAttributeType.SpearSpeed].CurrentValue;
                float duration = AttributeBase[IcicleFallAttributeType.SpearDuration].CurrentValue;
                
                GameObject spearGameObject = _icicleSpearContainer.Get();
                spearGameObject.GetComponent<IcicleSpear>()
                    .Launch(startPosition, endPosition, launchAngleOffset, speed, duration);
            }
        }
    }
}

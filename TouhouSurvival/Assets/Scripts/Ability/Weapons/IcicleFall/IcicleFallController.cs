using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace Unchord
{
    public class IcicleFallController : WeaponComponent
    {
        [Header("Prefab Settings")] public GameObject spearPrefab;   // 고드름 창
        [Header("Prefab Settings")] public GameObject shardPrefab;    // 고드름 파편
        
        private IcicleSpearContainer _icicleSpearContainer;
        private IcicleShardContainer _icicleShardContainer;
        
        protected override void Awake()
        {
            base.Awake();

            _icicleShardContainer = new IcicleShardContainer(shardPrefab, transform, Attributes);
            _icicleSpearContainer = new IcicleSpearContainer(spearPrefab, transform, Attributes, _icicleShardContainer);
        }

        protected override void UseWeapon()
        {
            base.UseWeapon();

            // Cooldown마다 호출
            SpawnIcicleSpear();
        }

        private void SpawnIcicleSpear()
        {
            GameObject nearestGameObject = Spawner.GetNearestEnemyOrNull(_player.transform.position);
            if (nearestGameObject)
            {
                Vector3 startPosition = _player.transform.position;
                Vector3 endPosition = nearestGameObject.transform.position;
                float launchAngleOffset = Attributes[IcicleFallAttributeType.LaunchAngleOffset].CurrentValue;
                float speed = Attributes[IcicleFallAttributeType.SpearSpeed].CurrentValue;
                float duration = Attributes[IcicleFallAttributeType.SpearDuration].CurrentValue;
                
                GameObject icicleGameObject = _icicleSpearContainer.Get();
                icicleGameObject.GetComponent<IcicleSpear>()
                    .Launch(startPosition, endPosition, launchAngleOffset, speed, duration);
            }
        }
    }
}

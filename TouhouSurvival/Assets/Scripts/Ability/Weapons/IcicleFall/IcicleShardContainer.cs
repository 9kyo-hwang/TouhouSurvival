using UnityEngine;
using UnityEngine.Pool;

namespace Unchord
{
    public class IcicleShardContainer
    {
        private readonly GameObject _prefab;
        private readonly Transform _transform;
        private readonly AttributeBaseSet _attributeBase;
        private readonly ObjectPool<GameObject> _pool;

        public GameObject Get() => _pool.Get();
        
        public IcicleShardContainer(GameObject prefab, Transform transform, AttributeBaseSet attributeSet)
        {
            _prefab = prefab;
            _transform = transform;
            _attributeBase = attributeSet;
            
            _pool = new ObjectPool<GameObject>(
                CreateFunc, 
                ActionOnGet,
                ActionOnRelease,
                ActionOnDestroy, 
                collectionCheck: true, 
                defaultCapacity: 10, 
                maxSize: 50);
        }
        
        private GameObject CreateFunc()
        {
            // 캐릭터 위치에 생성
            GameObject shardGameObject = Object.Instantiate(_prefab.gameObject, GameManager.Instance.ProjectileContainer);

            // 충돌 이벤트 탐지기 획득
            shardGameObject.transform.Find("Colliders/Circle Collider 2D")
                .GetComponent<CollisionEventEmitter>()
                .onTriggerEnter2D += (sender, args) =>
            {
                GameObject enemyGameObject = args.targetObject;
                if (!enemyGameObject)
                {
                    return;
                }

                Enemy enemy = enemyGameObject.GetComponentInParent<Enemy>();
                if (!enemy)
                {
                    return;
                }
                
                float damage = _attributeBase[IcicleFallAttributeType.ShardDamage].CurrentValue;
                float knockBackStrength = _attributeBase[IcicleFallAttributeType.ShardKnockbackStrength].CurrentValue;
                Debug.Log($"Shard Attack To {enemy.name} about {damage}");
                enemy.TakeDamage(damage, _transform.GetComponentInParent<Player>(), shardGameObject);
                enemy.KnockBack(knockBackStrength);

                GameObject projectileGameObject = args.eventSource;
                LinearProjectile projectile = projectileGameObject.GetComponentInParent<LinearProjectile>(true);
                if (projectile)
                {
                    projectile.FlagTable[AbilityComponent.FLAG_SHOULD_DESTROY] = true;
                }
            };
            
            shardGameObject.GetComponent<FlagComponent>()
                .AddEventTrue(AbilityComponent.FLAG_SHOULD_DESTROY, component =>
                {
                    _pool.Release(component.gameObject);
                });
            
            return shardGameObject;
        }
        
        private void ActionOnGet(GameObject obj)
        {
            obj.GetComponent<FlagComponent>().SetFlagFalseWithoutEvent(AbilityComponent.FLAG_SHOULD_DESTROY);
            obj.SetActive(true);
            
            float size = _attributeBase[IcicleFallAttributeType.ShardSize].CurrentValue;
            obj.transform.localScale = new Vector3(size, size, 1.0f);
        }

        private void ActionOnRelease(GameObject obj)
        {
            obj.SetActive(false);
        }

        private void ActionOnDestroy(GameObject obj)
        {
            // NOTE: Do not action anything
        }
    }
}

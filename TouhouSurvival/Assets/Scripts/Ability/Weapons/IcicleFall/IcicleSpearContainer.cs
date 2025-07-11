using UnityEngine;
using UnityEngine.Pool;

namespace Unchord
{
    public class IcicleSpearContainer
    {
        private readonly GameObject _prefab;
        private readonly Transform _transform;
        private readonly AttributeBaseSet _attributeBase;
        private readonly ObjectPool<GameObject> _pool;
        private readonly IcicleShardContainer _shardContainer;
        
        public IcicleSpearContainer(GameObject prefab, Transform transform, AttributeBaseSet attributeSet, 
            IcicleShardContainer shardContainer)
        {
            _prefab = prefab;
            _transform = transform;
            _attributeBase = attributeSet;
            _shardContainer = shardContainer;
            
            _pool = new ObjectPool<GameObject>(
                CreateFunc,
                ActionOnGet,
                ActionOnRelease,
                ActionOnDestroy,
                collectionCheck: true,
                defaultCapacity: 4,
                maxSize: 10);
        }

        public GameObject Get() => _pool.Get();
        
        private GameObject CreateFunc()
        {
            // 캐릭터 위치에 생성
            GameObject spearGameObject = Object.Instantiate(_prefab.gameObject, GameManager.Instance.ProjectileContainer);

            // 충돌 이벤트 탐지기 획득
            spearGameObject.transform.Find("Colliders/Circle Collider 2D")
                .GetComponent<CollisionEventEmitter>()
                .onTriggerEnter2D += ActionOnEnter;
            
            spearGameObject.GetComponent<FlagComponent>()
                .AddEventTrue(AbilityComponent.FLAG_SHOULD_DESTROY, ActionOnHit);
            
            return spearGameObject;
        }

        private void ActionOnGet(GameObject obj)
        {
            obj.GetComponent<FlagComponent>().SetFlagFalseWithoutEvent(AbilityComponent.FLAG_SHOULD_DESTROY);
            obj.SetActive(true);
            
            float size = _attributeBase[IcicleFallAttributeType.SpearSize].CurrentValue;
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

        private void ActionOnEnter(object sender, CollisionEventArgs args)
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

            float damage = _attributeBase[IcicleFallAttributeType.SpearDamage].CurrentValue;
            float knockBackStrength = _attributeBase[IcicleFallAttributeType.SpearKnockbackStrength].CurrentValue;
            enemy.TakeDamage(damage);
            enemy.KnockBack(knockBackStrength);

            GameObject projectileGameObject = args.eventSource;
            LinearProjectile projectile = projectileGameObject.GetComponentInParent<LinearProjectile>(true);
            if (projectile)
            {
                projectile.FlagTable[AbilityComponent.FLAG_SHOULD_DESTROY] = true;
            }
        }

        private void ActionOnHit(FlagComponent flagTable)
        {
            SpawnIcicleShard(flagTable);
            _pool.Release(flagTable.gameObject);
        }

        private void SpawnIcicleShard(FlagComponent flagTable)
        {
            Vector3 position = flagTable.transform.position;
            float duration = _attributeBase[IcicleFallAttributeType.ShardDuration].CurrentValue;
            float speed = _attributeBase[IcicleFallAttributeType.ShardSpeed].CurrentValue;
            
            int shardCount = (int)_attributeBase[IcicleFallAttributeType.ShardCount].CurrentValue;
            for (int i = 0; i < shardCount; ++i)
            {
                float angle = 360f / shardCount * i;
                Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right;
                
                GameObject shardGameObject = _shardContainer.Get();
                shardGameObject.GetComponent<IcicleShard>().Launch(position, speed, direction, duration);
            }
        }
    }
}

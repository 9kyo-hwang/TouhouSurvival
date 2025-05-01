using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace Unchord
{
    public class SakuyaController : WeaponComponent
    {
        [Header("Prefab Settings")] 
        public GameObject sakuyaPrefab; // 단검 프리팹
        
        #region Sakuya Pool
        private ObjectPool<GameObject> _sakuyaPool;
        
        private void ActionOnDestroy(GameObject obj)
        {
            // NOTE: 아무 작업도 수행하지 않음
        }

        private void ActionOnRelease(GameObject obj)
        {
            obj.SetActive(false);
        }

        private void ActionOnGet(GameObject obj)
        {
            obj.GetComponent<FlagComponent>()
                .SetFlagFalseWithoutEvent(FLAG_SHOULD_DESTROY);
            
            obj.SetActive(true);

            float size = Attributes[SakuyaAttributeType.Size].CurrentValue;
            obj.transform.localScale = new Vector3(size, size, 1.0f);
        }

        private GameObject CreateFunc()
        {
            GameObject sakuyaGameObject = Instantiate(sakuyaPrefab.gameObject, GameManager.Instance.ProjectileContainer, true);

            sakuyaGameObject.transform.Find("Colliders/Circle Collider 2D")
                .GetComponent<CollisionEventEmitter>()
                .onTriggerEnter2D += (sender, args) =>
            {
                GameObject enemyGameObject = args.targetObject;
                if (!enemyGameObject)
                {
                    return;
                }

                Enemy enemy = enemyGameObject.GetComponentInParent<Enemy>(true);
                if (!enemy)
                {
                    return;
                }
                
                float damage = Attributes[SakuyaAttributeType.Damage].CurrentValue;
                float knockBackStrength = Attributes[SakuyaAttributeType.KnockbackStrength].CurrentValue;
                enemy.TakeDamage(damage, GetComponentInParent<Player>(), gameObject);
                enemy.KnockBack(knockBackStrength);

                GameObject projectileGameObject = args.eventSource;
                LinearProjectile projectile = projectileGameObject.GetComponentInParent<LinearProjectile>(true);
                if (projectile)
                {
                    projectile.FlagTable[FLAG_SHOULD_DESTROY] = true;
                }
            };

            sakuyaGameObject.GetComponent<FlagComponent>()
                .AddEventTrue(FLAG_SHOULD_DESTROY, flagComponent =>
                {
                    _sakuyaPool.Release(flagComponent.gameObject);
                });

            return sakuyaGameObject;
        }
        #endregion
        
        // TODO: 추후 피격 이펙트 발동도 필요
        
        protected override void Awake()
        {
            base.Awake();

            _sakuyaPool = new ObjectPool<GameObject>(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy,
                collectionCheck: true, defaultCapacity: 4, maxSize: 10);
        }

        public override void UseWeapon()
        {
            // 한 번 호출 시 throwDelay 간격으로 throwCount 생성
            StartCoroutine(ThrowCoroutine());
        }

        private IEnumerator ThrowCoroutine()
        {
            _isCooltimePaused = true;
            
            GameObject nearestGameObject = _player.GetNearestEnemyOrNull();
            if (nearestGameObject)
            {
                int throwCount = (int)Attributes[SakuyaAttributeType.ThrowCount].CurrentValue;
                Vector3 targetPosition = nearestGameObject.transform.position;
                Vector3 playerPosition = _player.transform.position;
                for (int i = 0; i < throwCount; i++)
                {
                    Throw(targetPosition, playerPosition);
                    float throwDelay = Attributes[SakuyaAttributeType.ThrowDelay].CurrentValue;
                    yield return new WaitForSeconds(throwDelay);
                }
            }
            
            _isCooltimePaused = false;
        }

        private void Throw(Vector3 targetPosition, Vector3 playerPosition)
        {
            float throwAngleOffset = Attributes[SakuyaAttributeType.ThrowAngleOffset].CurrentValue;
            float speed = Attributes[SakuyaAttributeType.ThrowSpeed].CurrentValue;
            float duration = Attributes[SakuyaAttributeType.Duration].CurrentValue;

            GameObject sakuyaObject = _sakuyaPool.Get();
            sakuyaObject.GetComponent<Sakuya>().Initialize(
                playerPosition,
                targetPosition,
                throwAngleOffset,
                speed,
                duration
            );
        }
    }    
}


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
            
            obj.SetActive(false);
            
            // NOTE: 단순히 단검이 이동하면 되기 때문에 Animation X
        }

        private GameObject CreateFunc()
        {
            GameObject sakuyaGameObject = Instantiate(sakuyaPrefab, transform);
            sakuyaGameObject.transform.localPosition = Vector3.zero;

            sakuyaGameObject.transform.Find("Colliders/Damaging Collider")
                .GetComponent<CollisionEventEmitter>()
                .onTriggerEnter2D += (sender, args) =>
            {
                GameObject enemyGameObject = args.targetObject;
                if (!enemyGameObject)
                {
                    return;
                }

                Enemy enemy = enemyGameObject.GetComponent<Enemy>();
                if (!enemy)
                {
                    return;
                }

                // TODO: enemy.TakeDamage(damage);
                float damage = Attributes[SakuyaAttributeType.Damage].CurrentValue;
                enemy.TakeDamage(damage, GetComponentInParent<Player>(), gameObject);
            };

            sakuyaGameObject.GetComponent<FlagComponent>()
                .AddEventTrue(FLAG_SHOULD_DESTROY, flagComponent =>
                {
                    _sakuyaPool.Release(flagComponent.gameObject);
                });

            return sakuyaGameObject;
        }
        #endregion
        
        protected override void Awake()
        {
            base.Awake();

            _sakuyaPool = new ObjectPool<GameObject>(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy,
                collectionCheck: true, defaultCapacity: 4, maxSize: 10);
        }

        protected override void UseWeapon()
        {
            // Cooldown마다 호출
            base.UseWeapon();

            // 한 번 호출 시 throwDelay 간격으로 throwCount 생성
            StartCoroutine(ThrowCoroutine());
        }

        private IEnumerator ThrowCoroutine()
        {
            _isCooltimePaused = true;
            
            int throwCount = (int)Attributes[SakuyaAttributeType.ThrowCount].CurrentValue;
            for (int i = 0; i < throwCount; i++)
            {
                ThrowSakuya();
                float throwDelay = Attributes[SakuyaAttributeType.ThrowDelay].CurrentValue;
                yield return new WaitForSeconds(throwDelay);
            }
            
            _isCooltimePaused = false;
        }

        private void ThrowSakuya()
        {
            
        }

        private GameObject FindNearestGameObject()
        {
            return null;
        }
    }    
}


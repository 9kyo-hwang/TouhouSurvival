using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Unchord
{
    public class ShanghaiGuard : WeaponComponent
    {
        private static int s_shanghaiShowHash = Animator.StringToHash("ShanghaiDollShow");
        
        [Header("Prefab Settings")]
        public GameObject shanghaiPrefab;

        private ObjectPool<DotProjectile> _shanghaiPool;
        private List<DotProjectile> _shanghaiEnabledList;

        private float _targetShanghaiSize = 0.0f;
        private float _currentShanghaiSize = 0.0f;

        private float _targetShanghaiRadius = 0.0f;
        private float _currentShanghaiRadius = 0.0f;

        public float _leftDuration;
        private float _rotationPhaseAngle;

        protected override void Awake()
        {
            base.Awake();

            _shanghaiPool = new ObjectPool<DotProjectile>(
                OnCreateShanghai,
                OnGetShanghai,
                OnReleaseShanghai,
                OnDestroyShanghai,
                true,
                6,
                20
                );

            _shanghaiEnabledList = new List<DotProjectile>(6);
        }

        protected override void Update()
        {
            base.Update();

            if (_leftDuration > 0.0f && (_leftDuration -= Time.deltaTime) <= 0.0f)
            {
                _targetShanghaiSize = -0.1f;
                _targetShanghaiRadius = 0.0f;

                StartHideDolls();
            }

            if (_targetShanghaiSize > 0.0f || _currentShanghaiSize > 0.0f)
            {
                _currentShanghaiSize = Mathf.Lerp(_currentShanghaiSize, _targetShanghaiSize, Time.deltaTime * 1.0f);

                RotateShanghaiDolls();
            }
            else
            {
                for (int i = _shanghaiEnabledList.Count - 1; i >= 0; --i)
                {
                    _shanghaiPool.Release(_shanghaiEnabledList[i]);
                    _shanghaiEnabledList.RemoveAt(i);
                }
            }

            _currentShanghaiRadius = Mathf.Lerp(_currentShanghaiRadius, _targetShanghaiRadius, Time.deltaTime * 1.0f);
        }

        public override void UseWeapon()
        {
            _targetShanghaiSize = Attributes[ShanghaiGuardAttributeType.ShanghaiSize].CurrentValue;
            _targetShanghaiRadius = Attributes[ShanghaiGuardAttributeType.ShanghaiRadius].CurrentValue;
            _leftDuration = Attributes[ShanghaiGuardAttributeType.ShanghaiDuration].CurrentValue;

            for (int i = 0; i < Attributes[ShanghaiGuardAttributeType.ShanghaiCount].CurrentValue; ++i)
            {
                DotProjectile shanghai = _shanghaiPool.Get();
            }
        }

        //protected override void OnChangeAbilityLevel(int prevLevel, int nextLevel)
        //{
        //    if (prevLevel == 0)
        //        return;

        //    base.OnChangeAbilityLevel(prevLevel, nextLevel);

        //    Attributes.ApplyLevelUpData(prevLevel);
        //}

        private void RotateShanghaiDolls()
        {
            _rotationPhaseAngle += Time.deltaTime * Attributes[ShanghaiGuardAttributeType.ShanghaiAngularSpeed].CurrentValue;
            _rotationPhaseAngle %= 360.0f;

            UnityEngine.Debug.Assert(_shanghaiEnabledList.Count > 0);

            float phaseRadianAngle = Mathf.Deg2Rad * _rotationPhaseAngle;
            float pCos = Mathf.Cos(phaseRadianAngle);
            float pSin = Mathf.Sin(phaseRadianAngle);

            float deltaRadianAngle = 2.0f * Mathf.PI / _shanghaiEnabledList.Count;
            float dCos = Mathf.Cos(deltaRadianAngle);
            float dSin = Mathf.Sin(deltaRadianAngle);
            Vector2 axis = new Vector2(pCos, pSin);
            Vector2 axisBuffer = axis;
            Vector2 origin = transform.position;

            for (int i = 0; i < _shanghaiEnabledList.Count; ++i)
            {
                axisBuffer = axis;
                axis.x = axisBuffer.x * dCos - axisBuffer.y * dSin;
                axis.y = axisBuffer.y * dCos + axisBuffer.x * dSin;
                _shanghaiEnabledList[i].OriginPosition = origin + axis * _currentShanghaiRadius;
                _shanghaiEnabledList[i].transform.localScale = new Vector3(_currentShanghaiSize, _currentShanghaiSize, 1.0f);
            }
        }

        private void StartHideDolls()
        {
            for (int i = 0; i < _shanghaiEnabledList.Count; ++i)
            {
                Animator animator = _shanghaiEnabledList[i].GetComponent<Animator>();
                animator.SetBool("IsShow", false);
            }
        }

        private DotProjectile OnCreateShanghai()
        {
            GameObject shanghai = GameObject.Instantiate(shanghaiPrefab, GameManager.Instance.ProjectileContainer, true);

            CollisionEventEmitter emitter = shanghai.transform.Find("Colliders/Circle Collider 2D").GetComponent<CollisionEventEmitter>();
            emitter.onTriggerStay2D += OnShanghaiDollStay;

            return shanghai.GetComponent<DotProjectile>();
        }

        private void OnGetShanghai(DotProjectile shanghai)
        {
            shanghai.gameObject.SetActive(true);
            shanghai.transform.localPosition = Vector3.forward * shanghai.transform.localPosition.z;

            Animator animator = shanghai.GetComponent<Animator>();
            animator.SetBool("IsShow", true);
            animator.Play(s_shanghaiShowHash);

            _shanghaiEnabledList.Add(shanghai);
        }

        private void OnReleaseShanghai(DotProjectile shanghai)
        {
            shanghai.gameObject.SetActive(false);
        }

        private void OnDestroyShanghai(DotProjectile shanghai)
        {
            // NOTE: This block is intentionally no operation.
        }

        private void OnShanghaiDollStay(object collisionEventEmitter, CollisionEventArgs args)
        {
            GameObject enemyObject = args.targetObject;
            Enemy enemy = enemyObject.GetComponentInParent<Enemy>();

            UnityEngine.Debug.Assert(enemy != null);

            if (enemy.Attributes[EnemyAttributeType.Health].CurrentValue > 0.0f)
            {
                float damage = Attributes[ShanghaiGuardAttributeType.ShanghaiDamage].CurrentValue;
                enemy.TakeDamage(damage, null, null);
            }
        }
    }
}
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

        private float _capturedUsingTime;
        private int _capturedShanghaiCount;
        private float _capturedShanghaiSize = 0.0f;
        private float _capturedShanghaiRadius = 0.0f;

        private float _w0;
        private float _w1;
        private float _w2;
        private float _weight; // _weight is in range [0, 1]

        private float _rotationPhaseAngle;

        private DotProjectile GetShanghai(int index)
        {
            while (index >= _shanghaiEnabledList.Count)
                _shanghaiPool.Get();

            return _shanghaiEnabledList[index];
        }

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

            if (!base._isCooltimePaused)
                return;

            float time = GameManager.Instance.AbsolutePlaytime - _capturedUsingTime;
            float nextWeight = Mathf.Clamp01(Mathf.Min(time * _w0, time * _w1 + _w2));

            for (int i = 0; i < _capturedShanghaiCount; ++i)
            {
                DotProjectile shanghai = GetShanghai(i);
                Animator animator = shanghai.GetComponent<Animator>();

                animator.SetBool("IsShow", nextWeight >= _weight);
            }

            _weight = nextWeight;
            RotateShanghaiDolls();

            if (time > 0.0f && _weight == 0.0f)
            {
                base._isCooltimePaused = false;

                for (int i = _capturedShanghaiCount - 1; i >= 0; --i)
                {
                    _shanghaiPool.Release(_shanghaiEnabledList[i]);
                    _shanghaiEnabledList.RemoveAt(i);
                }
            }
        }

        protected override void UseWeapon()
        {
            base.UseWeapon();

            float t0 = 2.0f; // up-rising time
            float t1 = Attributes[ShanghaiGuardAttributeType.ShanghaiDuration].CurrentValue;
            float t2 = 1.0f; // down-rising time

            UnityEngine.Debug.Assert(t0 > 0.0f);
            UnityEngine.Debug.Assert(t1 >= 0.0f);
            UnityEngine.Debug.Assert(t2 > 0.0f);

            _capturedUsingTime = GameManager.Instance.AbsolutePlaytime;
            _capturedShanghaiCount = (int)Attributes[ShanghaiGuardAttributeType.ShanghaiCount].CurrentValue;
            _capturedShanghaiSize = Attributes[ShanghaiGuardAttributeType.ShanghaiSize].CurrentValue;
            _capturedShanghaiRadius = Attributes[ShanghaiGuardAttributeType.ShanghaiRadius].CurrentValue;
            _w0 = 1.0f / t0;
            _w1 = -1.0f / t2;
            _w2 = (t0 + t1 + t2) / t2;

            base._isCooltimePaused = true;
        }

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
                _shanghaiEnabledList[i].OriginPosition = origin + axis * _capturedShanghaiRadius * _weight;
                _shanghaiEnabledList[i].transform.localScale = new Vector3(_capturedShanghaiSize * _weight, _capturedShanghaiSize * _weight, 1.0f);
            }
        }

        private DotProjectile OnCreateShanghai()
        {
            GameObject shanghai = GameObject.Instantiate(shanghaiPrefab, transform, true);

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
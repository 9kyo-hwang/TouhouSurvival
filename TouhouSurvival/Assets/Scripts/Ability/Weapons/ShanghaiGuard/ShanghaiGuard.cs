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

        private ObjectPool<PlayerTrackingProjectile> _shanghaiPool;
        private List<PlayerTrackingProjectile> _enabledDolls;

        private float _targetSize = 0.0f;
        private float _targetRadius = 0.0f;

        private float _wTarget = 0.0f;
        private float _w = 0.0f;
        private float _direction;

        private float _leftDuration;

        private float _rotationPhaseAngle;

        protected override void Awake()
        {
            base.Awake();

            _shanghaiPool = new ObjectPool<PlayerTrackingProjectile>(
                OnCreateShanghai,
                OnGetShanghai,
                OnReleaseShanghai,
                OnDestroyShanghai,
                true,
                6,
                20
                );

            _enabledDolls = new List<PlayerTrackingProjectile>(6);
        }

        protected override void Update()
        {
            base.Update();

            if (!base._isCooltimePaused)
                return;

            UpdateDolls();
            UpdateDuration();
            UpdateWeight();
        }

        public override void UseWeapon()
        {
            _targetSize = AttributeBase[ShanghaiGuardAttributeType.ShanghaiSize].CurrentValue;
            _targetRadius = AttributeBase[ShanghaiGuardAttributeType.ShanghaiRadius].CurrentValue;

            _wTarget = 1.0f;
            
            _leftDuration = base.AttributeBase[ShanghaiGuardAttributeType.ShanghaiDuration].CurrentValue;

            base._isCooltimePaused = true;

            GetDolls();
        }

        private void UpdateDolls()
        {
            _rotationPhaseAngle += Time.deltaTime * AttributeBase[ShanghaiGuardAttributeType.ShanghaiAngularSpeed].CurrentValue;
            _rotationPhaseAngle %= 360.0f;

            UnityEngine.Debug.Assert(_enabledDolls.Count > 0);

            float phaseRadianAngle = Mathf.Deg2Rad * _rotationPhaseAngle;
            float pCos = Mathf.Cos(phaseRadianAngle);
            float pSin = Mathf.Sin(phaseRadianAngle);

            float deltaRadianAngle = 2.0f * Mathf.PI / _enabledDolls.Count;
            float dCos = Mathf.Cos(deltaRadianAngle);
            float dSin = Mathf.Sin(deltaRadianAngle);
            Vector2 axis = new Vector2(pCos, pSin);
            Vector2 axisBuffer = axis;
            Vector2 origin = transform.position;

            float radius = _targetRadius * _w;
            float size = _targetSize * _w;

            for (int i = 0; i < _enabledDolls.Count; ++i)
            {
                axisBuffer = axis;
                axis.x = axisBuffer.x * dCos - axisBuffer.y * dSin;
                axis.y = axisBuffer.y * dCos + axisBuffer.x * dSin;
                _enabledDolls[i].DeltaPosition = axis * radius;
                _enabledDolls[i].transform.localScale = new Vector3(size, size, 1.0f);
            }
        }

        private void UpdateDuration()
        {
            if (_w != 1.0f)
                return;

            _leftDuration = Mathf.Max(0.0f, _leftDuration - Time.deltaTime);

            if (_leftDuration == 0.0f)
                _wTarget = 0.0f;
        }

        private void UpdateWeight()
        {
            if (_w != _wTarget)
            {
                float wSpeed = 0.5f;
                float direction = (_wTarget - _w) / Mathf.Abs(_wTarget - _w);

                if (_direction > 0.0f && direction < 0.0f)
                    HideDolls();

                _direction = direction;
                _w = Mathf.Clamp01(_w + Time.deltaTime * wSpeed * direction);
            }

            if (_w != _wTarget)
                return;

            if (_wTarget == 0.0f)
            {
                base._isCooltimePaused = false;
                ReleaseDolls();
            }
        }

        private void GetDolls()
        {
            int dollCount = (int)base.AttributeBase[ShanghaiGuardAttributeType.ShanghaiCount].CurrentValue;

            for (int i = 0; i < dollCount; ++i)
            {
                _shanghaiPool.Get();
            }
        }

        private void HideDolls()
        {
            for (int i = _enabledDolls.Count - 1; i >= 0; --i)
            {
                PlayerTrackingProjectile shanghai = _enabledDolls[i];
                Animator animator = shanghai.GetComponent<Animator>();
                animator.SetBool("IsShow", true);
                animator.Play(s_shanghaiShowHash);

            }
        }

        private void ReleaseDolls()
        {
            for (int i = _enabledDolls.Count - 1; i >= 0; --i)
            {
                _shanghaiPool.Release(_enabledDolls[i]);
                _enabledDolls.RemoveAt(i);
            }
        }

        private PlayerTrackingProjectile OnCreateShanghai()
        {
            GameObject shanghai = GameObject.Instantiate(shanghaiPrefab, GameManager.Instance.ProjectileContainer, true);

            CollisionEventEmitter emitter = shanghai.transform.Find("Colliders/Circle Collider 2D").GetComponent<CollisionEventEmitter>();
            emitter.onTriggerStay2D += OnShanghaiDollStay;

            return shanghai.GetComponent<PlayerTrackingProjectile>();
        }

        private void OnGetShanghai(PlayerTrackingProjectile shanghai)
        {
            shanghai.gameObject.SetActive(true);

            Animator animator = shanghai.GetComponent<Animator>();
            animator.SetBool("IsShow", true);
            animator.Play(s_shanghaiShowHash);

            _enabledDolls.Add(shanghai);
        }

        private void OnReleaseShanghai(PlayerTrackingProjectile shanghai)
        {
            shanghai.gameObject.SetActive(false);
        }

        private void OnDestroyShanghai(PlayerTrackingProjectile shanghai)
        {
            // NOTE: This block is intentionally no operation.
        }

        private void OnShanghaiDollStay(object collisionEventEmitter, CollisionEventArgs args)
        {
            GameObject enemyObject = args.targetObject;
            Enemy enemy = enemyObject.GetComponentInParent<Enemy>();

            UnityEngine.Debug.Assert(enemy != null);

            if (enemy.AttributeBase[EnemyAttributeType.Health].CurrentValue > 0.0f)
            {
                float damage = AttributeBase[ShanghaiGuardAttributeType.ShanghaiDamage].CurrentValue;
                enemy.TakeDamage(damage);
            }
        }
    }
}
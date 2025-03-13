using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Unchord
{
    public class ShanghaiGuard : WeaponComponent
    {
        private static int s_shanghaiShowHash = Animator.StringToHash("ShanghaiShow");
        private static int s_shanghaiHideHash = Animator.StringToHash("ShanghaiHide");

        public ShanghaiGuardAttributeSet Attributes { get; private set; }

        [Header("Prefab Settings")]
        public GameObject shanghaiPrefab;

        private ObjectPool<GameObject> _shanghaiPool;
        private List<GameObject> _shanghaiEnabledList;

        private float _targetShanghaiSize = 0.0f;
        private float _currentShanghaiSize = 0.0f;

        private float _targetShanghaiRadius = 0.0f;
        private float _currentShanghaiRadius = 0.0f;

        private float _leftDuration;

        private Queue<Collider2D> _targetColliders;

        protected override void Awake()
        {
            base.Awake();

            Attributes = GetComponent<ShanghaiGuardAttributeSet>();

            _shanghaiPool = new ObjectPool<GameObject>(
                OnCreateShanghai,
                OnGetShanghai,
                OnReleaseShanghai,
                OnDestroyShanghai,
                true,
                6,
                20
                );

            _shanghaiEnabledList = new List<GameObject>(6);
            _targetColliders = new Queue<Collider2D>(20);
        }

        protected void FixedUpdate()
        {
            while (_targetColliders.Count > 0)
                OnShanghaiStay(_targetColliders.Dequeue());
        }

        protected override void Update()
        {
            base.Update();

            if (_leftDuration > 0.0f && (_leftDuration -= Time.deltaTime) <= 0.0f)
            {
                _targetShanghaiSize = -0.1f;
                _targetShanghaiRadius = 0.0f;
            }

            if (_targetShanghaiSize > 0.0f || _currentShanghaiSize > 0.0f)
                _currentShanghaiSize = Mathf.Lerp(_currentShanghaiSize, _targetShanghaiSize, Time.deltaTime * 1.0f);
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

        protected override void UseWeapon()
        {
            base.UseWeapon();

            _targetShanghaiSize = Attributes[ShanghaiGuardAttributeType.ShanghaiSize].CurrentValue;
            _targetShanghaiRadius = Attributes[ShanghaiGuardAttributeType.ShanghaiRadius].CurrentValue;
            _leftDuration = Attributes[ShanghaiGuardAttributeType.ShanghaiDuration].CurrentValue;

            for (int i = 0; i < Attributes[ShanghaiGuardAttributeType.ShanghaiCount].CurrentValue; ++i)
            {
                GameObject shanghai = _shanghaiPool.Get();
            }
        }

        private GameObject OnCreateShanghai()
        {
            GameObject shanghai = GameObject.Instantiate(shanghaiPrefab, transform, true);

            CollisionEventEmitterTest emitter = shanghai.transform.Find("Colliders/Circle Collider 2D").GetComponent<CollisionEventEmitterTest>();
            emitter.AddHandler(_targetColliders, CollisionEventType.OnTriggerStay2D);

            return shanghai;
        }

        private void OnGetShanghai(GameObject shanghai)
        {
            shanghai.gameObject.SetActive(true);
            shanghai.transform.localPosition = Vector3.forward * shanghai.transform.localPosition.z;

            Animator animator = shanghai.GetComponent<Animator>();
            animator.Play(s_shanghaiShowHash);

            _shanghaiEnabledList.Add(shanghai);
        }

        private void OnReleaseShanghai(GameObject shanghai)
        {
            shanghai.SetActive(false);
        }

        private void OnDestroyShanghai(GameObject shanghai)
        {
            // NOTE: This block is intentionally no operation.
        }

        private void OnShanghaiStay(Collider2D collider)
        {
            Enemy enemy = collider.GetComponentInParent<Enemy>();

            if (enemy == null)
                return;

            float damage = Attributes[ShanghaiGuardAttributeType.ShanghaiDamage].CurrentValue;
            enemy.TakeDamage(damage, null, null);
        }
    }
}
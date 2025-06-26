using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Unchord
{
    public class MoscowSpell : SpellComponent
    {
        private static int s_fireballProjectileFlyingHash = Animator.StringToHash("FireballProjectileFlying");
        private static int s_fireballExplosionHash = Animator.StringToHash("FireballExplosion");

        public event Action<FireballExplosion> explHandler;

        [Header("Prefab Settings")]
        public GameObject projectilePrefab;
        public GameObject explosionPrefab;

        private int _count = 0;
        private float _cooldown = 0.0f;
        private float _angle;

        private ObjectPool<FireballProjectile> _projPool;
        private ObjectPool<FireballExplosion> _explPool;

        private List<FireballProjectile> _projTimeoutController;

        protected override void Awake()
        {
            base.Awake();

            _projPool = new ObjectPool<FireballProjectile>(
                OnCreateProjectile,
                OnGetProjectile,
                OnReleaseProjectile,
                null,
                true,
                16,
                128);

            _explPool = new ObjectPool<FireballExplosion>(
                OnCreateExplosion,
                OnGetExplosion,
                OnReleaseExplosion,
                null,
                true,
                16,
                128);

            _projTimeoutController = new List<FireballProjectile>(16);
        }

        protected override void Update()
        {
            base.Update();

            if (!base.IsCooldownPaused)
                return;

            if (_count == 0)
            {
                base.IsCooldownPaused = false;
                return;
            }

            if (_cooldown > 0.0f)
            {
                _cooldown -= Time.deltaTime;
                return;
            }

            _count--;
            _cooldown += base.AttributeBase[MoscowSpellAttributeType.BurstDelay].CurrentValue;

            UseProjectile();
        }

        public override void UseSpell()
        {
            base.IsCooldownPaused = true;

            _cooldown = 0.0f;
            _count = (int)base.AttributeBase[MoscowSpellAttributeType.BurstCount].CurrentValue - 1;
        }

        private void UseProjectile()
        {
            int directionCount = (int)base.AttributeBase[MoscowSpellAttributeType.DirectionCount].CurrentValue;
            float dAngle = 2 * Mathf.PI / (float)directionCount;
            float rAngle = Mathf.Deg2Rad * base.AttributeBase[MoscowSpellAttributeType.RotationAnglePerTick].CurrentValue;

            _angle = (_angle + rAngle) % (2 * Mathf.PI);

            Vector3 playerPosition = Player.transform.position;

            for (int i = 0; i < directionCount; ++i)
            {
                float angle = (_angle + (float)i * dAngle) % (2.0f * Mathf.PI);
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);

                Vector2 direction = new Vector2(cos, sin);

                FireballProjectile proj = _projPool.Get();
                LinearProjectile lproj = proj.projectile;

                lproj.transform.position = playerPosition;
                lproj.ProjectileSpeed = 3.0f;
                lproj.ProjectileDirection = direction;
                lproj.OriginEulerAngle = angle * Mathf.Rad2Deg;
                lproj.ProjectileDirection = direction;
                lproj.OriginEulerAngle = angle * Mathf.Rad2Deg;
            }
        }

        private void UseExplosion(FireballProjectile proj)
        {
            FireballExplosion expl = _explPool.Get();

            expl.source.transform.position = proj.source.transform.position;

            explHandler?.Invoke(expl); // 반드시 UseExplosion 함수의 마지막에서 호출되어야 함.
        }

        private FireballProjectile OnCreateProjectile()
        {
            GameObject projObject = GameObject.Instantiate(projectilePrefab.gameObject, GameManager.Instance.ProjectileContainer, true);

            FireballProjectile proj = new FireballProjectile();

            proj.explHandler += UseExplosion;

            proj.attributeBase = base.AttributeBase;

            proj.pool = _projPool;

            proj.source = projObject;
            proj.source.gameObject.SetActive(false);
            proj.projectile = projObject.GetComponent<LinearProjectile>();
            proj.emitter = projObject.transform.Find("Colliders/Circle Collider 2D").GetComponent<CollisionEventEmitter>();
            proj.emitter.onTriggerEnter2D += proj.OnHit;
            proj.animator = projObject.GetComponent<Animator>();
            proj.flag = projObject.GetComponent<FlagComponent>();

            proj.leftPenetrationCount = 0;
            proj.penetratedEnemies = new List<Enemy>(16);

            return proj;
        }

        private void OnGetProjectile(FireballProjectile proj)
        {
            proj.leftPenetrationCount = (int)base.AttributeBase[FireballAttributeType.ProjectilePenetrationCount].CurrentValue;
            proj.penetratedEnemies.Clear();

            float scale = base.AttributeBase[FireballAttributeType.ProjectileSize].CurrentValue;
            proj.source.transform.localScale = new Vector3(scale, scale, 1.0f);

            proj.source.gameObject.SetActive(true);

            proj.absoluteTimeout = GameManager.Instance.AbsolutePlaytime + 30.0f;
            _projTimeoutController.Add(proj);

            proj.animator.Play(s_fireballProjectileFlyingHash, -1, 0.0f);
        }

        private void OnReleaseProjectile(FireballProjectile proj)
        {
            _projTimeoutController.Remove(proj);

            proj.source.gameObject.SetActive(false);
        }

        private FireballExplosion OnCreateExplosion()
        {
            GameObject explObject = GameObject.Instantiate(explosionPrefab.gameObject, GameManager.Instance.ProjectileContainer, true);

            FireballExplosion expl = new FireballExplosion();

            expl.attributeBase = base.AttributeBase;

            expl.pool = _explPool;

            expl.source = explObject;
            expl.source.gameObject.SetActive(false);
            expl.projectile = explObject.GetComponent<DotProjectile>();
            expl.emitter = explObject.transform.Find("Colliders/Circle Collider 2D").GetComponent<CollisionEventEmitter>();
            expl.emitter.onTriggerEnter2D += expl.OnHit;
            expl.animator = explObject.GetComponent<Animator>();
            expl.flag = explObject.GetComponent<FlagComponent>();
            expl.flag.AddEventTrue(AbilityComponent.FLAG_SHOULD_DESTROY, expl.OnAnimationEnd);

            return expl;
        }

        private void OnGetExplosion(FireballExplosion expl)
        {
            expl.flag.SetFlagFalseWithoutEvent(AbilityComponent.FLAG_SHOULD_DESTROY);

            expl.source.gameObject.SetActive(true);

            float scale = base.AttributeBase[FireballAttributeType.ExplosionSize].CurrentValue;
            expl.source.transform.localScale = new Vector3(scale, scale, 1.0f);

            expl.animator.Play(s_fireballExplosionHash, -1, 0.0f);
        }

        private void OnReleaseExplosion(FireballExplosion expl)
        {
            expl.source.gameObject.SetActive(false);
        }
    }
}
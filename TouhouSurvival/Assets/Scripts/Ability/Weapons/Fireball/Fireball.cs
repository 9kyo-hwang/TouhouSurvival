using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Unchord
{
    public class Fireball : WeaponComponent
    {
        private static int s_fireballProjectileFlyingHash = Animator.StringToHash("FireballProjectileFlying");
        private static int s_fireballExplosionHash = Animator.StringToHash("FireballExplosion");

        public event Action<FireballExplosion> explHandler;

        [Header("Prefab Settings")]
        public GameObject projectilePrefab;
        public GameObject explosionPrefab;

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

            for (int i = _projTimeoutController.Count - 1; i >= 0; --i)
            {
                FireballProjectile proj = _projTimeoutController[i];

                if (proj.absoluteTimeout <= GameManager.Instance.AbsolutePlaytime)
                {
                    proj.OnTimeout();
                }
            }
        }

        public override void UseWeapon()
        {
            UseProjectile();
        }

        private void UseProjectile()
        {
            GameObject nearestEnemy = Player.GetNearestEnemyOrNull();

            if (!nearestEnemy)
                return;

            FireballProjectile proj = _projPool.Get();
            LinearProjectile lproj = proj.projectile;

            Vector3 playerPosition = Player.transform.position;
            Vector3 enemyPosition = nearestEnemy.transform.position;

            lproj.transform.position = playerPosition;
            lproj.ProjectileSpeed = 3.0f;

            float angleError = base.AttributeBase[FireballAttributeType.ShootingEulerAngleError].CurrentValue;

            lproj.ProjectileDirection = Projectile.GetTargetDirectionVector(playerPosition, enemyPosition, angleError);
            lproj.OriginEulerAngle = Vector2.SignedAngle(Vector2.right, lproj.ProjectileDirection);
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
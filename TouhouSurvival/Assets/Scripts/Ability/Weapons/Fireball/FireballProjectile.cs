using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Unchord
{
    public class FireballProjectile
    {
        public event Action<FireballProjectile> explHandler;

        public AttributeBaseSet attributeBase;

        public ObjectPool<FireballProjectile> pool;

        public GameObject source;
        public LinearProjectile projectile;
        public CollisionEventEmitter emitter;
        public Animator animator;
        public FlagComponent flag;

        public int leftPenetrationCount;
        public List<Enemy> penetratedEnemies;

        public float absoluteTimeout;

        public virtual void OnHit(object sender, CollisionEventArgs args)
        {
            Enemy enemy = args.targetObject.GetComponentInParent<Enemy>();

            UnityEngine.Debug.Assert(enemy != null);

            if (penetratedEnemies.Contains(enemy))
                return;

            penetratedEnemies.Add(enemy);

            if (leftPenetrationCount >= 0)
                explHandler?.Invoke(this);

            if (leftPenetrationCount-- == 0)
                pool.Release(this);

            if (enemy.AttributeBase[EnemyAttributeType.Health].CurrentValue > 0.0f)
            {
                float damage = attributeBase[FireballAttributeType.ProjectileDamage].CurrentValue;
                enemy.TakeDamage(damage, null, null);
            }
        }

        public void OnTimeout()
        {
            pool.Release(this);
        }
    }
}
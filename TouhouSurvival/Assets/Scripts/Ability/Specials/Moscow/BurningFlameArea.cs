using System;
using UnityEngine;
using UnityEngine.Pool;

namespace Unchord
{
    public class BurningFlameArea
    {
        public AttributeBaseSet attributeBase;

        public ObjectPool<BurningFlameArea> pool;

        public float duration;
        public float tickPeriod;

        public float flameTimeout;
        public float flameTickTime;

        public GameObject source;
        public CircleCollider2D collider;
        public CollisionEventEmitter emitter;

        public virtual void OnHit(object sender, CollisionEventArgs args)
        {
            collider.enabled = false;

            Enemy enemy = args.targetObject.GetComponentInParent<Enemy>();

            UnityEngine.Debug.Assert(enemy != null);

            float damage = attributeBase["FlameDamage"].CurrentValue;
            enemy.TakeDamage(damage);
        }

        public void OnTimeout()
        {
            pool.Release(this);
        }
    }
}
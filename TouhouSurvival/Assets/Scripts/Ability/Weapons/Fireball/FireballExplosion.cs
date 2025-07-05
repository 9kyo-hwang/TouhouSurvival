using UnityEngine;
using UnityEngine.Pool;

namespace Unchord
{
    public class FireballExplosion
    {
        public AttributeBaseSet attributeBase;

        public ObjectPool<FireballExplosion> pool;

        public GameObject source;
        public DotProjectile projectile;
        public CollisionEventEmitter emitter;
        public Animator animator;
        public FlagComponent flag;

        public virtual void OnHit(object sender, CollisionEventArgs args)
        {
            Enemy enemy = args.targetObject.GetComponentInParent<Enemy>();

            UnityEngine.Debug.Assert(enemy != null);

            if (enemy.AttributeBase[EnemyAttributeType.Health].CurrentValue > 0.0f)
            {
                float damage = attributeBase[FireballAttributeType.ExplosionDamage].CurrentValue;
                enemy.TakeDamage(damage);
            }
        }

        public virtual void OnAnimationEnd(FlagComponent flagComponent)
        {
            pool.Release(this);
        }
    }
}

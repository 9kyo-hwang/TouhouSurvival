using UnityEngine;

namespace Unchord
{
    public class LinearProjectile : Projectile
    {
        public float ProjectileSpeed { get; set; }
        public Vector3 ProjectileDirection { get; set; }

        private Vector3 _deltaPosition;

        protected override void OnEnable()
        {
            base.OnEnable();

            _deltaPosition = Vector3.zero;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            _deltaPosition += ProjectileSpeed * Time.fixedDeltaTime * ProjectileDirection;
            transform.position += _deltaPosition;

            base.FlagTable[AbilityComponent.FLAG_SHOULD_DESTROY] |= ScreenBounds.EvalScreenZone(transform.position) == ScreenZone.Dead;
        }
    }
}
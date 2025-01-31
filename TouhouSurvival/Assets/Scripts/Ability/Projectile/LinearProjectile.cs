using UnityEngine;

namespace Unchord
{
    public class LinearProjectile : Projectile
    {
        public float ProjectileSpeed { get; set; }
        public Vector3 ProjectileDirection
        {
            get => _projectileDirection;
            set
            {
                _projectileDirection = value;

                float projectileEulerAngle = Vector2.SignedAngle(Vector2.right, value);
                transform.eulerAngles = Vector3.forward * projectileEulerAngle;
            }
        }

        private Vector3 _projectileDirection;

        protected void FixedUpdate()
        {
            transform.position += ProjectileSpeed * Time.fixedDeltaTime * ProjectileDirection;

            base.FlagTable[AbilityComponent.FLAG_SHOULD_DESTROY] |= ScreenBounds.EvalScreenZone(transform.position) == ScreenZone.Dead;
        }
    }
}
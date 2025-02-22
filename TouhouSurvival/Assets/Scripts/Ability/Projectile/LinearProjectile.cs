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

        private Vector3 _nextPosition;
        private Vector3 _projectileDirection;

        protected override void OnEnable()
        {
            base.OnEnable();

            _nextPosition = transform.position;
        }

        protected void FixedUpdate()
        {
            _nextPosition += ProjectileSpeed * Time.fixedDeltaTime * ProjectileDirection;
            transform.position = _nextPosition;

            base.FlagTable[AbilityComponent.FLAG_SHOULD_DESTROY] |= ScreenBounds.EvalScreenZone(_nextPosition) == ScreenZone.Dead;
        }
    }
}
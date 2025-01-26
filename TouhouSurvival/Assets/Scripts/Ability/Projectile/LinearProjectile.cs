using UnityEngine;

namespace Unchord
{
    public class LinearProjectile : Projectile
    {
        public float ProjectileSpeed { get; set; }
        public float ProjectileEulerAngle { get; set; }
        
        protected void FixedUpdate()
        {
            float cos = Mathf.Cos(ProjectileEulerAngle * Mathf.Deg2Rad);
            float sin = Mathf.Sin(ProjectileEulerAngle * Mathf.Deg2Rad);
            Vector3 direction = new Vector3(cos, sin, 0);

            transform.position += ProjectileSpeed * Time.fixedDeltaTime * direction;
            transform.eulerAngles = Vector3.forward * ProjectileEulerAngle;

            base.ShouldDestroy |= ScreenBounds.EvalScreenZone(transform.position) == ScreenZone.Dead;
        }
    }
}
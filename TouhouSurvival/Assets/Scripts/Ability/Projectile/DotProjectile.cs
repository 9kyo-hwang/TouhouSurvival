using UnityEngine;

namespace Unchord
{
    public class DotProjectile : Projectile
    {
        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            transform.position = base.OriginPosition;
        }
    }
}
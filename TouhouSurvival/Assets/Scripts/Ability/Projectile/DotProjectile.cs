using UnityEngine;

namespace Unchord
{
    public class DotProjectile : Projectile
    {
        protected void FixedUpdate()
        {
            transform.position = base.OriginPosition;
        }
    }
}
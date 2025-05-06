using UnityEngine;

namespace Unchord
{
    public class PlayerTrackingProjectile : Projectile
    {
        public Vector2 DeltaPosition { get; set; }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            transform.position = GameManager.Instance.Player.transform.position + (Vector3)DeltaPosition;
        }
    }
}
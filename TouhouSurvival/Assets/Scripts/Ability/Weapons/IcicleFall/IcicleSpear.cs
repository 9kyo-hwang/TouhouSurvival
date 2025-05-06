using System.Collections;
using UnityEngine;

namespace Unchord
{
    public class IcicleSpear : MonoBehaviour
    {
        public void Launch(Vector3 startPosition, Vector3 endPosition, float launchAngleOffset, float speed,
            float duration)
        {
            LinearProjectile projectile = GetComponent<LinearProjectile>();
            projectile.transform.position = GameManager.Instance.Player.transform.position;
            projectile.ProjectileSpeed = speed;
            projectile.ProjectileDirection =
                Projectile.GetTargetDirectionVector(startPosition, endPosition, launchAngleOffset);
            projectile.OriginEulerAngle = Vector2.SignedAngle(Vector2.right, projectile.ProjectileDirection) - 45.0f;
            projectile.RotationSpeed = 0.0f;

            StartCoroutine(Timeout(duration));
        }

        private IEnumerator Timeout(float duration)
        {
            yield return new WaitForSeconds(duration);
            
            GetComponentInParent<LinearProjectile>(includeInactive: true)
                .FlagTable[AbilityComponent.FLAG_SHOULD_DESTROY] = true;
        }
    }
}

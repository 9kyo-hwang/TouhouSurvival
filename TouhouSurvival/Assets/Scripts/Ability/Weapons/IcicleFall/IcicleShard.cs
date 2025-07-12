using System.Collections;
using UnityEngine;

namespace Unchord
{
    public class IcicleShard : MonoBehaviour
    {
        public void Launch(Vector3 spearPosition, float speed, Vector2 direction, float duration)
        {
            LinearProjectile projectile = GetComponent<LinearProjectile>();
            projectile.transform.position = spearPosition;
            projectile.ProjectileSpeed = speed;
            projectile.ProjectileDirection = direction;
            projectile.OriginEulerAngle = Vector2.SignedAngle(Vector2.right, direction) - 45.0f;

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

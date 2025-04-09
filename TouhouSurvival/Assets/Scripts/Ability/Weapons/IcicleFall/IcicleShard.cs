using System.Collections;
using UnityEngine;

namespace Unchord
{
    public class IcicleShard : MonoBehaviour
    {
        public void Launch(Vector3 spearPosition, float speed, Vector2 direction, float duration)
        {
            LinearProjectile projectile = GetComponent<LinearProjectile>();
            projectile.transform.position = Vector3.zero;
            projectile.OriginPosition = spearPosition;
            projectile.ProjectileSpeed = speed;
            projectile.ProjectileDirection = direction;

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

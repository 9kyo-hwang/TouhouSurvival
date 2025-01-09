using System;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyScanner : MonoBehaviour
{
    [SerializeField] private float range;
    [SerializeField] private LayerMask targetLayer;
    private RaycastHit2D[] _targets;
    [SerializeField] private Transform nearestTarget;

    private void FixedUpdate()
    {
        _targets = Physics2D.CircleCastAll(transform.position, range, Vector2.zero, 0, targetLayer);
        FindNearestTarget(out nearestTarget);
    }

    public bool FindNearestTarget(out Transform targetTransform)
    {
        float nearestDistance = Mathf.Infinity;
        targetTransform = null;
        
        foreach (RaycastHit2D target in _targets)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                targetTransform = target.transform;
            }
        }

        return targetTransform;
    }
}

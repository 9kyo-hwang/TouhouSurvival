using System;
using UnityEngine;

namespace Unchord
{
    [RequireComponent(typeof(FlagComponent))]
    public abstract class Projectile : MonoBehaviour
    {
        public FlagComponent FlagTable { get; private set; }

        public LayerMask layerMask;

        protected virtual void Awake()
        {
            FlagTable = GetComponent<FlagComponent>();
        }

        protected virtual void OnEnable()
        {
            FlagTable[AbilityComponent.FLAG_SHOULD_DESTROY] = false;
            transform.localPosition = Vector3.zero;
            transform.eulerAngles = Vector3.zero;
        }

        protected virtual void OnDisable()
        {
            FlagTable[AbilityComponent.FLAG_SHOULD_DESTROY] = false;
        }

        public static Vector2 GetTargetDirectionVector(Vector2 origin, Vector2 target, float eulerAngleError)
        {
            Vector2 diffVector = target - origin;
            float randomRadian = Mathf.Deg2Rad * 2.0f * eulerAngleError * (UnityEngine.Random.value - 0.5f);

            float a = diffVector.x;
            float b = diffVector.y;
            float c = UnityEngine.Mathf.Cos(randomRadian);
            float d = UnityEngine.Mathf.Sin(randomRadian);

            return new Vector2(a * c - b * d, a * d + b * c).normalized;
        }
    }
}
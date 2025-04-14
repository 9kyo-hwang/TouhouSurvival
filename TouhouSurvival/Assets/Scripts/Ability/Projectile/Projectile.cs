using System;
using UnityEngine;

namespace Unchord
{
    [RequireComponent(typeof(FlagComponent))]
    public abstract class Projectile : MonoBehaviour
    {
        public FlagComponent FlagTable { get; private set; }
        public Vector3 OriginPosition { get; set; }
        public float OriginEulerAngle { get; set; }
        public float RotationSpeed { get; set; } = 0.0f;

        private float _deltaRotationEulerAngle = 0.0f;

        protected virtual void Awake()
        {
            FlagTable = GetComponent<FlagComponent>();
        }

        protected virtual void OnEnable()
        {
            // TODO: Projectile을 사용하는 Pool의 Event Handler에서 Flag를 세팅할지, 여기세 세팅할지 추후 결정해볼 문제.
            // FlagTable.SetFlagFalseWithoutEvent(AbilityComponent.FLAG_SHOULD_DESTROY);
            transform.localPosition = Vector3.zero;
            transform.eulerAngles = Vector3.zero;
        }

        protected virtual void FixedUpdate()
        {
            _deltaRotationEulerAngle += Time.deltaTime * RotationSpeed;
            _deltaRotationEulerAngle %= 360.0f;
            transform.eulerAngles = Vector3.forward * ((OriginEulerAngle + _deltaRotationEulerAngle) % 360.0f);
        }

        protected virtual void OnDisable()
        {
            // FlagTable.SetFlagFalseWithoutEvent(AbilityComponent.FLAG_SHOULD_DESTROY);
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
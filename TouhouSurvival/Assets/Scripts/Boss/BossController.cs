using System;
using UnityEngine;

namespace Unchord
{
    public class BossController : MonoBehaviour
    {
        public Animator Animator { get; protected set; }
        protected Rigidbody Rigidbody;

        private void OnEnable()
        {
            Animator = GetComponent<Animator>();
            Animator.updateMode = AnimatorUpdateMode.Fixed;

            Rigidbody = GetComponentInChildren<Rigidbody>();
            if (!Rigidbody)
            {
                Rigidbody = gameObject.AddComponent<Rigidbody>();
            }

            Rigidbody.isKinematic = true;
            Rigidbody.useGravity = false;
            Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        }

        private void FixedUpdate()
        {
            Animator.speed = 1.0f;
        }

        public void SetForward(Vector3 forward)
        {
            Quaternion targetRotation = Quaternion.LookRotation(forward);
            transform.rotation = targetRotation;
        }
    }
}

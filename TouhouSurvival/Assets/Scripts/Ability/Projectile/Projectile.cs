using System;
using UnityEngine;

namespace Unchord
{
    public abstract class Projectile : MonoBehaviour
    {
        public bool ShouldDestroy { get; set; }

        public LayerMask layerMask;

        protected virtual void OnEnable()
        {
            ShouldDestroy = false;
            transform.localPosition = Vector3.zero;
            transform.eulerAngles = Vector3.zero;
        }

        protected virtual void OnDisable()
        {
            ShouldDestroy = false;
        }
    }
}
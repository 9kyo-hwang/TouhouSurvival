using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    public class CollisionEventEmitter : MonoBehaviour
    {
        public LayerMask targetLayerMask;

        public event EventHandler<CollisionEventArgs> onTriggerEnter2D;
        public event EventHandler<CollisionEventArgs> onTriggerStay2D;
        public event EventHandler<CollisionEventArgs> onTriggerExit2D;

        private bool IsIgnoredCollider(Collider2D collider)
        {
            return (targetLayerMask & (1 << collider.gameObject.layer)) == 0;
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (IsIgnoredCollider(collider))
                return;

            CollisionEventArgs args = new CollisionEventArgs(this.gameObject, collider.gameObject);
            onTriggerEnter2D?.Invoke(this, args);
        }

        private void OnTriggerStay2D(Collider2D collider)
        {
            if (IsIgnoredCollider(collider))
                return;

            CollisionEventArgs args = new CollisionEventArgs(this.gameObject, collider.gameObject);
            onTriggerStay2D?.Invoke(this, args);
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            if (IsIgnoredCollider(collider))
                return;

            CollisionEventArgs args = new CollisionEventArgs(this.gameObject, collider.gameObject);
            onTriggerExit2D?.Invoke(this, args);
        }
    }
}
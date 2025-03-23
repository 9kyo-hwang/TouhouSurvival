using System;
using UnityEngine;

namespace Unchord
{
    public class CollisionEventArgs : EventArgs
    {
        public readonly GameObject eventSource;
        public readonly GameObject targetObject;

        public CollisionEventArgs(GameObject eventSource, GameObject targetObject)
        {
            this.eventSource = eventSource;
            this.targetObject = targetObject;
        }
    }
}
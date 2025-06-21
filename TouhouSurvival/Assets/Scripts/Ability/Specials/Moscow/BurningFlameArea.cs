using System;
using UnityEngine;

namespace Unchord
{
    public class BurningFlameArea
    {
        public float getTime;
        public float duration;

        public float lastTickedTime;
        public float tickPeriod;

        public GameObject source;
        public CollisionEventEmitter emitter;

        public bool ShouldRelease(float currentTime)
        {
            return currentTime - getTime >= duration;
        }

        public bool ShouldTick(float currentTime)
        {
            return currentTime - lastTickedTime >= tickPeriod;
        }

        public void PublishEvent(EventHandler<CollisionEventArgs> handler, float currentTime)
        {
            lastTickedTime = currentTime;

            emitter.onTriggerEnter2D += handler;
            emitter.onTriggerEnter2D += OnTriggerEnter2D;
        }

        private void OnTriggerEnter2D(object sender, CollisionEventArgs args)
        {
            emitter = null;
        }
    }
}
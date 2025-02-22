using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    public class CollisionEventEmitter : MonoBehaviour
    {
        [Header("Collider Ignorance Criterias")]
        public LayerMask targetLayerMask;

        private class _HandlerInfo<T_Handler>
        {
            public int refCount;
            public T_Handler handler;
        }

        private SortedDictionary<TriggerEventHandler2D, _HandlerInfo<TriggerEventHandler2D>> _onTriggerEnter2D;
        private SortedDictionary<TriggerEventHandler2D, _HandlerInfo<TriggerEventHandler2D>> _onTriggerStay2D;
        private SortedDictionary<TriggerEventHandler2D, _HandlerInfo<TriggerEventHandler2D>> _onTriggerExit2D;

        private void Awake()
        {
            _onTriggerEnter2D = new SortedDictionary<TriggerEventHandler2D, _HandlerInfo<TriggerEventHandler2D>>();
            _onTriggerStay2D = new SortedDictionary<TriggerEventHandler2D, _HandlerInfo<TriggerEventHandler2D>>();
            _onTriggerExit2D = new SortedDictionary<TriggerEventHandler2D, _HandlerInfo<TriggerEventHandler2D>>();
        }

        public void AddHandler(string gameObjectNamePath, TriggerEventHandler2D eventHandler, CollisionEventType eventType, bool cascadeAdding = true)
        {
            Transform root = transform.Find(gameObjectNamePath);

            if (root == null)
            {
                return;
            }

            if (!cascadeAdding)
            {
                CollisionEventEmitter emitter = root.GetComponent<CollisionEventEmitter>();

                if (emitter != null)
                    emitter.AddHandler(eventHandler, eventType);

                return;
            }

            CollisionEventEmitter[] emitters = root.GetComponentsInChildren<CollisionEventEmitter>();

            for (int i = 0; i < emitters.Length; ++i)
            {
                emitters[i].AddHandler(eventHandler, eventType);
            }
        }

        public void RemoveHandler(string gameObjectNamePath, TriggerEventHandler2D eventHandler, CollisionEventType eventType, bool cascadeRemoving = true)
        {
            Transform root = transform.Find(gameObjectNamePath);

            if (root == null)
            {
                return;
            }

            if (!cascadeRemoving)
            {
                CollisionEventEmitter emitter = root.GetComponent<CollisionEventEmitter>();

                if (emitter != null)
                    emitter.RemoveHandler(eventHandler, eventType);

                return;
            }

            CollisionEventEmitter[] emitters = root.GetComponentsInChildren<CollisionEventEmitter>();

            for (int i = 0; i < emitters.Length; ++i)
            {
                emitters[i].AddHandler(eventHandler, eventType);
            }
        }

        public void AddHandler(TriggerEventHandler2D eventHandler, CollisionEventType eventType)
        {
            switch (eventType)
            {
                case CollisionEventType.OnTriggerEnter2D:
                    AddHandler(_onTriggerEnter2D, eventHandler);
                    break;
                case CollisionEventType.OnTriggerStay2D:
                    AddHandler(_onTriggerStay2D, eventHandler);
                    break;
                case CollisionEventType.OnTriggerExit2D:
                    AddHandler(_onTriggerExit2D, eventHandler);
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
        }

        public void RemoveHandler(TriggerEventHandler2D eventHandler, CollisionEventType eventType)
        {
            switch (eventType)
            {
                case CollisionEventType.OnTriggerEnter2D:
                    RemoveHandler(_onTriggerEnter2D, eventHandler);
                    break;
                case CollisionEventType.OnTriggerStay2D:
                    RemoveHandler(_onTriggerStay2D, eventHandler);
                    break;
                case CollisionEventType.OnTriggerExit2D:
                    RemoveHandler(_onTriggerExit2D, eventHandler);
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
        }

        private void AddHandler(SortedDictionary<TriggerEventHandler2D, _HandlerInfo<TriggerEventHandler2D>> table, TriggerEventHandler2D handler)
        {
            if (!table.ContainsKey(handler))
                table.Add(handler, new _HandlerInfo<TriggerEventHandler2D>());

            table[handler].refCount++;
            table[handler].handler += handler;
        }

        private void RemoveHandler(SortedDictionary<TriggerEventHandler2D, _HandlerInfo<TriggerEventHandler2D>> table, TriggerEventHandler2D handler)
        {
            if (!table.ContainsKey(handler) || table[handler].refCount == 0)
                return;

            table[handler].refCount--;
            table[handler].handler -= handler;
        }

        private bool IsIgnoredCollider(Collider2D collider)
        {
            return (targetLayerMask & (1 << collider.gameObject.layer)) == 0;
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (IsIgnoredCollider(collider))
                return;

            foreach (_HandlerInfo<TriggerEventHandler2D> handlerInfo in _onTriggerEnter2D.Values)
            {
                handlerInfo.handler?.Invoke(this.gameObject, collider);
            }
        }

        private void OnTriggerStay2D(Collider2D collider)
        {
            if (IsIgnoredCollider(collider))
                return;

            foreach (_HandlerInfo<TriggerEventHandler2D> handlerInfo in _onTriggerStay2D.Values)
            {
                handlerInfo.handler?.Invoke(this.gameObject, collider);
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            if (IsIgnoredCollider(collider))
                return;

            foreach (_HandlerInfo<TriggerEventHandler2D> handlerInfo in _onTriggerExit2D.Values)
            {
                handlerInfo.handler?.Invoke(this.gameObject, collider);
            }
        }
    }
}
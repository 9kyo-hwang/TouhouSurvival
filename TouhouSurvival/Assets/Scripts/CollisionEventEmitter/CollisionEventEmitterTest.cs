using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    public class CollisionEventEmitterTest : MonoBehaviour
    {
        private List<Queue<Collider2D>> _onTriggerEnter2D;
        private List<Queue<Collider2D>> _onTriggerStay2D;
        private List<Queue<Collider2D>> _onTriggerExit2D;

        private void Awake()
        {
            _onTriggerEnter2D = new List<Queue<Collider2D>>(2);
            _onTriggerStay2D = new List<Queue<Collider2D>>(2);
            _onTriggerExit2D = new List<Queue<Collider2D>>(2);
        }

        private void Start()
        {

        }

        private void Update()
        {

        }

        public void AddHandler(Queue<Collider2D> queue, CollisionEventType eventType)
        {
            switch (eventType)
            {
                case CollisionEventType.OnTriggerEnter2D:
                    _onTriggerEnter2D.Add(queue);
                    break;
                case CollisionEventType.OnTriggerStay2D:
                    _onTriggerStay2D.Add(queue);
                    break;
                case CollisionEventType.OnTriggerExit2D:
                    _onTriggerExit2D.Add(queue);
                    break;
                default:
                    UnityEngine.Debug.Assert(false);
                    break;
            }
        }

        public void RemoveHandler(Queue<Collider2D> queue, CollisionEventType eventType)
        {
            switch (eventType)
            {
                case CollisionEventType.OnTriggerEnter2D:
                    _onTriggerEnter2D.Remove(queue);
                    break;
                case CollisionEventType.OnTriggerStay2D:
                    _onTriggerStay2D.Remove(queue);
                    break;
                case CollisionEventType.OnTriggerExit2D:
                    _onTriggerExit2D.Remove(queue);
                    break;
                default:
                    UnityEngine.Debug.Assert(false);
                    break;
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            for (int i = 0; i < _onTriggerEnter2D.Count; ++i)
            {
                _onTriggerEnter2D[i].Enqueue(collider);
            }
        }

        private void OnTriggerStay2D(Collider2D collider)
        {
            for (int i = 0; i < _onTriggerStay2D.Count; ++i)
            {
                _onTriggerStay2D[i].Enqueue(collider);
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            for (int i = 0; i < _onTriggerExit2D.Count; ++i)
            {
                _onTriggerExit2D[i].Enqueue(collider);
            }
        }
    }
}
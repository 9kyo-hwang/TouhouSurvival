using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    public class BlockingEventHandler : MonoBehaviour
    {
        public event Action onBlockingEventOccurred;
        public event Action onBlockingEventHandled;

        private Queue<IEnumerator> _eventHandlers;
        public float _handlingCooldown;
        public bool _eventEnabled;

        private GameManager _gameManager;

        public void Publish(IEnumerator eventHandler)
        {
            _eventHandlers.Enqueue(eventHandler);
            _gameManager = GameManager.Instance;
        }

        private void Awake()
        {
            _eventHandlers = new Queue<IEnumerator>(16);
        }

        private void Update()
        {
            if (_eventEnabled)
                return;
            else if (_handlingCooldown > 0.0f)
                _handlingCooldown -= Time.deltaTime;
            else if (_eventHandlers.Count > 0)
                StartCoroutine(HandleEventCoroutine(_eventHandlers.Dequeue()));
        }

        private IEnumerator HandleEventCoroutine(IEnumerator eventHandler)
        {
            _gameManager.InterruptTimeStop();
            _eventEnabled = true;
            onBlockingEventOccurred?.Invoke();
            yield return StartCoroutine(eventHandler);
            _handlingCooldown = 1.0f;
            _eventEnabled = false;
            onBlockingEventHandled?.Invoke();
            _gameManager.ReleaseTimeStopInterrupt();
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    public class BlockingEventHandler : MonoBehaviour
    {
        private Queue<IEnumerator> _eventHandlers;
        private float _handlingCooldown;
        private bool _eventEnabled;

        private GameManager _gameManager;

        public void Publish(IEnumerator eventHandler)
        {
            if (!_gameManager.IsGameStarted)
                return;

            _eventHandlers.Enqueue(eventHandler);
        }

        public void SetCooldown(float cooldown)
        {
            if (cooldown <= 0.0f)
                _handlingCooldown = cooldown;
        }

        private void Awake()
        {
            _gameManager = GameManager.Instance;
            _eventHandlers = new Queue<IEnumerator>(16);

            _handlingCooldown = 0.0f;
        }

        private void Update()
        {
            if (_eventEnabled)
                return;
            else if (_handlingCooldown > 0.0f)
                _handlingCooldown -= Time.deltaTime;
            else if (_eventHandlers.Count > 0)
                StartCoroutine(HandleEventCoroutine());
        }

        private IEnumerator HandleEventCoroutine()
        {
            _gameManager.InterruptTimeStop();
            _eventEnabled = true;
            while (_eventHandlers.Count > 0)
                yield return StartCoroutine(_eventHandlers.Dequeue());
            _eventEnabled = false;
            _gameManager.ReleaseTimeStopInterrupt();
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Pool;

namespace Unchord
{
    // 2-3
    public class BurningFlame : SpecialAbilityComponent
    {
        public GameObject flamePrefab;

        private List<BurningFlameArea> _nodes;
        private ObjectPool<BurningFlameArea> _flamePool;
        private Queue<BurningFlameArea> _eventQueue;
        private Queue<BurningFlameArea> _readyQueue;
        
        protected override void Awake()
        {
            base.Awake();

            _nodes = new List<BurningFlameArea>(16);
            _flamePool = new ObjectPool<BurningFlameArea>(
                OnCreateFlame
                );
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            float currentTime = GameManager.Instance.AbsolutePlaytime;

            int n = 0;

            n = _readyQueue.Count;

            for (int i = 0; i < n; ++i)
            {
                BurningFlameArea flame = _readyQueue.Dequeue();

                if (flame.ShouldRelease(currentTime))
                    _flamePool.Release(flame);
                else if (flame.ShouldTick(currentTime))
                    _eventQueue.Enqueue(flame);
                else
                    _readyQueue.Enqueue(flame);
            }

            n = _eventQueue.Count;

            for (int i = 0; i < n; ++i)
            {
                BurningFlameArea flame = _eventQueue.Dequeue();

                if (flame.ShouldRelease(currentTime))
                {
                    _flamePool.Release(flame);
                    continue;
                }

                flame.PublishEvent(OnFlameEnter, currentTime);
                _readyQueue.Enqueue(flame);
            }
        }

        public void GenerateFlame()
        {
            BurningFlameArea flame = _flamePool.Get();

            _eventQueue.Enqueue(flame);
        }

        private BurningFlameArea OnCreateFlame()
        {
            GameObject flameObject = GameObject.Instantiate(flamePrefab, GameManager.Instance.ProjectileContainer, true);

            BurningFlameArea flame = new BurningFlameArea();

            flame.getTime = GameManager.Instance.AbsolutePlaytime;
            flame.duration = base.AttributeBase["FlameDuration"].CurrentValue;
            flame.lastTickedTime = -base.AttributeBase["FlameTickTime"].CurrentValue;
            flame.tickPeriod = base.AttributeBase["FlameTickTime"].CurrentValue;
            flame.source = flameObject;
            flame.emitter = flameObject.transform.Find("Colliders/Circle Collider 2D").GetComponent<CollisionEventEmitter>();

            return flame;
        }

        private void OnFlameEnter(object sender, CollisionEventArgs args)
        {
            Enemy enemy = args.targetObject.GetComponent<Enemy>();

            UnityEngine.Debug.Assert(enemy != null);

            float damage = base.AttributeBase["FlameDamage"].CurrentValue;
            enemy.TakeDamage(damage, null, null);
        }
    }
}
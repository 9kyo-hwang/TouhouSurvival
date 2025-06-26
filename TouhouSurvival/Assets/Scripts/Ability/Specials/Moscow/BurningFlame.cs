using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace Unchord
{
    // 2-3
    public class BurningFlame : SpecialAbilityComponent
    {
        public GameObject flamePrefab;

        private ObjectPool<BurningFlameArea> _flamePool;
        private List<BurningFlameArea> _flameEnabled;
        
        protected override void Awake()
        {
            base.Awake();

            _flamePool = new ObjectPool<BurningFlameArea>(
                OnCreateFlame,
                OnGetFlame,
                OnReleaseFlame,
                null,
                true,
                16,
                128);

            _flameEnabled = new List<BurningFlameArea>(16);
        }

        protected override void Update()
        {
            float currentTime = GameManager.Instance.AbsolutePlaytime;
            
            for (int i = _flameEnabled.Count - 1; i >= 0; --i)
            {
                BurningFlameArea flame = _flameEnabled[i];

                if (currentTime >= flame.flameTimeout)
                {
                    flame.OnTimeout();
                    continue;
                }
                else if (currentTime >= flame.flameTickTime)
                {
                    flame.flameTickTime += flame.tickPeriod;
                    flame.collider.enabled = true;
                }
            }
        }

        protected override void OnEnableSpecial()
        {
            Fireball weapon = base.Player.WeaponTransform.GetComponentInChildren<Fireball>();
            MoscowSpell spell = base.Player.SpellTransform.GetComponentInChildren<MoscowSpell>();

            weapon.explHandler += UseFlame;
            spell.explHandler += UseFlame;
        }

        private void UseFlame(FireballExplosion expl)
        {
            BurningFlameArea flame = _flamePool.Get();

            flame.source.transform.position = expl.source.transform.position;
        }

        private BurningFlameArea OnCreateFlame()
        {
            GameObject flameObject = GameObject.Instantiate(flamePrefab, GameManager.Instance.ProjectileContainer, true);

            BurningFlameArea flame = new BurningFlameArea();

            flame.attributeBase = base.AttributeBase;

            flame.pool = _flamePool;

            flame.source = flameObject;
            flame.collider = flameObject.transform.Find("Colliders/Circle Collider 2D").GetComponent<CircleCollider2D>();
            flame.emitter = flameObject.transform.Find("Colliders/Circle Collider 2D").GetComponent<CollisionEventEmitter>();
            flame.emitter.onTriggerEnter2D += flame.OnHit;

            flame.collider.enabled = false;

            flame.source.gameObject.SetActive(false);

            return flame;
        }

        private void OnGetFlame(BurningFlameArea flame)
        {
            flame.duration = base.AttributeBase["FlameDuration"].CurrentValue;
            flame.tickPeriod = base.AttributeBase["FlameTickTime"].CurrentValue;

            flame.source.gameObject.SetActive(true);

            float currentTime = GameManager.Instance.AbsolutePlaytime;
            flame.flameTimeout = currentTime + flame.duration;
            flame.flameTickTime = currentTime;

            _flameEnabled.Add(flame);
        }

        private void OnReleaseFlame(BurningFlameArea flame)
        {
            _flameEnabled.Remove(flame);

            flame.source.gameObject.SetActive(false);
        }
    }
}
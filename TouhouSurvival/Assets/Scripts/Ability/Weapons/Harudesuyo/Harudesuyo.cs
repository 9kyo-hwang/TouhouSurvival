using System.Collections;
using System.Linq;
using Unchord;
using UnityEngine;

namespace Unchord
{
    public class Harudesuyo : WeaponComponent
    {
        [Header("Prefab Settings")]
        public GameObject harudesuyoPrefab;

        private float _cooldown;

        protected override void Awake()
        {
            base.Awake();
            
            _cooldown = Attributes[HarudesuyoAttributeType.Cooldown].CurrentValue;
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void UseWeapon()
        {
            base.UseWeapon();
            
        }
        
        private void ActivateSkill()
        {
            float radius = Attributes[HarudesuyoAttributeType.Radius].CurrentValue;
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, radius);

            if (enemies.Length == 0)
            {
                return;
            }
            
            float targetCount = Attributes[HarudesuyoAttributeType.TargetCount].CurrentValue;
            StartCoroutine(SpawnBombs(enemies.Take((int)targetCount).ToArray()));
        }

        IEnumerator SpawnBombs(Collider2D[] targets)
        {
            return null;
        }
    }
}
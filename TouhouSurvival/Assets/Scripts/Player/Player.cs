using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Unchord
{
    public class Player : Pawn
    {
        private Vector2 _movementVector;
        public PlayerAttributeSet AttributeSet { get; private set; }
        public PlayerLevelSystem LevelSystem { get; private set; }
        
        private AbilitySelectUIHandler _abilitySelectUI;
        private AbilityManager _abilityManager;

        public Transform WeaponTransform  { get; private set; }
        public Transform PassiveTransform { get; private  set; }
        public Transform SpellTransform { get; private  set; }

        protected override void Awake()
        {
            base.Awake();
            
            AttributeSet = gameObject.GetComponent<PlayerAttributeSet>();
            LevelSystem = new PlayerLevelSystem();
            _abilitySelectUI = new AbilitySelectUIHandler();
            _abilityManager = GetComponent<AbilityManager>();
            
            WeaponTransform = transform.Find($"Abilities/Weapons");
            PassiveTransform = transform.Find($"Abilities/Passives");
            SpellTransform = transform.Find($"Abilities/Spells");
        }

        protected override void Start()
        {
            base.Start();
            
            AttributeSet.Initialize(LevelSystem);
            LevelSystem.Initialize(AttributeSet);
            LevelSystem.OnLevelUp += (sender, args) =>
            {
                GameManager.Instance.BlockingEvent.Publish(
                    _abilitySelectUI.WaitForSelection(_abilityManager.SampleAbilities(3))
                );
            };
        }

        protected override void Update()
        {
            base.Update();

            Animator.SetFloat("Health", AttributeSet[PlayerAttributeType.Health].CurrentValue);
            Animator.SetBool("IsMove", _movementVector.magnitude > 0.0f);
        }

        private void FixedUpdate()
        {
            float movementSpeed = AttributeSet[PlayerAttributeType.MovementSpeed].CurrentValue;
            Vector2 next = _movementVector * (movementSpeed * Time.fixedDeltaTime);
            Rigidbody.MovePosition(Rigidbody.position + next);
        }

        protected override void LateUpdate()
        {
            Animator.SetFloat("Speed", _movementVector.magnitude);
            if (_movementVector.x != 0)
            {
                float angle = _movementVector.x < 0 ? 0.0f : 180.0f;
                Renderers.eulerAngles = Vector3.up * angle;
                Colliders.eulerAngles = Vector3.up * angle;
            }
        }
        
        private void OnMove(InputValue value)
        {
            // Input Setting에서 이미 값을 Normalized된 상태로 받도록 세팅됨
            Debug.Log("OnMove");
            _movementVector = value.Get<Vector2>();
        }

        public GameObject GetNearestEnemyOrNull()
        {
            Vector2 originPosition = transform.position;
            GameObject selected = null;
            List<GameObject> spawnedEnemies = GameManager.Instance.SpawnedEnemies;

            for (int i = spawnedEnemies.Count - 1; i >= 0; --i)
            {
                if (!spawnedEnemies[i])
                {
                    spawnedEnemies.RemoveAt(i);
                    continue;
                }

                if (!selected)
                {
                    selected = spawnedEnemies[i];
                    continue;
                }

                Vector2 diffTarget = (Vector2)spawnedEnemies[i].transform.position - originPosition;
                Vector2 diffSelected = (Vector2)selected.transform.position - originPosition;

                if (diffTarget.sqrMagnitude < diffSelected.sqrMagnitude)
                {
                    selected = spawnedEnemies[i];
                }
            }

            return selected;
        }

        public override float TakeDamage(float damageAmount, Pawn eventInstigator, GameObject damageCauser)
        {
            if (!AttributeSet)
            {
                Debug.Assert(false, "Player has no attribute set");
                return 0f;
            }

            GameplayAttribute health = AttributeSet[PlayerAttributeType.Health];
            GameplayAttribute defense = AttributeSet[PlayerAttributeType.Defense];
            
            float currentHealth = health.CurrentValue;
            float currentDefense = defense.CurrentValue;
            damageAmount -= currentDefense;
            
            // TODO: 런타임에 값이 바뀔 수 있는 Attribute 변수는 멤버 변수로 둡니다.
            //health.CurrentValue -= damageAmount;
            float newHealth = health.CurrentValue;
            
            Debug.Log($"플레이어가 {damageAmount} 피해를 입었습니다. 체력: {currentHealth} -> {newHealth}");
            return damageAmount;
        }
    }
}

using System;
using System.Collections;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Unchord
{
    public class Enemy : Pawn
    {        
        private Rigidbody2D _target;
        private readonly WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();

        private DropTable _dropTable;
        private int _playerLayer;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void InitializeFromFile(MultiCSVReader reader)
        {
            this.AttributeBase = new AttributeBaseSet(reader);
            this._dropTable = new DropTable(reader);
        }

        protected override void Start()
        {
            base.Start();
            
            AttributeBase[EnemyAttributeType.Health].onAttributeChanged += this.OnHealthChanged;
            _currentHealth = AttributeBase[EnemyAttributeType.Health].CurrentValue;
            _playerLayer = LayerMask.NameToLayer("Player");
        }

        private void OnHealthChanged(object sender, AttributeChangedEventArgs args)
        {
            //Debug.Log($"Health changed from {e.OldValue} to {e.NewValue}");
        }

        protected override void Update()
        {
            if (!_target)
            {
                _target = GameManager.Instance.Player.Rigidbody;
            }
            
            if (!gameObject.activeSelf)
            {
                return;
            }

            base.Update();
        }

        private void FixedUpdate()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            float speed = AttributeBase[EnemyAttributeType.Speed].CurrentValue;
            Vector2 toTargetDirection = (_target.position - Rigidbody.position).normalized;
            Vector2 nextPosition = toTargetDirection * (speed * Time.fixedDeltaTime);
            Rigidbody.MovePosition(Rigidbody.position + nextPosition);
            Rigidbody.linearVelocity = Vector2.zero;
        }

        protected override void LateUpdate()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }
            
            float angle = _target.position.x < Rigidbody.position.x ? 0f : 180f;
            Renderers.eulerAngles = Vector3.up * angle;
            Colliders.eulerAngles = Vector3.up * angle;
        }

        public override float TakeDamage(float damageAmount)
        {
            base.TakeDamage(damageAmount);

            // TODO: 이벤트 변수로 빼는 방안을 고려함.
            OnHealthChanged(this, null);
            return damageAmount;
        }

        public override float TakeTrueDamage(float damageAmount)
        {
            GameplayAttribute maxHealth = AttributeBase[EnemyAttributeType.Health];

            float currentHealth = _currentHealth;
            _currentHealth = Mathf.Clamp(_currentHealth - damageAmount, 0.0f, maxHealth.CurrentValue);

            // TODO: 이벤트 변수로 빼는 방안을 고려함.
            OnHealthChanged(this, null);

            Debug.Log($"적이 {damageAmount} 고정 피해를 입었습니다. 체력: {currentHealth} -> {_currentHealth}");
            return damageAmount;
        }

        public void KnockBack(float knockBackStrength)
        {
            StartCoroutine(KnockBackCoroutine(knockBackStrength));
        }
        
        private void OnEnable()
        {
            // TODO: Target Set
            Rigidbody.simulated = true;
            // Renderer.sortingOrder++;
            Animator.SetBool("Dead", false);
            _target = GameManager.Instance.Player.Rigidbody;
        }

        private void OnDisable()
        {

        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if(collision.gameObject.layer != _playerLayer)
            {
                return;
            }    

            if(collision.gameObject.TryGetComponent<Player>(out Player player))
            {
                float damage = AttributeBase[EnemyAttributeType.Attack].CurrentValue;
                player.TakeDamage(damage);
            }
        }

        private IEnumerator KnockBackCoroutine(float knockBackStrength)
        {
            yield return _waitForFixedUpdate; // 1 frame 대기

            // TODO: 플레이어 반대 방향으로 넉백
            Vector3 direction = transform.position - _target.transform.position;
            Rigidbody.AddForce(direction.normalized * (3 * knockBackStrength), ForceMode2D.Impulse);
        }

        public override void Die()
        {
            Rigidbody.simulated = false;
            // Renderer.sortingOrder--;
            Animator.SetBool("Dead", true);

            float dropRate = AttributeBase[EnemyAttributeType.DropRate].CurrentValue;
            if (Random.value >= dropRate)
            {
                _dropTable.Generate(transform.position, 1.0f, 1.0f);
            }
        }

        protected override void OnHit()
        {
            //Animator.SetTrigger("Hit");
        }

        // 적 사망 애니메이션 종료 시 이벤트
        private void OnDeadAnimationEnd()
        {
            //gameObject.SetActive(false);
            Destroy(this.gameObject);
            UIManager.Instance.GameCanvas.SetKillCount(++GameManager.Instance.KillCount);
        }
    }
}

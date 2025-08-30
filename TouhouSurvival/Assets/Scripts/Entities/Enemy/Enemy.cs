using System;
using System.Collections;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Unchord
{
    public class Enemy : Pawn
    {
        // TODO: 추후 Attributes 속성을 Pawn에 배치해야 합니다. (AbilityComponent에 선언한 Attributes 속성과 같은 형태로 코드를 작성할 수 있도록 합니다.)
        public AttributeBaseSet AttributeBase { get; private set; }
        
        [Tooltip("Root path is UnityEngine.Application.streamingAssetsPath. Relative path must be start with slash(/) character.")]
        public string dataFilePathRelative;

        [SerializeField] private Rigidbody2D target;
        private readonly WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();
        [SerializeField] private GameObject dropExperiencePrefab;

        private float _currentHealth;

        protected override void Awake()
        {
            base.Awake();

            FileStream fs = new FileStream(Application.streamingAssetsPath + this.dataFilePathRelative, FileMode.Open, FileAccess.Read, FileShare.Read);
            MultiCSVReader rd = new MultiCSVReader(fs);

            this.AttributeBase = new AttributeBaseSet(rd);
            
            rd.Close();
            fs.Close();
        }

        protected override void Start()
        {
            base.Start();

            AttributeBase[EnemyAttributeType.Health].onAttributeChanged += this.OnHealthChanged;

            _currentHealth = AttributeBase[EnemyAttributeType.Health].CurrentValue;
        }

        private void OnHealthChanged(object sender, AttributeChangedEventArgs args)
        {
            //Debug.Log($"Health changed from {e.OldValue} to {e.NewValue}");
        }

        protected override void Update()
        {
            if (!target)
            {
                target = GameManager.Instance.Player.Rigidbody;
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
            Vector2 toTargetDirection = (target.position - Rigidbody.position).normalized;
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
            
            float angle = target.position.x < Rigidbody.position.x ? 0f : 180f;
            Renderers.eulerAngles = Vector3.up * angle;
            Colliders.eulerAngles = Vector3.up * angle;
        }

        private void OnHealthChange(object sender, EventArgs args)
        {
            // NOTE: this block intentionally no operation.
        }

        public override float TakeDamage(float damageAmount)
        {
            if (AttributeBase == null)
            {
                Debug.Assert(false, "Enemy has no attribute set");
                return 0f;
            }

            GameplayAttribute maxHealth = AttributeBase[EnemyAttributeType.Health];

            float currentHealth = _currentHealth;

            _currentHealth = Mathf.Clamp(_currentHealth - damageAmount, 0.0f, maxHealth.CurrentValue);

            float newHealth = _currentHealth;

            // TODO: 이벤트 변수로 빼는 방안을 고려함.
            OnHealthChanged(this, null);

            if (newHealth <= 0.0f)
            {
                Die();
            }
            else
            {
                OnHit();
            }
            
            Debug.Log($"적이 {damageAmount} 피해를 입었습니다. 체력: {currentHealth} -> {newHealth}");
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
            target = GameManager.Instance.Player.Rigidbody;
        }

        private void OnDisable()
        {

        }

        private IEnumerator KnockBackCoroutine(float knockBackStrength)
        {
            yield return _waitForFixedUpdate; // 1 frame 대기

            // TODO: 플레이어 반대 방향으로 넉백
            Vector3 direction = transform.position - target.transform.position;
            Rigidbody.AddForce(direction.normalized * (3 * knockBackStrength), ForceMode2D.Impulse);
        }

        private void OnHit()
        {
            //Animator.SetTrigger("Hit");
        }

        public override void Die()
        {
            Rigidbody.simulated = false;
            // Renderer.sortingOrder--;
            Animator.SetBool("Dead", true);

            DropExperienceObject();
        }

        // 적 사망 애니메이션 종료 시 이벤트
        private void OnDeadAnimationEnd()
        {
            //gameObject.SetActive(false);
            Destroy(this.gameObject);
            UIManager.Instance.GameCanvas.SetKillCount(++GameManager.Instance.KillCount);
        }

        private void DropExperienceObject()
        {
            if (!dropExperiencePrefab)
            {
                Debug.LogAssertion("DropExperiencePrefab is null");
                return;
            }

            // TODO: 드랍 확률 적용 & 해당 맵 섹션(청크)에 정보를 넘겨줘야 함
            float dropRate = AttributeBase[EnemyAttributeType.DropRate].CurrentValue;
            if (Random.value >= dropRate)    // [0.0f, 1.0f] 사이 랜덤값이 dropRate(0.0 ~ 1.0) 사이보다 크거나 같으면 Drop
            {
                GameObject experience = Instantiate(dropExperiencePrefab, transform.position, Quaternion.identity);
                experience.transform.SetParent(GameManager.Instance.RuntimeContainer, true);
            }
        }
    }
}

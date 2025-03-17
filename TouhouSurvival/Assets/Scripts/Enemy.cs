using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Unchord
{
    public class Enemy : Pawn
    {
        [SerializeField] private Rigidbody2D target;
        private EnemyAttributeSet _attributeSet;
        private readonly WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();
        [SerializeField] private GameObject dropExperiencePrefab;

        protected override void Awake()
        {
            base.Awake();
            _attributeSet = gameObject.GetComponent<EnemyAttributeSet>();
        }

        protected override void Start()
        {
            base.Start();
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

            float speed = _attributeSet[EnemyAttributeType.Speed].CurrentValue;
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

        public override float TakeDamage(float damageAmount, Pawn eventInstigator, GameObject damageCauser)
        {
            if (!_attributeSet)
            {
                Debug.Assert(false, "Enemy has no attribute set");
                return 0f;
            }

            GameplayAttribute healthAttribute = _attributeSet[EnemyAttributeType.Health];
            float currentHealth = healthAttribute.CurrentValue;
            healthAttribute.CurrentValue -= damageAmount;
            float newHealth = healthAttribute.CurrentValue;
            
            Debug.Log($"적이 {damageAmount} 피해를 입었습니다. 체력: {currentHealth} -> {newHealth}");
            return damageAmount;
        }
        
        private void OnEnable()
        {
            // TODO: Target Set
            Rigidbody.simulated = true;
            // Renderer.sortingOrder++;
            Animator.SetBool("Dead", false);
            _attributeSet.ResetAttributes();
            target = GameManager.Instance.Player.Rigidbody;
        }

        private void OnDisable()
        {

        }

        private IEnumerator KnockBack(float knockBackStrength)
        {
            yield return _waitForFixedUpdate; // 1 frame 대기

            // TODO: 플레이어 반대 방향으로 넉백
            Vector3 direction = transform.position - target.transform.position;
            Rigidbody.AddForce(direction.normalized * (3 * knockBackStrength), ForceMode2D.Impulse);
        }

        public void OnHit(float knockBackStrength)
        {
            Animator.SetTrigger("Hit");
            StartCoroutine(KnockBack(knockBackStrength));
        }

        public void OnDead()
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
            float dropRate = _attributeSet[EnemyAttributeType.DropRate].CurrentValue;
            if (Random.value >= dropRate)    // [0.0f, 1.0f] 사이 랜덤값이 dropRate(0.0 ~ 1.0) 사이보다 크거나 같으면 Drop
            {
                GameObject experience = Instantiate(dropExperiencePrefab, transform.position, Quaternion.identity);
                experience.transform.SetParent(GameManager.Instance.RuntimeContainer, true);
            }
        }
    }
}

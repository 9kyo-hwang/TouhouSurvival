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
            Random.InitState((int)DateTime.Now.Ticks);
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
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

            Vector2 toTargetDirection = (target.position - Rigidbody.position).normalized;
            Vector2 nextPosition = toTargetDirection *
                                   (_attributeSet.GetAttributeValue(EnemyAttributeType.Health.ToString()) *
                                    Time.fixedDeltaTime);
            Rigidbody.MovePosition(Rigidbody.position + nextPosition);
            Rigidbody.linearVelocity = Vector2.zero;
        }

        private void LateUpdate()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            Renderer.flipX = target.position.x < Rigidbody.position.x;
        }

        public override float TakeDamage(float damageAmount, Pawn eventInstigator, GameObject damageCauser)
        {
            if (!_attributeSet)
            {
                Debug.Assert(false, "Enemy has no attribute set");
                return 0f;
            }

            float currentHealth = _attributeSet.GetAttributeValue(EnemyAttributeType.Health.ToString());
            _attributeSet.ModifyAttributeValue(EnemyAttributeType.Health.ToString(), -damageAmount);
            float newHealth = _attributeSet.GetAttributeValue(EnemyAttributeType.Health.ToString());

            if (newHealth > 0)
            {
                Animator.SetTrigger("Hit");
            }
            else
            {
                OnDead();
            }

            Debug.Log($"적이 {damageAmount} 피해를 입었습니다. 체력: {currentHealth} -> {newHealth}");
            return damageAmount;
        }

        private void OnEnable()
        {
            // TODO: Target Set
            Rigidbody.simulated = true;
            Collider.enabled = true;
            Renderer.sortingOrder++;
            Animator.SetBool("Dead", false);
            _attributeSet.ResetAttributes();
        }

        private void OnDisable()
        {

        }

        private IEnumerator KnockBack()
        {
            yield return _waitForFixedUpdate; // 1 frame 대기

            // TODO: 플레이어 반대 방향으로 넉백
            Vector3 direction = transform.position - target.transform.position;
            Rigidbody.AddForce(direction.normalized * 3, ForceMode2D.Impulse);
        }

        private void OnDead()
        {
            Rigidbody.simulated = false;
            Collider.enabled = false;
            Renderer.sortingOrder--;
            Animator.SetBool("Dead", true);

            DropExperienceObject();
        }

        // 적 사망 애니메이션 종료 시 이벤트
        private void OnDeadAnimationEnd()
        {
            gameObject.SetActive(false);
        }

        private void DropExperienceObject()
        {
            if (!dropExperiencePrefab)
            {
                Debug.LogAssertion("DropExperiencePrefab is null");
                return;
            }

            // TODO: 드랍 확률 적용 & 해당 맵 섹션(청크)에 정보를 넘겨줘야 함
            float dropRate = _attributeSet.GetAttributeValue(EnemyAttributeType.DropRate.ToString());
            if (Random.value > dropRate)
            {
                GameObject experience = Instantiate(dropExperiencePrefab, transform.position, Quaternion.identity);
            }
        }
    }
}

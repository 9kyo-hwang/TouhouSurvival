using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Pool;

public class Enemy : Pawn
{
    [SerializeField] private Rigidbody2D target;
    [SerializeField] private float speed;
    //[SerializeField] private float health;
    //[SerializeField] private float maxHealth;
    [SerializeField] private RuntimeAnimatorController[] animatorControllers;

    private bool _isDead;
    private WaitForFixedUpdate _wait;
    private EnemyStatComponent _stat;

    public void Initialize(SpawnData spawnData)
    {
        Animator.runtimeAnimatorController = animatorControllers[spawnData.enemyType];
        // speed = spawnData.speed;
        // maxHealth = spawnData.health;
        // health = spawnData.health;
    }

    protected override void Awake()
    {
        base.Awake();
        _wait = new WaitForFixedUpdate();
        _stat = GetComponent<EnemyStatComponent>();
    }

    private void FixedUpdate()
    {
        if (_isDead || IsHitAnimationPlaying())
        {
            return;
        }
        
        Vector2 direction = (target.position - Rigidbody.position).normalized;
        Vector2 toTargetVector = direction * (_stat.Speed * Time.fixedDeltaTime);
        Rigidbody.MovePosition(Rigidbody.position + toTargetVector);
        Rigidbody.linearVelocity = Vector2.zero;
    }

    private bool IsHitAnimationPlaying()  // like hit delay
    {
        return Animator.GetCurrentAnimatorStateInfo(0).IsName("Hit");
    }

    private void LateUpdate()
    {
        if (_isDead)
        {
            return;
        }
        
        Renderer.flipX = target.position.x < Rigidbody.position.x;
    }

    public override float TakeDamage(float damageAmount, Pawn eventInstigator, GameObject damageCauser)
    {
        base.TakeDamage(damageAmount, eventInstigator, damageCauser);
        _stat.ApplyDamage(damageAmount);
        return damageAmount;
    }

    private void OnEnable()
    {
        target = GameManager.Instance.player.GetComponent<Rigidbody2D>();
        _isDead = false;
        Rigidbody.simulated = true;
        Collider.enabled = true;
        Renderer.sortingOrder++;
        Animator.SetBool("Dead", false);
        
        // TODO: Stat Data Initialize(health = maxHealth, ...)
        _stat.Initialize();
    }

    private void OnDisable()
    {

    }

    // 기존에는 피격자 입장에서 피격을 판정,
    // 바뀐 구조에서는 공격자가 공격 판정 후 데미지를 가하도록 변경.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet") && !_isDead)
        {
            OnHitBullet(other.GetComponent<Bullet>());
        }
    }

    private void OnHitBullet(Bullet bullet)
    {
        // health -= bullet.damage;
        // StartCoroutine(KnockBack());
        //
        // if (health > 0)
        // {
        //     Animator.SetTrigger("Hit");
        // }
        // else
        // {
        //     Dead();
        // }
    }
    
    IEnumerator KnockBack()
    {
        // Wait for Next Fixed Update Frame
        yield return _wait;
        
        // KnockBack Enemy to Player's Opposite Direction 
        Vector3 direction = transform.position - GameManager.Instance.player.transform.position;
        Rigidbody.AddForce(direction.normalized * 3, ForceMode2D.Impulse);
    }

    private void Dead()
    {
        target = null;
        _isDead = true;
        Rigidbody.simulated = false;
        Collider.enabled = false;
        Renderer.sortingOrder--;
        Animator.SetBool("Dead", true);
    }

    private void OnDeadAnimationEnd()  // Call by Dead Animation Event
    {
        gameObject.SetActive(false);
    }
}

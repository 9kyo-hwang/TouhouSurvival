using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Rigidbody2D target;
    [SerializeField] private float speed;
    [SerializeField] private float health;
    [SerializeField] private float maxHealth;
    [SerializeField] private RuntimeAnimatorController[] animatorControllers;
    private Rigidbody2D _rigidbody;
    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private bool _isDead;
    private WaitForFixedUpdate _wait;

    public void Initialize(SpawnData spawnData)
    {
        _animator.runtimeAnimatorController = animatorControllers[spawnData.enemyType];
        speed = spawnData.speed;
        maxHealth = spawnData.health;
        health = spawnData.health;
    }
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _wait = new WaitForFixedUpdate();
    }

    private void FixedUpdate()
    {
        if (_isDead || IsHitAnimationPlaying())
        {
            return;
        }
        
        Vector2 direction = (target.position - _rigidbody.position).normalized;
        Vector2 toTargetVector = direction * (speed * Time.fixedDeltaTime);
        _rigidbody.MovePosition(_rigidbody.position + toTargetVector);
        _rigidbody.linearVelocity = Vector2.zero;
    }

    private bool IsHitAnimationPlaying()  // like hit delay
    {
        return _animator.GetCurrentAnimatorStateInfo(0).IsName("Hit");
    }

    private void LateUpdate()
    {
        if (_isDead)
        {
            return;
        }
        
        _spriteRenderer.flipX = target.position.x < _rigidbody.position.x;
    }

    private void OnEnable()
    {
        target = GameManager.Instance.player.GetComponent<Rigidbody2D>();
        _isDead = false;
        _rigidbody.simulated = true;
        _collider.enabled = true;
        _spriteRenderer.sortingOrder++;
        _animator.SetBool("Dead", false);
        
        // TODO: Stat Data Initialize(health = maxHealth, ...)
        health = maxHealth;
    }

    private void OnDisable()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet") && !_isDead)
        {
            OnHitBullet(other.GetComponent<Bullet>());
        }
    }

    private void OnHitBullet(Bullet bullet)
    {
        health -= bullet.damage;
        StartCoroutine(KnockBack());
        
        if (health > 0)
        {
            _animator.SetTrigger("Hit");
        }
        else
        {
            Dead();
        }
    }
    
    IEnumerator KnockBack()
    {
        // Wait for Next Fixed Update Frame
        yield return _wait;
        
        // KnockBack Enemy to Player's Opposite Direction 
        Vector3 direction = transform.position - GameManager.Instance.player.transform.position;
        _rigidbody.AddForce(direction.normalized * 3, ForceMode2D.Impulse);
    }

    private void Dead()
    {
        target = null;
        _isDead = true;
        _rigidbody.simulated = false;
        _collider.enabled = false;
        _spriteRenderer.sortingOrder--;
        _animator.SetBool("Dead", true);
    }

    private void OnDeadAnimationEnd()  // Call by Dead Animation Event
    {
        gameObject.SetActive(false);
    }
}

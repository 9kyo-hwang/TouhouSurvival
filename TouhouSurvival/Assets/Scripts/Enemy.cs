using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour
{
    private ObjectPool<Enemy> _pool;
    public ObjectPool<Enemy> Pool { set => _pool = value; }

    [SerializeField] private Rigidbody2D target;
    [SerializeField] private float speed;
    
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        Vector2 direction = (target.position - _rigidbody.position).normalized;
        Vector2 toTargetVector = direction * (speed * Time.fixedDeltaTime);
        _rigidbody.MovePosition(_rigidbody.position + toTargetVector);
        _rigidbody.linearVelocity = Vector2.zero;
    }
    
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        _spriteRenderer.flipX = target.position.x < _rigidbody.position.x;
    }

    private void OnEnable()
    {
        target = GameManager.Instance.player.GetComponent<Rigidbody2D>();
        
        // TODO: Stat Data Initialize(health = maxHealth, ...)
    }

    private void OnDisable()
    {
        target = null;
        _pool.Release(this);
    }
}

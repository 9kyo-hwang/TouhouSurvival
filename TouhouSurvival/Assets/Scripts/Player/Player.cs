using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    private static readonly int Speed = Animator.StringToHash("Speed");
    
    public EnemyScanner Scanner { get; private set; }
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    
    [SerializeField] public Vector2 InputVector { get; private set; }
    [SerializeField] private float speed;

    private void Awake()
    {
        Scanner = GetComponent<EnemyScanner>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector2 toNextVector = InputVector * (speed * Time.fixedDeltaTime);
        _rigidbody.MovePosition(_rigidbody.position + toNextVector);
    }

    private void LateUpdate()
    {
        _animator.SetFloat(Speed, InputVector.magnitude);
        if (InputVector.x != 0)
        {
            _spriteRenderer.flipX = InputVector.x < 0;
        }
    }

    private void OnMove(InputValue value)
    {
        // Input Setting에서 이미 값을 Normalized된 상태로 받도록 세팅됨
        InputVector = value.Get<Vector2>();
    }
}

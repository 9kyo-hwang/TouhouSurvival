using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Pawn
{
    public EnemyScanner Scanner { get; private set; }
    [SerializeField] public Vector2 MovementVector { get; private set; }
    
    private PlayerStatComponent _stat;

    protected override void Awake()
    {
        base.Awake();
        Scanner = GetComponent<EnemyScanner>();
        _stat = GetComponent<PlayerStatComponent>();
    }

    protected override void Start()
    {
        
    }

    protected override void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector2 toNextVector = MovementVector * (_stat.Speed * Time.fixedDeltaTime);
        Rigidbody.MovePosition(Rigidbody.position + toNextVector);
        Debug.Log(_stat.Speed);
    }

    private void LateUpdate()
    {
        Animator.SetFloat("Speed", MovementVector.magnitude);
        if (MovementVector.x != 0)
        {
            Renderer.flipX = MovementVector.x > 0;  // TODO: 임시로 좌우 반전
        }
    }

    private void OnMove(InputValue value)
    {
        // Input Setting에서 이미 값을 Normalized된 상태로 받도록 세팅됨
        MovementVector = value.Get<Vector2>();
    }

    public override float TakeDamage(float damageAmount, Pawn eventInstigator, GameObject damageCauser)
    {
        base.TakeDamage(damageAmount, eventInstigator, damageCauser);
        _stat.ApplyDamage(damageAmount);
        return damageAmount;
    }
}

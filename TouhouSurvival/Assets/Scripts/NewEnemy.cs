using System;
using UnityEngine;

public class NewEnemy : Pawn
{
    [SerializeField] private Rigidbody2D target;
    private EnemyAttributeSet _attributeSet;
    private bool _isDead = false;

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
        base.Update();
    }

    private void FixedUpdate()
    {
        if (_isDead)
        {
            return;
        }
        
        Vector2 direction = (target.position - Rigidbody.position).normalized;
        Vector2 toTargetVector = direction * (_attributeSet.GetAttribute("Speed") * Time.fixedDeltaTime);
        Rigidbody.MovePosition(Rigidbody.position + toTargetVector);
        Rigidbody.linearVelocity = Vector2.zero;
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
        if (!_attributeSet)
        {
            Debug.Assert(false, "Enemy has no attribute set");
            return 0f;
        }
        
        float currentHealth = _attributeSet.GetAttribute("Health");
        _attributeSet.ModifyAttribute("Health", -damageAmount);
        float newHealth = _attributeSet.GetAttribute("Health");

        if (newHealth <= 0)
        {
            _isDead = true;
        }
        
        Debug.Log($"적이 {damageAmount} 피해를 입었습니다. 체력: {currentHealth} -> {newHealth}");
        return damageAmount;
    }

    private void OnEnable()
    {
        _isDead = false;
        Rigidbody.simulated = true;
        Collider.enabled = true;
        Renderer.sortingOrder++;
        Animator.SetBool("Dead", false);
        
        // TODO: Attribute Data Initialize
    }

    private void OnDisable()
    {
        
    }
}

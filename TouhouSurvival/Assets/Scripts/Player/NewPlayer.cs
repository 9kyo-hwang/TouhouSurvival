using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewPlayer : Pawn
{
    private Vector2 _movementVector;
    private PlayerAttributeSet _attributeSet;
    public List<GameObject> SpawnedObjects { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        
        _attributeSet = gameObject.GetComponent<PlayerAttributeSet>();
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
        Vector2 nextPosition = _movementVector * (_attributeSet.GetAttribute("Speed") *
                                                  Time.fixedDeltaTime);
        Rigidbody.MovePosition(Rigidbody.position + nextPosition);
    }

    private void LateUpdate()
    {
        Animator.SetFloat("Speed", _movementVector.magnitude);
        if (_movementVector.x != 0)
        {
            Renderer.flipX = _movementVector.x > 0;  // TODO: 임시로 좌우 반전
        };
    }
    
    private void OnMove(InputValue value)
    {
        // Input Setting에서 이미 값을 Normalized된 상태로 받도록 세팅됨
        _movementVector = value.Get<Vector2>();
    }

    public override float TakeDamage(float damageAmount, Pawn eventInstigator, GameObject damageCauser)
    {
        if (!_attributeSet)
        {
            Debug.Assert(false, "Player has no attribute set");
            return 0f;
        }
        
        float currentHealth = _attributeSet.GetAttribute("Health");
        _attributeSet.ModifyAttribute("Health", -damageAmount);
        float newHealth = _attributeSet.GetAttribute("Health");
        
        Debug.Log($"플레이어가 {damageAmount} 피해를 입었습니다. 체력: {currentHealth} -> {newHealth}");
        return damageAmount;
    }

    public GameObject GetNearestEnemyOrNull()
    {
        Vector3 originPosition = transform.position;
        GameObject selected = null;

        for (int i = SpawnedObjects.Count - 1; i >= 0; i--)
        {
            if (!SpawnedObjects[i])
            {
                SpawnedObjects.RemoveAt(i);
                continue;
            }

            if (!selected)
            {
                selected = SpawnedObjects[i];
                continue;
            }
            
            Vector2 diffTarget = SpawnedObjects[i].transform.position - originPosition;
            Vector2 diffSelected = selected.transform.position - originPosition;

            if (diffTarget.magnitude < diffSelected.magnitude)
            {
                selected = SpawnedObjects[i];
            }
        }
        
        return selected;
    }
}

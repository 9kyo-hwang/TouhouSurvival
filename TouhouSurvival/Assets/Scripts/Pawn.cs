using System;
using UnityEngine;


// 플레이어나 AI가 제어할 수 있는 모든 게임 오브젝트의 베이스 클래스
public class Pawn : MonoBehaviour
{
    [Header("Components")] 
    protected Rigidbody2D Rigidbody;
    protected Collider2D Collider;
    protected SpriteRenderer Renderer;
    protected Animator Animator;
    
    protected virtual void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Collider = GetComponent<Collider2D>();
        Renderer = GetComponent<SpriteRenderer>();
        Animator = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    // Apply damage to this game object. The amount of damage actually applied.
    // instigator: 공격한 폰 계열(플레이어 or 몬스터 등)
    // damageCauser: 데미지를 입힌 오브젝트(무기, 총알 등)
    public virtual float TakeDamage(float damageAmount, Pawn eventInstigator = null, GameObject damageCauser = null)
    {
        return damageAmount;
    }
}

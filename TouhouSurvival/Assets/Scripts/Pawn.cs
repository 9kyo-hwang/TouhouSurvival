using System;
using UnityEngine;


// 플레이어나 AI가 제어할 수 있는 모든 게임 오브젝트의 베이스 클래스
public abstract class Pawn : MonoBehaviour
{
    [Header("Components")] 
    public Rigidbody2D Rigidbody { get; protected set; }
    public Collider2D Collider { get; protected set; }
    public SpriteRenderer Renderer { get; protected set; }
    public Animator Animator { get; protected set; }
    
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
    public abstract float TakeDamage(float damageAmount, Pawn eventInstigator = null, GameObject damageCauser = null);
}

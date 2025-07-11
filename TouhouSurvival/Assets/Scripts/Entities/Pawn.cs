using System;
using UnityEngine;

namespace Unchord
{
    // 플레이어나 AI가 제어할 수 있는 모든 게임 오브젝트의 베이스 클래스
    public abstract class Pawn : MonoBehaviour
    {
        [Header("Components")]
        public Transform Colliders { get; protected set; }
        public Transform Renderers { get; protected set; }
        public Rigidbody2D Rigidbody { get; protected set; }
        public Animator Animator { get; protected set; }

        protected virtual void Awake()
        {
            Colliders = transform.Find("Colliders");
            Renderers = transform.Find("Renderers");
            Rigidbody = GetComponent<Rigidbody2D>();
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

        protected virtual void LateUpdate()
        {

        }

        // Apply damage to this game object. The amount of damage actually applied.
        public abstract float TakeDamage(float damageAmount);
        public abstract float TakeTrueDamage(float damageAmount);

        public virtual void Die()
        {

        }
    }
}
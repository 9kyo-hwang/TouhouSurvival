using System;
using System.IO;
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

        [Tooltip("Root path is UnityEngine.Application.streamingAssetsPath. Relative path must be start with slash(/) character.")]
        public string dataFilePathRelative;
        public AttributeBaseSet AttributeBase { get; protected set; }
        protected float _currentHealth;

        protected virtual void Awake()
        {
            Colliders = transform.Find("Colliders");
            Renderers = transform.Find("Renderers");
            Rigidbody = GetComponent<Rigidbody2D>();
            Animator = GetComponent<Animator>();

            if(!string.IsNullOrEmpty(dataFilePathRelative))
            {
                FileStream fs = new FileStream(Application.streamingAssetsPath + this.dataFilePathRelative, FileMode.Open, FileAccess.Read, FileShare.Read);
                MultiCSVReader rd = new MultiCSVReader(fs);

                InitializeFromFile(rd);

                rd.Close();
                fs.Close();
            }
        }

        protected abstract void InitializeFromFile(MultiCSVReader reader);

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
        public virtual float TakeDamage(float damageAmount)
        {
            if(AttributeBase == null)
            {
                Debug.Assert(false, $"{this.gameObject.name} has no attribute set");
                return 0f;
            }

            if(damageAmount <= 0f)
            {
                return 0f;
            }

            float oldHealth = _currentHealth;
            _currentHealth = Mathf.Max(_currentHealth - damageAmount, 0f);

            Debug.Log($"{this.gameObject.name}이(가) {damageAmount}의 피해를 입었습니다. 체력: {oldHealth} -> {_currentHealth}");

            if(_currentHealth <= 0f)
            {
                Die();
            }
            else
            {
                OnHit();
            }

            return damageAmount;
        }

        public abstract float TakeTrueDamage(float damageAmount);

        public virtual void Die()
        {

        }

        protected virtual void OnHit()
        {
            
        }
    }
}
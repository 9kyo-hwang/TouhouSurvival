using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Unchord
{
    public class BossBehaviour : MonoBehaviour
    {
        /**
         *  Animation State Hash Values
         */
        
        public static readonly int HashInPursuit = Animator.StringToHash("InPursuit");
        public static readonly int HashAttack = Animator.StringToHash("Attack");

        public static readonly int HashIdleState = Animator.StringToHash("Idle");
        
        public BossController Controller { get; protected set; }
        public Player Target { get; protected set; }
        
        public Vector3 OriginalPosition { get; protected set; }
        [NonSerialized] public float AttackDistance = 3;

        protected void OnEnable()
        {
            Controller = GetComponentInChildren<BossController>();
            OriginalPosition = transform.position;
            Controller.Animator.Play(HashInPursuit);
            Target = GameManager.Instance.Player;
            MonoStateMachineBehaviour<BossBehaviour>.Initialize(Controller.Animator, this);
        }
        
        #region Called by animation events
        #endregion

        protected void OnDisable()
        {
            if (Target != null)
            {
                Target = null;
            }
        }

        private void FixedUpdate()
        {
            
        }

        public void FindTarget()
        {
            Player target = GameManager.Instance.Player;
            if (Target == null)
            {
                if (target != null)
                {
                    Target = target;
                }
            }
        }

        public void StartPursuit()
        {
            Controller.Animator.SetBool(HashInPursuit, true);
        }

        public void StopPursuit()
        {
            Controller.Animator.SetBool(HashInPursuit, false);
        }

        public void TriggerAttack()
        {
            Controller.Animator.SetTrigger(HashAttack);
        }

        public void Death()
        {
            
        }

        public void ApplyDamage()
        {
            
        }
    }
}


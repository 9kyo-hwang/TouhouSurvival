using UnityEngine;
using UnityEngine.Animations;

namespace Unchord
{
    public abstract class SealedStateMachineBehaviour : StateMachineBehaviour
    {
        public sealed override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
        public sealed override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
        public sealed override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
    }
    
    public class MonoStateMachineBehaviour<T> : SealedStateMachineBehaviour where T : MonoBehaviour
    {
        protected T MonoBehaviour;
        private bool _firstFrameHappened;
        private bool _lastFrameHappened;

        public static void Initialize(Animator animator, T monoBehaviour)
        {
            MonoStateMachineBehaviour<T>[] behaviours = animator.GetBehaviours<MonoStateMachineBehaviour<T>>();
            foreach (MonoStateMachineBehaviour<T> behaviour in behaviours)
            {
                behaviour.InternalInitialize(animator, monoBehaviour);
            }
        }

        private void InternalInitialize(Animator animator, T monoBehaviour)
        {
            MonoBehaviour = monoBehaviour;
            OnStart(animator);
        }

        public sealed override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex,
            AnimatorControllerPlayable controller)
        {
            _firstFrameHappened = false;
            OnPreEnter(animator, stateInfo, layerIndex);
            OnPreEnter(animator, stateInfo, layerIndex, controller);
        }

        public sealed override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex,
            AnimatorControllerPlayable controller)
        {
            if (!animator.gameObject.activeSelf)
            {
                return;
            }

            if (animator.IsInTransition(layerIndex) &&
                animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash == stateInfo.fullPathHash)
            {
                OnEnter(animator, stateInfo, layerIndex);
                OnEnter(animator, stateInfo, layerIndex, controller);
            }

            if (!animator.IsInTransition(layerIndex) && _firstFrameHappened)
            {
                OnUpdate(animator, stateInfo, layerIndex);
                OnUpdate(animator, stateInfo, layerIndex, controller);
            }

            if (animator.IsInTransition(layerIndex) && !_lastFrameHappened && _firstFrameHappened)
            {
                _lastFrameHappened = true;
                
                OnPreExit(animator, stateInfo, layerIndex);
                OnPreExit(animator, stateInfo, layerIndex, controller);
            }

            if (!animator.IsInTransition(layerIndex) && !_firstFrameHappened)
            {
                _firstFrameHappened = true;
                
                OnPostEnter(animator, stateInfo, layerIndex);
                OnPostEnter(animator, stateInfo, layerIndex, controller);
            }

            if (animator.IsInTransition(layerIndex) 
                && animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash == stateInfo.fullPathHash)
            {
                OnExit(animator, stateInfo, layerIndex);
                OnExit(animator, stateInfo, layerIndex, controller);
            }
        }

        public sealed override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            _lastFrameHappened = false;
            
            OnPostExit(animator, stateInfo, layerIndex);
            OnPostExit(animator, stateInfo, layerIndex, controller);
        }

        // MonoBehaviour의 Start 함수에서 호출
        public virtual void OnStart(Animator animator) {}
        
        // 상태 실행이 처음 시작될 때 (상태로 전환될 때) 업데이트 전에 호출
        public virtual void OnPreEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {}
        public virtual void OnPreEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller) {}
        
        // 상태 전환 중일 때, OnPreEnter 이후 매 프레임마다 호출
        public virtual void OnEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {}
        public virtual void OnEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller) {}
        
        // 상태 전환이 완료된 첫 프레임에 호출
        public virtual void OnPostEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {}
        public virtual void OnPostEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller) {}
        
        // 상태가 전환되지 않고 있을 때 매 프레임마다 호출
        public virtual void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {}
        public virtual void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller) {}
        
        // 상태에서 벗어나는 전환이 시작된 첫 프레임에 호출(전환이 프레임보다 짧을 경우 호출되지 않을 수 있음).
        public virtual void OnPreExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {}
        public virtual void OnPreExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller) {}
        
        // 상태에서 벗어나는 동안 매 프레임마다 호출
        public virtual void OnExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {}
        public virtual void OnExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller) {}
        
        // OnExit 이후에 호출
        public virtual void OnPostExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {}
        public virtual void OnPostExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller) {}
    }
}
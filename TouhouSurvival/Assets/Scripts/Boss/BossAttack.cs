using UnityEngine;

namespace Unchord
{
    public class BossAttack : MonoStateMachineBehaviour<BossBehaviour>
    {
        protected Vector3 AttackPosition;

        public override void OnPreEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnPreEnter(animator, stateInfo, layerIndex);
            
            AttackPosition = MonoBehaviour.Target.transform.position;
            Vector3 toTarget = AttackPosition - MonoBehaviour.transform.position;
            
            // 3D 기반이라 정면 잡아주는 코드
            MonoBehaviour.transform.forward = toTarget.normalized;
            MonoBehaviour.Controller.SetForward(MonoBehaviour.transform.forward);
        }

        public override void OnPostExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnPostExit(animator, stateInfo, layerIndex);
        }
    }
}


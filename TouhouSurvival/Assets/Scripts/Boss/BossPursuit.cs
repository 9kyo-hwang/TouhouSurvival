using UnityEngine;

namespace Unchord
{
    public class BossPursuit : MonoStateMachineBehaviour<BossBehaviour>
    {
        public override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnUpdate(animator, stateInfo, layerIndex);

            MonoBehaviour.FindTarget();
            
            if (!MonoBehaviour.Target)
            {
                MonoBehaviour.StopPursuit();
            }
            else
            {
                Vector3 toTarget = MonoBehaviour.Target.transform.position - MonoBehaviour.transform.position;
                if (toTarget.sqrMagnitude < MonoBehaviour.AttackDistance * MonoBehaviour.AttackDistance)
                {
                    MonoBehaviour.TriggerAttack();
                }
                else
                {
                    MonoBehaviour.StopPursuit();
                }
            }
        }
    }
}


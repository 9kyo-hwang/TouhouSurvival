using UnityEngine;

namespace Unchord
{
    public class BossIdle : MonoStateMachineBehaviour<BossBehaviour>
    {
        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            
        }

        public override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnUpdate(animator, stateInfo, layerIndex);
            
            MonoBehaviour.FindTarget();
            if (MonoBehaviour.Target != null)
            {
                MonoBehaviour.StartPursuit();
            }
        }
    }
}
using UnityEngine;
public class DodgeBehaviour : StateMachineBehaviour
{
    // This script is useless for now...
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<PlayerCombatController>().IsDodging = true;
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<PlayerCombatController>().IsDodging = false;
    }
}

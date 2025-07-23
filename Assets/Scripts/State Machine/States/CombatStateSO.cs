using UnityEngine;
using Global.Hashes;

[CreateAssetMenu(fileName = "CombatState", menuName = "State Machine/State/Combat State")]
public class CombatStateSO : StateSO
{
    public override void OnEnter(StateMachineController controller)
    {
        base.OnEnter(controller);
        _controller._animator.SetTrigger(AnimationHashes.Equip);
        _controller._animator.SetBool(AnimationHashes.IsEquipped, false);
    }

    public override void OnExit()
    {
        base.OnExit();
        _controller._animator.SetTrigger(AnimationHashes.Equip);
        _controller._animator.SetBool(AnimationHashes.IsEquipped, true);
        _controller._animator.SetFloat(AnimationHashes.Equipped, 0f);
        _controller._animator.SetFloat(AnimationHashes.Vertical, 0);
        _controller._animator.SetFloat(AnimationHashes.Horizontal, 0f);
        _controller._animator.SetFloat(AnimationHashes.Motion, 0f);
    }

    public override void OnUpdate(StateMachineController controller)
    {
        // Debug.Log("Combat State.");
        StepBack();
    }

    private void StepBack()
    {
        if (_controller._animator.GetCurrentAnimatorStateInfo(0).IsTag("Motion"))
        {
            if (_controller.Combat.GetTargetDistance() < 3f)
            {
                // StepBack
                _controller.Locomotion.ForcedMove(-_controller.transform.root.forward, 1.5f);
                _controller._animator.SetFloat(AnimationHashes.Equipped, 1.0f, 0.1f, Time.deltaTime);
                _controller._animator.SetFloat(AnimationHashes.Vertical, -1, 0.23f, Time.deltaTime);
                _controller._animator.SetFloat(AnimationHashes.Horizontal, 0f, 0.1f, Time.deltaTime);
                _controller._animator.SetFloat(AnimationHashes.Motion, 1f, 0.1f, Time.deltaTime);

                if (_controller.Combat.GetTargetDistance() < 1f && !_controller._animator.GetCurrentAnimatorStateInfo(1).IsTag("Equip"))
                {
                    int rv = Random.Range(-1, 3);
                    if (rv > 1)
                        _controller._animator.SetTrigger("Dodge");
                    else
                        _controller._animator.SetTrigger("LAttack");
                }
            }
            else if (_controller.Combat.GetTargetDistance() > 3f)
            {
                // StepForward
                _controller.Locomotion.ForcedMove(_controller.transform.root.forward, 1.5f);
                _controller._animator.SetFloat(AnimationHashes.Equipped, 1.0f, 0.1f, Time.deltaTime);
                _controller._animator.SetFloat(AnimationHashes.Vertical, 1, 0.23f, Time.deltaTime);
                _controller._animator.SetFloat(AnimationHashes.Horizontal, 0f, 0.1f, Time.deltaTime);
                _controller._animator.SetFloat(AnimationHashes.Motion, 1f, 0.1f, Time.deltaTime);
            }
        }
    }
    
}


using UnityEngine;

[CreateAssetMenu(fileName = "IdleCondition", menuName = "State Machine/Condition/Idle Condition")]
public class IdleConditionSO : ConditionSO
{
    public override bool IsConditionMet()
    {
        return _controller.Combat.GetCurrentTarget() == null;
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "CombatCondition", menuName = "State Machine/Condition/Combat Condition")]
public class CombatConditionSO : ConditionSO
{
    public override bool IsConditionMet()
    {
        return _controller.Combat.GetCurrentTarget() != null;
    }
}

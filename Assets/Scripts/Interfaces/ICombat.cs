using UnityEngine;

public interface ICombat
{
    // Deal with attack logic, currently using animation trigger
    void OnAttack(string hitAnimationName);

    Transform GetCurrentTarget();

    float GetTargetDistance();
}
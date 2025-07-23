using UnityEngine;
using Global.Hashes;

[CreateAssetMenu(fileName = "IdleState", menuName = "State Machine/State/Idle State")]
public class IdleStateSO : StateSO
{

    public override void OnUpdate(StateMachineController controller)
    {
        // Debug.Log("Idle State.");

    }
}

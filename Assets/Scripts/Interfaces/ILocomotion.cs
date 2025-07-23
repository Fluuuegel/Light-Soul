using UnityEngine;

public interface ILocomotion
{
    void ForcedMove(Vector3 dir, float speed);

    void HandleDodge();
}
using UnityEngine;
using Global.Hashes;

public class EnemyLocomotionController : BaseCharacterLocomotion
{
    void Start()
    {

    }

    protected override void Update()
    {
        base.Update();
        _verticalDirection.Set(0f, _verticalSpeed, 0f);
        _characterController.Move(Time.deltaTime * _verticalDirection);
        HandleDodge();
    }

    public override void HandleDodge()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Dodge"))
        {
            float dodgeSpeed = _animator.GetFloat(AnimationHashes.AnimationMove) * 5.0f; // Later: Rewrite the logic into base class _dodgeMoveWeight;
            ForcedMove(-transform.forward, dodgeSpeed);
        }
    }
}

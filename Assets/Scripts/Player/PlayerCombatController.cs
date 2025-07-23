using UnityEngine;
using Global.Hashes;

public class PlayerCombatController : BaseCharacterCombat
{
    private bool _lock = false;
    private float _equipTimeout = 1.0f;
    private float _equipTimeoutDelta;
    private float _equipTransitionSpeed = 4.0f;
    private float _inputSmoothSpeed = 5.0f;
    private float _equippedValue = 0.0f;
    private float _targetEquipped = 0.0f;
    private float _lockValue = 0.0f;
    private float _targetLock = 0.0f;
    private GameObject _lockSignPanel;

    // TODO: We need put more effort on the player animator if we want to remove the flag
    public bool IsDodging = false;

    // For now, press 1 to equip sword
    protected void Start()
    {
        _equipTimeoutDelta = 0.0f;
    }

    void Update()
    {
        HandleEquip(1);
        HandleAttack();
        if (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            // TODO: improve it...
            _baseCharacterLocomotion.ForcedMove(transform.forward, _animator.GetFloat(AnimationHashes.AnimationMove) * _attackMoveWeight);
        }


        // Handle auto lock
        UpdateCurrentTarget();
    }

    void LateUpdate()
    {
        HandleLock();
        HandleAutoLock();
    }

    private void HandleEquip(int slotNum)
    {
        if (_equipTimeoutDelta >= 0.0f)
        {
            _equipTimeoutDelta -= Time.deltaTime;
        }

        if (_inputManager.Equip1 && _equipTimeoutDelta <= 0.0f)
        {
            _isEquipped = !_isEquipped;
            _animator.SetBool(AnimationHashes.IsEquipped, _isEquipped);
            _animator.SetTrigger(AnimationHashes.Equip);
            _targetEquipped = _isEquipped ? 1.0f : 0.0f;
            _equipTimeoutDelta = _equipTimeout;
        }

        if (Mathf.Abs(_equippedValue - _targetEquipped) > 0.001f)
        {
            _equippedValue = Mathf.MoveTowards(_equippedValue, _targetEquipped, Time.deltaTime * _equipTransitionSpeed);
            _animator.SetFloat(AnimationHashes.Equipped, _equippedValue);
        }
    }

    private void HandleAttack()
    {
        if (_inputManager.HAttackBuffer.IsTriggered())
        {
            _animator.SetTrigger(AnimationHashes.HAttack);
            _inputManager.HAttackBuffer.ResetTrigger();
            _inputManager.LAttackBuffer.ResetTrigger();
        }
        else if (_inputManager.LAttackBuffer.IsTriggered())
        {
            _animator.SetTrigger(AnimationHashes.LAttack);
            _inputManager.LAttackBuffer.ResetTrigger();
        }
    }

    private void HandleLock()
    {
        if (_inputManager.Lock)
        {
            if (!_lock)
            {
                int targetCount = DetectTarget(_detectedTargets, _detectCenter.position, _detectRadius, _enemyLayer);
                if (targetCount > 0)
                {
                    // Has target
                    SetCurrentTarget(_detectedTargets[0].transform);
                    // Set lock sign
                    _lockSignPanel = UIHelper.Instance.FindChildRecur(_detectedTargets[0].gameObject, "LockSignPanel");
                    Debug.Log(_detectedTargets[0].gameObject.name);
                    if (_lockSignPanel != null)
                    {
                        _lockSignPanel.SetActive(true);
                    }

                    _lock = !_lock;
                    _targetLock = 1.0f;
                }
            }
            else
            {
                _lockSignPanel.SetActive(false);
                _lockSignPanel = null;
                _currentTarget = null;
                _lock = !_lock;
                _targetLock = 0.0f;

                _animator.SetFloat(AnimationHashes.Horizontal, 0.0f);
                _animator.SetFloat(AnimationHashes.Vertical, 0.0f);
            }
        }

        if (_lock)
        {
            transform.root.rotation = LockOnTarget(transform.root.transform, _currentTarget, 50f);

            float targetH = _inputManager.Move.x;
            float targetV = _inputManager.Move.y;
            
            _animator.SetFloat(
                AnimationHashes.Horizontal,
                targetH,
                1f / _inputSmoothSpeed,
                Time.deltaTime);
            _animator.SetFloat(
                AnimationHashes.Vertical,
                targetV,
                1f / _inputSmoothSpeed,
                Time.deltaTime);
        }

        if (Mathf.Abs(_lockValue - _targetLock) > 0.0f)
        {
            _lockValue = Mathf.MoveTowards(_lockValue, _targetLock, Time.deltaTime * 4.0f);
            _animator.SetFloat(AnimationHashes.Lock, _lockValue);
        }
    }

    private void HandleAutoLock()
    {
        if (_lock) return;

        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);
        if (info.IsTag("Attack") && info.normalizedTime < 0.75f)
        {
            transform.root.rotation = LockOnTarget(transform.root.transform, _currentTarget, 50f);
        }
    }
    
    private void UpdateCurrentTarget()
    {
        if (_lock) return;
        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);
        if (info.IsTag("Motion") || info.IsTag("Attack"))
        {
            if (_inputManager.Move.sqrMagnitude > 0)
            {
                _currentTarget = null;
            }
            else
            {
                int targetCount = DetectTarget(_autoLockTargets, _autoLockCenter.position, _autoLockRadius, _enemyLayer);
                if (targetCount > 0)
                {
                    SetCurrentTarget(_autoLockTargets[0].transform);
                }
            }
        }
    }

    public void OnEquip(AnimationEvent animationEvent)
    {
        if (Weapon1 != null)
        {
            if (_isEquipped)
            {
                Weapon1.SetActive(true);
            }
            else
            {
                Weapon1.SetActive(false);
            }
        }
    }
    
    public bool GetLockStatus()
    {
        return _lock;
    }

}

using UnityEditor.EditorTools;
using UnityEngine;
using Global.Hashes;
using Unity.VisualScripting;

public class PlayerLocomotionController : BaseCharacterLocomotion
{

    private Transform _cameraTransform;
    private ThirdPersonCameraController _cameraController;
    private PlayerCombatController _playerCombatController;

    [Header("Camera Pivots")]
    [SerializeField, Tooltip("Pivot when standing")]
    private Transform _standCameraPivot;
    [SerializeField, Tooltip("Pivot when crouching")]
    private Transform _crouchCameraPivot;

    [Header("Rotation Settings")]
    [SerializeField, Tooltip("Time to smooth rotation")]
    private float _rotationSmoothTime = 0.1f;
    private float _rotationSpeed;
    [SerializeField, Tooltip("Time to smooth movement direction")]
    private float _movementSmoothTime;

    [Header("Movement Speeds")]
    [SerializeField, Tooltip("Walking speed")]
    private float _walkSpeed = 4f;
    [SerializeField, Tooltip("Sprint speed")]
    private float _sprintSpeed = 6f;
    [SerializeField, Tooltip("Crouch walking speed")]
    private float _crouchWalkSpeed = 1f;
    [SerializeField, Tooltip("Speed when attacking")]
    private float _attackMotionSpeed = 1f;
    [SerializeField, Tooltip("Crouch sprint speed")]
    private float _crouchSprintSpeed = 2f;
    [SerializeField, Tooltip("Dodge movement weight")]
    private float _dodgeMoveWeight = 5.0f;

    [Header("Jump Settings")]
    [SerializeField, Tooltip("The height the player can jump")]
    private float _jumpHeight = 1.2f;
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    private float _jumpTimeout = 0.50f;
    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    private float _fallTimeout = 0.35f;
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;
    private float _airMovementPenalty = 0.5f;

    [Header("Crouch Settings")]
    [SerializeField, Tooltip("Collider height when crouched")]
    private float _crouchHeight = 1f;
    [SerializeField, Tooltip("Collider center when crouched")]
    private Vector3 _crouchCenter = new Vector3(0, 0.5f, 0);
    [SerializeField, Tooltip("Collider height when standing")]
    private float _standHeight = 2f;
    [SerializeField, Tooltip("Collider center when standing")]
    private Vector3 _standCenter = new Vector3(0, 1f, 0);
    [SerializeField, Tooltip("Position to check for crouch clearance")]
    private Transform _crouchCheckPoint;
    [SerializeField, Tooltip("Layers considered for crouch clearance")]
    private LayerMask _crouchCheckLayer;

    private bool _isCrouching = false;

    protected override void Awake()
    {
        base.Awake();
        _cameraTransform = Camera.main.transform.root;
        _cameraController = _cameraTransform.GetComponent<ThirdPersonCameraController>();
        _playerCombatController = GetComponent<PlayerCombatController>();
    }

    protected void Start()
    {
        _horizontalSpeed = _walkSpeed;
        _jumpTimeoutDelta = 0.0f;
        _fallTimeoutDelta = _fallTimeout;
    }
    protected override void Update()
    {
        HandleAirState();
        GroundedCheck();
        HandleMotion();
        HandleDodge();
    }

    private void LateUpdate()
    {
        HandleCrouch();
    }

    private void HandleMotion()
    {
        if (_inputManager.Move == Vector2.zero)
        {
            _horizontalDirection = Vector3.zero;
        }

        if (CanMove())
        {
            if (_inputManager.Move != Vector2.zero)
            {
                float targetAngle = Mathf.Atan2(_inputManager.Move.x, _inputManager.Move.y) * Mathf.Rad2Deg
                    + _cameraTransform.localEulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _rotationSpeed, _rotationSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 direction = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                // If on slop, _horizontalDirection gets a y sub-value
                _horizontalDirection = Vector3.Slerp(_horizontalDirection, GetOnSlopeDirection(direction.normalized), _movementSmoothTime * Time.deltaTime);
            }
        }
        else
        {
            _horizontalDirection = Vector3.zero;
        }

        bool isSprinting = _inputManager.Sprint && CanSprint();
        if (!_isCrouching)
        {
            _horizontalSpeed = isSprinting ? _sprintSpeed : _walkSpeed;
        }
        else if (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            _horizontalSpeed = _attackMotionSpeed;
        }
        else
        {
            _horizontalSpeed = isSprinting ? _crouchSprintSpeed : _crouchWalkSpeed;
        }

        // Current velocity = horizontal velocity + vertical velocity
        _characterController.Move(Time.deltaTime * _horizontalSpeed * (_isGrounded ? 1f : _airMovementPenalty) * _horizontalDirection.normalized
                                 + Time.deltaTime * new Vector3(0f, _verticalSpeed, 0f));

        float motionValue = _horizontalDirection.magnitude * (_inputManager.Sprint ? 2f : 1f);
        _animator.SetFloat(AnimationHashes.Motion, motionValue, 0.1f, Time.deltaTime);
        _animator.SetFloat(AnimationHashes.Sprint, (_inputManager.Sprint && !_isCrouching) ? 1f : 0f);

    }

    protected override void HandleAirState()
    {
        if (_isGrounded)
        {
            _fallTimeoutDelta = _fallTimeout;
            _animator.SetFloat(AnimationHashes.FreeFall, 0.0f);

            if (_verticalSpeed < 0.0f)
            {
                _verticalSpeed = -2f;
            }

            if (_inputManager.Jump && _jumpTimeoutDelta <= 0.0f)
            {
                _verticalSpeed = Mathf.Sqrt(_jumpHeight * -2f * _characterGravity);
                _animator.SetTrigger(AnimationHashes.Jump);
            }

            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // Not grounded
            _jumpTimeoutDelta = _jumpTimeout;

            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                // Make sure the trigger will not be set twice
                // _fallTimeoutDelta = Mathf.Infinity;
                _animator.SetFloat(AnimationHashes.FreeFall, 1f);
            }
        }

        if (_verticalSpeed < _terminalSpeed)
        {
            _verticalSpeed += _characterGravity * Time.deltaTime;
        }
    }

    private void HandleCrouch()
    {
        if (!_inputManager.Crouch || _playerCombatController.GetLockStatus())
            return;

        if (_isCrouching && !IsUpperObstacle())
        {
            _isCrouching = false;
            _animator.SetTrigger(AnimationHashes.Crouch);
            SetCollider(_standHeight, _standCenter);
            _cameraController.SetFollowTarget(_standCameraPivot);
        }
        else if (!_isCrouching)
        {
            _isCrouching = true;
            _animator.SetTrigger(AnimationHashes.Crouch);
            SetCollider(_crouchHeight, _crouchCenter);
            _cameraController.SetFollowTarget(_crouchCameraPivot);
        }
    }

    public override void HandleDodge()
    {
        // TODO: Add cooldown
        if (_inputManager.Dodge)
        {
            _animator.SetTrigger(AnimationHashes.Dodge);
        }
        if (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Dodge"))
        {
            float dodgeSpeed = _animator.GetFloat(AnimationHashes.AnimationMove) * _dodgeMoveWeight;
            if (_playerCombatController.GetLockStatus())
            {
                Vector2 input = _inputManager.Move;
                Vector3 moveDir = (transform.forward * input.y + transform.right * input.x).normalized;

                if (moveDir.sqrMagnitude < 0.01f)
                    moveDir = transform.forward;

                //transform.root.rotation = Quaternion.LookRotation(moveDir);

                ForcedMove(moveDir, dodgeSpeed);
            }
            else
            {
                // Not lock
                ForcedMove(transform.forward, dodgeSpeed);
            }
        }
    }

    private bool CanMove()
    {
        AnimatorStateInfo state = _animator.GetCurrentAnimatorStateInfo(0);
        return state.IsTag("Motion");
    }

    private bool CanSprint()
    {
        float forwardDot = Vector3.Dot(_horizontalDirection.normalized, transform.forward);
        return forwardDot > 0.75f && CanMove();
    }

    private void SetCollider(float height, Vector3 center)
    {
        _characterController.height = height;
        _characterController.center = center;
    }

    private bool IsUpperObstacle()
    {
        return Physics.OverlapSphere(_crouchCheckPoint.position, 0.5f, _crouchCheckLayer).Length > 0;
    }
}

using UnityEngine;

/// <summary>
/// Third-person camera controller: handles input rotation, follow, collision, and optional lock-on.
/// </summary>
public class ThirdPersonCameraController : MonoBehaviour
{
    [Header("Target and Camera")]
    [SerializeField]
    private Transform _followTarget;             // The transform the camera follows (e.g., player)

    private Transform _cameraTransform;          // Main camera's transform
    private InputManager _inputManager;  // Input component from player

    [Header("Input Settings")]
    [SerializeField, Range(0.1f, 1.0f)]
    private float _mouseSensitivity = 0.5f;      // Mouse look sensitivity

    [Header("Rotation Settings")]
    [SerializeField, Range(0.01f, 0.5f)]
    private float _rotationSmoothTime = 0.12f;   // Smooth damping for rotation

    [Header("Follow Settings")]
    [SerializeField]
    private float _followDistance = 3f;         // Default distance behind target
    [SerializeField]
    private Vector3 _followOffset = Vector3.zero; // Offset from target position
    [SerializeField]
    private Vector2 _pitchLimits = new Vector2(-85f, 70f); // Min and max pitch angles
    [SerializeField]
    private float _followLerpTime = 0.1f;       // Smooth damping for position

    [Header("Lock-On Settings")]
    [SerializeField]
    private bool _enableLockOn = false;         // Toggle for lock-on mode
    [SerializeField]
    private Transform _lockOnTarget;            // Target to lock camera onto

    [Header("Collision Settings")]
    [SerializeField]
    private Vector2 _collisionDistanceLimits = new Vector2(0.1f, 3f); // Min/max camera distance
    [SerializeField]
    private float _collisionLerpTime = 0.1f;    // Smooth damping for collision adjustments
    [SerializeField]
    private LayerMask _collisionLayers;         // Layers considered for camera occlusion

    // Internal state
    private Vector3 _currentRotation;           // Smoothed rotation angles (pitch, yaw)
    private Vector3 _rotationVelocity;          // Velocity reference for SmoothDamp
    private Vector3 _cameraDirection;           // Local direction vector for camera
    private float _cameraDistance;              // Current distance from target
    private float _yaw;                         // Yaw (horizontal rotation)
    private float _pitch;                       // Pitch (vertical rotation)

    private void Awake()
    {
        // Cache main camera
        _cameraTransform = Camera.main.transform;
        if (_followTarget != null)
            _inputManager = InputManager.Instance;
    }

    private void Start()
    {
        // Initial camera direction and distance
        _cameraDirection = transform.localPosition.normalized;
        _cameraDistance = _collisionDistanceLimits.y;
    }

    private void Update()
    {
        UpdateCursorLock();    // Hide and lock cursor
        HandleCameraInput();   // Read mouse input for yaw/pitch
    }

    private void LateUpdate()
    {
        if (_enableLockOn && _lockOnTarget != null)
        {
            HandleLockOn();     // Rotate towards target when locked on
        }
        else
        {
            ApplyRotation();    // Smoothly rotate camera based on input
        }

        ApplyPosition();        // Move camera to follow target
        ResolveCollision();     // Adjust distance to avoid clipping
    }

    /// <summary>
    /// Locks cursor to center and hides it for gameplay.
    /// </summary>
    private void UpdateCursorLock()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Reads mouse movement to update pitch and yaw.
    /// </summary>
    private void HandleCameraInput()
    {
        if (_enableLockOn) return;

        // Accumulate yaw and pitch
        _yaw += _inputManager.Look.x * _mouseSensitivity;
        _pitch -= _inputManager.Look.y * _mouseSensitivity;
        // Clamp vertical angle
        _pitch = Mathf.Clamp(_pitch, _pitchLimits.x, _pitchLimits.y);
    }

    /// <summary>
    /// Smoothly applies input-based rotation to camera rig.
    /// </summary>
    private void ApplyRotation()
    {
        Vector3 targetRotation = new Vector3(_pitch, _yaw);
        // Smooth damping of rotation angles
        _currentRotation = Vector3.SmoothDamp(_currentRotation, targetRotation, ref _rotationVelocity, _rotationSmoothTime);
        transform.eulerAngles = _currentRotation;
    }

    /// <summary>
    /// Positions camera behind target with offset and smoothing.
    /// </summary>
    private void ApplyPosition()
    {
        Vector3 desiredPosition = _followTarget.position + _followOffset - transform.forward * _followDistance;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, _followLerpTime * Time.deltaTime);
    }

    /// <summary>
    /// Checks for obstacles between target and camera, adjusts distance.
    /// </summary>
    private void ResolveCollision()
    {
        // Compute desired camera local position
        Vector3 rayOrigin = _followTarget.position + _followOffset;
        Vector3 desiredCamWorld = transform.TransformPoint(_cameraDirection * _collisionDistanceLimits.y);

        if (Physics.Linecast(rayOrigin, desiredCamWorld, out RaycastHit hit, _collisionLayers))
        {
            // Clamp distance when hitting obstacle
            _cameraDistance = Mathf.Clamp(hit.distance * 0.9f, _collisionDistanceLimits.x, _collisionDistanceLimits.y);
        }
        else
        {
            _cameraDistance = _collisionDistanceLimits.y;
        }

        // Smoothly move local camera position
        _cameraTransform.localPosition = Vector3.Lerp(_cameraTransform.localPosition,
            _cameraDirection * (_cameraDistance - 0.1f),
            _collisionLerpTime * Time.deltaTime);
    }

    /// <summary>
    /// Rotates camera to look at lock-on target.
    /// </summary>
    private void HandleLockOn()
    {
        Vector3 directionToTarget = (_lockOnTarget.position + Vector3.up * 0.7f - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
    }

    /// <summary>
    /// Sets the follow target at runtime.
    /// </summary>
    public void SetFollowTarget(Transform target)
    {
        _followTarget = target;
    }
}

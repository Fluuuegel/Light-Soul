using UnityEngine;
using Global.Hashes;
using UnityEngine.Rendering;

public abstract class BaseCharacterLocomotion : MonoBehaviour, ILocomotion
{
    // Component references
    protected Animator _animator;
    protected CharacterController _characterController;
    protected InputManager _inputManager;
    protected AudioSource _audioSource;

    // Movement state
    protected Vector3 _horizontalDirection;
    protected Vector3 _verticalDirection;
    protected float _horizontalSpeed;
    protected float _verticalSpeed;

    [SerializeField, Header("Gravity")]
    protected float _characterGravity = -9.81f;

    [SerializeField, Header("Grounded Detection")]
    protected float _groundedRadius;
    [SerializeField]
    protected float _groundedOffset;
    [SerializeField]
    protected LayerMask _groundLayer;
    [SerializeField]
    protected LayerMask _obstacleLayer;
    [SerializeField]
    protected float _slopeRayDistance = 0.2f;
    [SerializeField]
    protected float _terminalSpeed = 53f;

    [SerializeField]
    protected bool _isGrounded;

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
        //_inputManager = GetComponent<InputManager>();
        _inputManager = InputManager.Instance;
        _audioSource = GetComponent<AudioSource>();
    }

    protected virtual void Update()
    {
        GroundedCheck();
        HandleAirState();
    }

    protected virtual void HandleAirState()
    {
        if (_isGrounded)
        {
            _verticalSpeed = 0f;
        }
        else
        {
            _verticalSpeed += _characterGravity * Time.deltaTime;
            _verticalSpeed = Mathf.Sign(_verticalSpeed) * Mathf.Max(Mathf.Abs(_verticalSpeed), _terminalSpeed);
        }
    }

    protected void GroundedCheck()
    {
        Vector3 spherePos = transform.position - Vector3.up * _groundedOffset;
        _isGrounded = Physics.CheckSphere(spherePos, _groundedRadius, _groundLayer, QueryTriggerInteraction.Ignore);
        _animator.SetBool(AnimationHashes.Grounded, _isGrounded);
    }

    protected Vector3 GetOnSlopeDirection(Vector3 dir)
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _slopeRayDistance))
        {
            if (_verticalSpeed <= 0f)
                return Vector3.ProjectOnPlane(dir, hit.normal);
        }
        return dir;
    }

    protected bool CanBeMoved(Vector3 dir)
    {
        return !Physics.Raycast(transform.position + transform.up * .5f,
                            dir.normalized,
                            out var hit,
                            1f,
                            _obstacleLayer);
    }

    public virtual void HandleDodge()
    {

    }

    public void ForcedMove(Vector3 dir, float speed)
    {
        if (CanBeMoved(dir))
        {
            _horizontalDirection = GetOnSlopeDirection(dir.normalized);
            _characterController.Move(speed * dir.normalized * Time.deltaTime);
        }
    }

    public void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            GameResourceManager.Instance.PlaySfx(_audioSource, SoundType.FootStep);
        }
    }

    public void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            GameResourceManager.Instance.PlaySfx(_audioSource, SoundType.Land);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = _isGrounded ? Color.red : Color.green;
        Vector3 pos = new Vector3(transform.position.x, transform.position.y - _groundedOffset, transform.position.z);
        Gizmos.DrawWireSphere(pos, _groundedRadius);
    }
}

using UnityEngine;
using Global.Hashes;

public abstract class BaseCharacterCombat : MonoBehaviour, ICombat
{
    // Component references
    protected Animator _animator;
    protected InputManager _inputManager;
    protected BaseCharacterLocomotion _baseCharacterLocomotion;
    protected AudioSource _audioSource;

    [SerializeField, Header("Attack Range")]
    protected Transform _attackCenter;
    [SerializeField]
    protected float _attackRadius;

    [SerializeField, Header("Detect Range")]
    protected Transform _detectCenter;
    [SerializeField]
    protected float _detectRadius;

    [SerializeField, Header("Auto Lock Range")]
    protected Transform _autoLockCenter;
    [SerializeField]
    protected float _autoLockRadius;

    [SerializeField]
    protected LayerMask _enemyLayer;

    [SerializeField]
    protected float _attackMoveWeight = 2.0f;
    [SerializeField]
    protected Transform _currentTarget;

    protected Collider[] _detectedTargets;
    protected Collider[] _attackTargets;
    protected Collider[] _autoLockTargets;

    public GameObject Weapon1;

    protected bool _isEquipped = false;

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        //_inputManager = GetComponentInParent<InputManager>();
        _inputManager = InputManager.Instance;
        _audioSource = GetComponentInParent<AudioSource>();
        _baseCharacterLocomotion = GetComponent<BaseCharacterLocomotion>();
        _detectedTargets = new Collider[4];
        _detectCenter = transform.Find("DetectCenter");
        _attackTargets = new Collider[4];
        _attackCenter = transform.Find("AttackCenter");
        _autoLockCenter = transform.Find("AutoLockCenter");
        _autoLockTargets = new Collider[1];
    }


    public virtual void OnAttack(string hitAnimationName)
    {
        if (!_animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")) return;

        int targetCount = DetectTarget(_attackTargets, _attackCenter.position, _attackRadius, _enemyLayer);

        // TODO: Add different combo has different damage( Parameters change to float?)
        float damage = 10f;

        for (int i = 0; i < targetCount; i++)
        {
            if (_attackTargets[i].TryGetComponent(out IDamageable damageable))
            {
                //damageable.TakeDamage(hitAnimationName, transform);
                if (_attackTargets[i].gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsTag("Dodge"))
                    continue;
                damageable.TakeDamage(transform, damage);
            }
        }

        GameResourceManager.Instance.PlaySfx(_audioSource, SoundType.SwordSwing);
    }

    public Transform GetCurrentTarget()
    {
        if (_currentTarget == null)
        {
            return null;
        }
        return _currentTarget;
    }
    
    protected void SetCurrentTarget(Transform target)
    {
        if (_currentTarget == null || _currentTarget != target)
        {
            _currentTarget = target;
        }
    }

    protected void ResetTarget()
    {
        _currentTarget = null;
    }
    
    protected int DetectTarget(Collider[] targets, Vector3 center, float radius, LayerMask layer)
    {
        int targetCount = Physics.OverlapSphereNonAlloc(center, radius, targets, layer);
        return targetCount;
    }

    public Quaternion LockOnTarget(Transform self, Transform target, float lerpTime)
    {
        if (target == null) return self.rotation;

        Vector3 targetDirection = (target.position - self.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        
        return Quaternion.Lerp(self.rotation, targetRotation, lerpTime * Time.deltaTime);
    }

    public float GetTargetDistance() => Vector3.Distance(_currentTarget.position, transform.position);
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_attackCenter.position, _attackRadius);
        Gizmos.DrawWireSphere(_detectCenter.position, _detectRadius);
        //Gizmos.DrawWireSphere(_autoLockCenter.position, _autoLockRadius);
    }
}

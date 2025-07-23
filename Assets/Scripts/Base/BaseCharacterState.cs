using System;
using UnityEngine;

public abstract class BaseCharacterState : MonoBehaviour, IDamageable
{

    protected Animator _animator;
    protected BaseCharacterLocomotion _locomotion;
    protected BaseCharacterCombat _combat;
    protected AudioSource _audioSource;

    protected Transform _attacker;

    [SerializeField]
    protected float _currHP;

    [SerializeField]
    protected float _maxHP = 100;

    public event Action<float, float> OnCharacterHPChanged;

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _locomotion = GetComponent<BaseCharacterLocomotion>();
        _combat = GetComponent<BaseCharacterCombat>();
        _audioSource = GetComponent<AudioSource>();
        _currHP = _maxHP;
    }

    protected virtual void Update()
    {
        HandleRepulse();
    }

    protected virtual void HandleRepulse()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Hit"))
        {
            // TODO: Improve it..
            transform.rotation = _combat.LockOnTarget(transform, _attacker, 50f);
            _locomotion.ForcedMove(transform.forward, 0.5f);
        }
    }

    public void TakeDamage(string hitAnimationName)
    {
        _animator.Play(hitAnimationName, 0, 0f);
        GameResourceManager.Instance.PlaySfx(_audioSource, SoundType.Hit);
    }

    public void TakeDamage(string hitAnimationName, Transform attacker)
    {
        _attacker = attacker;
        _animator.Play(hitAnimationName, 0, 0f);
        GameResourceManager.Instance.PlaySfx(_audioSource, SoundType.Hit);
    }

    public void TakeDamage(Transform attacker, float damage)
    {
        _attacker = attacker;
        _animator.Play("Hit_F", 0, 0f);
        _currHP = (_currHP - damage) <= 0 ? 0 : _currHP - damage;
        OnCharacterHPChanged.Invoke(_currHP, _maxHP);
        if (_currHP <= 0)
        {
            // TODO: Death logic
        }
        GameResourceManager.Instance.PlaySfx(_audioSource, SoundType.Hit);
    }
}
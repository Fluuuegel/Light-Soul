using UnityEngine;
using Global.Hashes;
using System;
using UniRx;
public class EnemyCombatController : BaseCharacterCombat
{
    [SerializeField]
    private LayerMask _obstacleLayer;

    private Collider[] _target = new Collider[1];

    private bool _hasTarget = false;
    [SerializeField]
    private float _loseTargetDelta = 3.0f;

    private void Update()
    {
        EnemyView();
        CheckLose();
        LockOnPlayer();
    }

    private void EnemyView()
    {
        int targetCount = DetectTarget(_target, _detectCenter.position, _detectRadius, _enemyLayer);
        if (targetCount > 0)
        {
            _hasTarget = true;
            if (!Physics.Raycast(transform.root.position + transform.root.up * 0.5f, _target[0].transform.position - transform.root.position, out var hit, _detectRadius, _obstacleLayer))
            {
                if (Vector3.Dot((_target[0].transform.position - transform.root.position).normalized, transform.root.forward) > 0.35f)
                {
                    _currentTarget = _target[0].transform;
                }
            }
        }
        else
        {
            _hasTarget = false;
        }
    }

    private IDisposable _loseTargetTimer;

    private void CheckLose()
    {
        if (_currentTarget == null) return;

        if (!_hasTarget && _loseTargetTimer == null)
        {
            _loseTargetTimer = Observable.Timer(TimeSpan.FromSeconds(_loseTargetDelta))
                .ObserveOnMainThread()
                .Subscribe(_ =>
                {
                    OnLoseTarget();
                    _loseTargetTimer = null;
                })
                .AddTo(this);
        }
        else if (_hasTarget)
        {
            _loseTargetTimer?.Dispose();
            _loseTargetTimer = null;
        }
    }

    private void OnLoseTarget()
    {
        _currentTarget = null;
        // Debug.Log("Target Lost");
    }

    private void LockOnPlayer()
    {
        if (_currentTarget != null)
        {
            _animator.SetFloat(AnimationHashes.Lock, 1.0f, 0.1f, Time.deltaTime);
            transform.root.rotation = LockOnTarget(transform.root.transform, _currentTarget, 50f);
        }
        else
        {
            if (_animator.GetFloat(AnimationHashes.Lock) > 0.0f)
            {
                _animator.SetFloat(AnimationHashes.Lock, 0.0f, 0.1f, Time.deltaTime);
            }
        }
    }

    public void OnEquip(AnimationEvent animationEvent)
    {
        // Different from player
        // At first _equipped = false
        Weapon1.SetActive(!_isEquipped);
        _isEquipped = !_isEquipped;
    }
}
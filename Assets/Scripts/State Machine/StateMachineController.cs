using UnityEngine;

public class StateMachineController : MonoBehaviour
{
    public TransitionSO Transition;
    // Default State, cannot be empty
    public StateSO CurrentState;
    public float CurrentStateRunningTimer = 0f;
    public ILocomotion Locomotion { get; private set; }
    public ICombat Combat { get; private set; }
    public Animator _animator;


    private void Awake()
    {
        Locomotion = GetComponent<ILocomotion>();
        Combat = GetComponent<ICombat>();
        _animator = GetComponent<Animator>();
        Transition?.Init(this);
        CurrentState?.OnEnter(this);
    }

    private void Update()
    {
        StateMachineTick();
    }

    private void StateMachineTick() 
    {
        Transition?.EvaluateTransitions();
        CurrentState?.OnUpdate(this);
    }
}

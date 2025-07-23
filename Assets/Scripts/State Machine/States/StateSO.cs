using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateSO : ScriptableObject
{
    [SerializeField]
    protected int _priority;
    protected StateMachineController _controller;

    public virtual void OnEnter(StateMachineController controller)
    {
        _controller = controller;
    }

    public abstract void OnUpdate(StateMachineController controller);

    public virtual void OnExit() {}

    public int GetStatePriority() => _priority;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConditionSO : ScriptableObject
{
    [SerializeField]
    protected int _priority;
    protected StateMachineController _controller;
    public virtual void Init(StateMachineController controller)
    {
        _controller = controller;
    }
    
    public abstract bool IsConditionMet();

    public int GetConditionPriority() => _priority;
}

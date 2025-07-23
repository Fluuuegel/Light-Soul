using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Transition", menuName = "State Machine/Transition")]
public class TransitionSO : ScriptableObject
{
    [Serializable]
    private class TransEntry 
    {
        public StateSO FromState;
        public StateSO ToState;
        public List<ConditionSO> Conditions;
    }

    // The dict contains all the trans config
    private Dictionary<StateSO, List<TransEntry>> _transitions = new Dictionary<StateSO, List<TransEntry>>();

    // Menully config
    [SerializeField]
    private List<TransEntry> _transEntries = new List<TransEntry>();
    private StateMachineController _controller;


    public void Init(StateMachineController controller) 
    {
        _controller = controller;
        BuildTransitionMap();
    }
    
    private void BuildTransitionMap() 
    {
        foreach (var entry in _transEntries)
        {
            if (!_transitions.ContainsKey(entry.FromState))
            {
                _transitions.Add(entry.FromState, new List<TransEntry>());
                _transitions[entry.FromState].Add(entry);
            }
            else
            {
                _transitions[entry.FromState].Add(entry);
            }
            foreach (var condition in entry.Conditions)
            {
                condition.Init(_controller);
            }
        }
    }

    public void EvaluateTransitions() 
    {
        int conditionPriority = 0;
        int statePriority = 0;
        List<StateSO> toStates = new List<StateSO>();
        StateSO toState = null;

        if (_transitions.ContainsKey(_controller.CurrentState)) 
        {
            foreach (var entry in _transitions[_controller.CurrentState])
            {
                foreach (var condition in entry.Conditions)
                {
                    if (condition.IsConditionMet())
                    {
                        if (condition.GetConditionPriority() >= conditionPriority)
                        {
                            // Save the next possible state in ToStates
                            conditionPriority = condition.GetConditionPriority();
                            toStates.Add(entry.ToState);
                        }
                    }
                }
            }
        }
        else 
        {
            return;
        }

        if (toStates.Count != 0) 
        {
            foreach (var state in toStates)
            {
                if (state.GetStatePriority() >= statePriority)
                {
                    statePriority = state.GetStatePriority();
                    toState = state;
                }
            }
        }

        if (toState != null) 
        {
            _controller.CurrentState.OnExit();
            _controller.CurrentState = toState;
            _controller.CurrentState.OnEnter(_controller);            
            toStates.Clear();
        }
    }
}

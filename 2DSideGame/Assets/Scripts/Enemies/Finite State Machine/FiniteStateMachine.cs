using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public sealed class FiniteStateMachine
{
    [SerializeField]
    FiniteState currentState;

    [SerializeField]
    FiniteState[] states;

    public Enemy enemy { get; private set; }

    public void Init(Enemy enemy)
    {
        this.enemy = enemy;
        
        foreach (var state in states)
        {
            state.SetStateMachine(this);
        }
    }

    public void Update()
    {
        currentState.Execute();
        FiniteState next = currentState.GetNextState();
        if (next != null)
        {
            currentState.Shutdown();
            currentState = next;
            currentState.Startup();
        }
    }
}

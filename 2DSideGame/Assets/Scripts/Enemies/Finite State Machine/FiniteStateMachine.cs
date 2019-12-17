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
        currentState.SetStateMachine(this);
    }

    public void Update()
    {
        currentState.Execute();
        FiniteState.TransitionState(currentState);
    }
}

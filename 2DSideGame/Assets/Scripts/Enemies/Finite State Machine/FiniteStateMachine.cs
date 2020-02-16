using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    [System.Serializable]
    public sealed class FiniteStateMachine
    {
        public FiniteState[] states;

        private FiniteState currentState;

        public Enemy enemy { get; private set; }

        public void Init(Enemy enemy)
        {
            this.enemy = enemy;

            foreach (var state in states)
            {
                state.SetStateMachine(this);
            }

            currentState = states[0];
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
}
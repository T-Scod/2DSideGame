using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    [System.Serializable]
    public abstract class FiniteState : MonoBehaviour
    {
        [SerializeField]
        List<Transition> _transitions = new List<Transition>();
        public List<Transition> transitions { get => _transitions; }

        public FiniteStateMachine stateMachine { get; private set; }
        public Enemy enemy { get; private set; }
        public bool complete { get; protected set; }

        public abstract void Execute();

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public virtual void Startup()
        {
        }

        public virtual void Shutdown()
        {
        }

        public void SetStateMachine(FiniteStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
            enemy = stateMachine.enemy;
        }

        public FiniteState GetNextState()
        {
            foreach (var transition in transitions)
            {
                if (transition.conditionsMet)
                {
                    return transition.state;
                }
            }
            return null;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class FiniteState : MonoBehaviour
{
    [SerializeField]
    List<FiniteStateTransition> _transitions = new List<FiniteStateTransition>();
    public List<FiniteStateTransition> transitions { get => _transitions; }

    public FiniteStateMachine stateMachine { get; private set; }
    public Enemy enemy { get; private set; }

    public abstract void Execute();
    
    // Start is called before the first frame update
    void Start()
    {
    }

    protected virtual void OnStart()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected virtual void OnUpdate()
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

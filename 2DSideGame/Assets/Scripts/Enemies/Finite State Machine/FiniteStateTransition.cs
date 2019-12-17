using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public sealed class FiniteStateTransition
{
    [SerializeField]
    FiniteState _state;
    public FiniteState state { get => _state; }

    [SerializeField]
    List<FiniteStateCondition> conditions = new List<FiniteStateCondition>();

    public bool conditionsMet
    {
        get => conditions.TrueForAll(condition => condition.IsMet());
    }
}

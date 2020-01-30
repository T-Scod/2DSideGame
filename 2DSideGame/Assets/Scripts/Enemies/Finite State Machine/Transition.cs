using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    [System.Serializable]
    public sealed class Transition
    {
        [SerializeField]
        FiniteState _state;
        public FiniteState state { get => _state; }

        [SerializeField]
        List<Condition> conditions = new List<Condition>();

        public bool conditionsMet
        {
            get => conditions.TrueForAll(condition => condition.IsMet());
        }
    }
}
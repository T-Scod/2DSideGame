using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    [System.Serializable]
    public sealed class Transition
    {
        public FiniteState state;
        public List<Condition> conditions = new List<Condition>();

        public bool conditionsMet
        {
            get => conditions.TrueForAll(condition => condition.IsMet());
        }
    }
}
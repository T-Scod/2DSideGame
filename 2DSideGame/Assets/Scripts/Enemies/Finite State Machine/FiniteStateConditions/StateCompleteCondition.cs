using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    [AddComponentMenu("FSM/Conditions/StateComplete")]
    public class StateCompleteCondition : Condition
    {
        public FiniteState state;

        protected override bool CheckCondition()
        {
            return state.complete;
        }
    }
}
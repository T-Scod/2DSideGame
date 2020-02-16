using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM.Builder {
    public class StateCompleteConditionNode : ConditionNode
    {
        StateCompleteConditionNode()
        {
            type = ConditionType.STATE_COMPLETE;
        }

        public StateNode state;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM.Builder
{
    public class PlayerProximityConditionNode : ConditionNode
    {
        PlayerProximityConditionNode()
        {
            type = ConditionType.PLAYER_PROXIMITY;
        }

        public float distance;
        public Transform targetTransform;
    }
}

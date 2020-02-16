using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM.Builder
{
    public class ProjectileThreatConditionNode : ConditionNode
    {
        ProjectileThreatConditionNode()
        {
            type = ConditionType.PROJECTILE_THREAT;
        }

        public float distance;
        public Transform parentTransform;
    }
}
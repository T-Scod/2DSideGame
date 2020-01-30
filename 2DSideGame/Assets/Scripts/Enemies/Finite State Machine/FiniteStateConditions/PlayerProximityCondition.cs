using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    [AddComponentMenu("FSM/Conditions/PlayerProximity")]
    public class PlayerProximityCondition : Condition
    {
        public Transform targetTransform;
        public float distance;

        protected override bool CheckCondition()
        {
            float sqrDist = (targetTransform.position - transform.position).sqrMagnitude;
            return sqrDist <= distance * distance;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateConditionPlayerProximity : FiniteStateCondition
{
    public Transform targetTransform;
    public float distance;

    protected override bool CheckCondition()
    {
        float sqrDist = (targetTransform.position - transform.position).sqrMagnitude;
        return sqrDist <= distance * distance;
    }
}

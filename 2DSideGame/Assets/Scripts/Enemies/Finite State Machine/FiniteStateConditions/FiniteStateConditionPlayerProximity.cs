using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateConditionPlayerProximity : FiniteStateCondition
{
    public Transform playerTransform;
    public float distance;

    protected override bool CheckCondition()
    {
        float sqrDist = (playerTransform.position - transform.position).sqrMagnitude;
        return sqrDist <= distance * distance;
    }
}

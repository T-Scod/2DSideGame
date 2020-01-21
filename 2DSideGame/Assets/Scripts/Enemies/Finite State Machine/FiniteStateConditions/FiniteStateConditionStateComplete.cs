using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateConditionStateComplete : FiniteStateCondition
{
    public FiniteState state;

    protected override bool CheckCondition()
    {
        return state.complete;
    }
}

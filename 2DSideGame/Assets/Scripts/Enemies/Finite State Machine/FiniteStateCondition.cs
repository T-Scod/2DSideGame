using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FiniteStateCondition : MonoBehaviour
{
    public bool not;
    protected abstract bool CheckCondition();
    public bool IsMet()
    {
        bool r = CheckCondition();
        return not ? !r : r;
    }
}

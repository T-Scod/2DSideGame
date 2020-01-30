using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    public abstract class Condition : MonoBehaviour
    {
        public bool not;
        protected abstract bool CheckCondition();
        public bool IsMet()
        {
            bool r = CheckCondition();
            return not ? !r : r;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM.Builder
{
    public abstract class ConditionNode : ScriptableObject
    {
        public ConditionType type { get; protected set; }
        public bool not;
    }
}
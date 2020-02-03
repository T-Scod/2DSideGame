using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM.Builder
{
    public abstract class Condition : ScriptableObject
    {
        public bool not;

        public enum Type
        {
            PLAYER_PROXIMITY,
            PROJECTILE_THREAT,
            STATE_COMPLETE,
        }
    }
}
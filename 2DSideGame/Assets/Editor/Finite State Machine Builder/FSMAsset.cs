using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM.Builder
{
    [System.Serializable]
    public class FSMAsset : ScriptableObject
    {
        public List<State> states = new List<State>();
        public List<Transition> transitions = new List<Transition>();
        public List<Comment> comments = new List<Comment>();

        [System.Serializable]
        public struct State
        {
            public StateType type;
            public Rect windowRect;

            // Wait State Data
            public float duration;

            // Seek State Data
            public float speed;
            public Transform targetTransform;
        }

        [System.Serializable]
        public struct Transition
        {
            public int fromState;
            public Vector2 fromStatePos;
            public int toState;
            public Vector2 toStatePos;
            public List<Condition> conditions;
        }

        [System.Serializable]
        public struct Condition
        {
            public ConditionType type;
            public bool not;

            // Player Proximity Data
            public float distance;
            public Transform targetTransform;

            // Projectile Threat Data
            public Transform parentTransform;

            // State Complete Data
            public int state;
        }

        [System.Serializable]
        public struct Comment
        {
            public Rect windowRect;
            public string text;
        }
    }
}

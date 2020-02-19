using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FSM.Builder
{
    public class TransitionNode : BaseNode
    {
        public bool show = false;
        public StateNode fromState;
        public StateNode toState;
        public Vector2 fromStatePos;
        public Vector2 toStatePos;
        public List<ConditionNode> conditions = new List<ConditionNode>();

        public override void DrawWindow()
        {

        }
    }
}

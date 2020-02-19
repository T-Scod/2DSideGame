using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FSM.Builder
{
    public abstract class StateNode : BaseNode
    {
        public List<TransitionNode> transitionNodes = new List<TransitionNode>();
        public abstract void DrawContents();
        public override void DrawWindow()
        {
            DrawContents();
        }
    }
}
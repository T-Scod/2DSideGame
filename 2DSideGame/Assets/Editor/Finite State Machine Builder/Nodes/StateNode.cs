using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FSM.Builder
{
    public class StateNode : BaseNode
    {
        bool collapse;

        List<TransitionNode> transitions = new List<TransitionNode>();

        public void AddTransition()
        {
            //
            // @Todo: Add Transition
            //

            Debug.Log("Add Transition");
        }
    }
}
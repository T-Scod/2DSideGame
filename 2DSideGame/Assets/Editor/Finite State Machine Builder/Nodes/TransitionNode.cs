using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FSM.Builder
{
    public class TransitionNode : BaseNode
    {
        public StateNode fromState;
        public StateNode toState;
        public List<ConditionNode> conditions;

        public override void DrawWindow()
        {

        }

        //public Transition transition;
        //public StateNode enterState;
        //public StateNode targetState;

        //public void Init(StateNode enterState, Transition transition)
        //{
        //    this.transition = transition;
        //    this.enterState = enterState;
        //}

        //public override void DrawWindow()
        //{
        //    if (transition == null)
        //        return;

        //    EditorGUILayout.LabelField("");
        //    EditorGUILayout.Toggle("[Placeholder]", false);
        //}

        //public override void DrawCurve()
        //{
        //    if (enterState)
        //    {
        //        Rect rect = windowRect;
        //        rect.y += windowRect.height * .5f;
        //        rect.width = 1;
        //        rect.height = 1;

        //        // Builder.DrawNodeCurve(enterState.windowRect, rect, Color.black, true);
        //    }
        //}
    }
}

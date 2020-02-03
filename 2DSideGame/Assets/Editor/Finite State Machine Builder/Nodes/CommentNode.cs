using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM.Builder
{
    public class CommentNode : BaseNode
    {
        public string comment = "Comment Here!";

        public override void DrawWindow()
        {
            comment = GUILayout.TextArea(comment, 200);
        }
    }
}
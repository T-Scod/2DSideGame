using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FSM.Builder
{
    public class WaitStateNode : StateNode
    {
        public float duration;

        public override void DrawContents()
        {
            var so = new SerializedObject(this);
            var durationProperty = so.FindProperty("duration");

            EditorGUIUtility.labelWidth = 50;
            EditorGUILayout.PropertyField(durationProperty);

            so.ApplyModifiedProperties();
        }
    }
}

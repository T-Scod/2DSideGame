using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FSM.Builder
{
    public class SeekStateNode : StateNode
    {
        public float speed;
        public Transform targetTransform;

        public override void DrawContents()
        {
            var so = new SerializedObject(this);
            var speedProperty = so.FindProperty("speed");
            var targetProperty = so.FindProperty("targetTransform");

            EditorGUIUtility.labelWidth = 40;
            EditorGUILayout.PropertyField(speedProperty);

            EditorGUIUtility.labelWidth = 100;
            EditorGUILayout.PropertyField(targetProperty);

            so.ApplyModifiedProperties();
        }
    }
}

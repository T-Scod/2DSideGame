using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    [AddComponentMenu("FSM/States/Seek")]
    public class SeekState : FiniteState
    {
        public float speed;
        public Transform targetTransform;

        public override void Execute()
        {
            Vector3 direction = (targetTransform.position - enemy.transform.position).normalized;
            enemy.transform.position += direction * speed * Time.deltaTime;
        }
    }
}
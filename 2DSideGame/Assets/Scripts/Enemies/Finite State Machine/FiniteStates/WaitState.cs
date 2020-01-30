using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    [AddComponentMenu("FSM/States/Wait")]
    public class WaitState : FiniteState
    {
        public float waitDuration;

        private float timer;

        public override void Execute()
        {
            timer += Time.deltaTime;
            if (timer >= waitDuration)
                complete = true;
        }

        public override void Startup()
        {
            timer = 0f;
            complete = false;
        }
    }
}
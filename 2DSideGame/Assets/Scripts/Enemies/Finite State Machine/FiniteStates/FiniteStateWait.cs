using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateWait : FiniteState
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStatePatrol : FiniteState
{
    [Header("Movement")]
    public float speed;
    public float currentSpeed;
    public float slowDownDistance = 1f;
    public float slowDownMagnitude = 1f;

    [Header("Pathing")]
    public int currentPointIndex;
    public EnemyPath path;

    public override void Execute()
    { 
        // calculate direction
        currentPointIndex = path.GetRequiredIndex(enemy.transform, currentPointIndex);
        Vector3 target = path.GetPoint(currentPointIndex);
        Vector3 current = enemy.transform.position;
        Vector3 direction = target - current;
        float sqrDist = direction.sqrMagnitude;
        direction.Normalize();

        // calculate speed
        currentSpeed = speed;
        if (sqrDist <= slowDownDistance * slowDownDistance)
        {
            float percentageTravelled = Mathf.InverseLerp(slowDownDistance * slowDownDistance, 0f, sqrDist);
            float slowDown = percentageTravelled * slowDownMagnitude;
            currentSpeed = speed - (speed * slowDown);
        }

        // move enemy
        enemy.transform.position = current + direction * currentSpeed * Time.deltaTime;
    }
}

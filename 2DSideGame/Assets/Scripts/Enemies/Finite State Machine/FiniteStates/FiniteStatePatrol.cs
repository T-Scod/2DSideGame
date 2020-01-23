using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStatePatrol : FiniteState
{
    [Header("Movement")]
    public float speed;
    public float currentSpeed;
    public float slowDownDistance = 1f;

    [Header("Pathing")]
    public int currentPointIndex;
    public EnemyPath path;

    public override void Execute()
    { 
        // calculate direction
        currentPointIndex = path.GetRequiredIndex(enemy.transform.position, currentPointIndex);
        Vector3 target = path.GetPoint(currentPointIndex);
        Vector3 current = enemy.transform.position;
        Vector3 direction = target - current;
        float sqrDist = direction.sqrMagnitude;
        direction.Normalize();

        // calculate speed
        currentSpeed = speed;
        if (sqrDist <= slowDownDistance * slowDownDistance)
        {
            float dist = Mathf.Sqrt(sqrDist);
            currentSpeed = speed * (dist / slowDownDistance);
        }

        // move enemy
        enemy.transform.position = current + direction * currentSpeed * Time.deltaTime;
    }

    //
    // @Summary: Sets currentPointIndex to the closest point on the path
    //
    public override void Startup()
    {
        int closest = int.MaxValue;
        float closestSqrDistance = float.PositiveInfinity;
        for (int i = 0; i < path.numPoints; i++)
        {
            float sqrDist = (path.GetPoint(i) - enemy.transform.position).sqrMagnitude;
            if (sqrDist < closestSqrDistance)
            {
                closest = i;
                closestSqrDistance = sqrDist;
            }
        }
        currentPointIndex = closest;
    }
}

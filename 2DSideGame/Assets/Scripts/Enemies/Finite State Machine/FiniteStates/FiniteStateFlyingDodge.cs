using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateFlyingDodge : FiniteState
{
    public float dodgeSpeed;
    public float dodgeDistance;
    public float detectionRange;

    private Vector3 endPosition;

    public override void Execute()
    {
        enemy.transform.position = Vector3.Lerp(enemy.transform.position, endPosition, dodgeSpeed * Time.deltaTime);
        float sqrDist = (enemy.transform.position - endPosition).sqrMagnitude;
        if (sqrDist <= 0.1f)
        {
            complete = true;
        }
    }

    //
    // @Summary: Calculates position enemy needs to move to for successfuly dodge
    //
    public override void Startup()
    {
        // reset state
        complete = false;

        // get potential threats
        Collider2D[] potentialThreats = Physics2D.OverlapCircleAll(enemy.transform.position, detectionRange);
        List<Vector3> avasionDirs = new List<Vector3>();

        foreach (var threat in potentialThreats)
        {
            TestPlayerProjectile projectile = threat.GetComponent<TestPlayerProjectile>();
            if (projectile != null)
            {
                Vector3 desiredDodgeDirection = enemy.transform.position - projectile.transform.position;
                desiredDodgeDirection.z = 0f;
                desiredDodgeDirection.Normalize();
                desiredDodgeDirection = Vector2.Perpendicular(desiredDodgeDirection);

                // flip direction depending on projectile's future position relative to the enemy
                Vector3 projFuturePosition = projectile.transform.position + projectile.velocity * Time.deltaTime;
                // change y direction
                if (desiredDodgeDirection.y > 0 && projFuturePosition.y > enemy.transform.position.y ||
                    desiredDodgeDirection.y < 0 && projFuturePosition.y < enemy.transform.position.y)
                    desiredDodgeDirection.y = -desiredDodgeDirection.y;
                // change x direction
                if (desiredDodgeDirection.x > 0 && projFuturePosition.x > enemy.transform.position.x ||
                    desiredDodgeDirection.x < 0 && projFuturePosition.x < enemy.transform.position.x)
                    desiredDodgeDirection.x = -desiredDodgeDirection.x;

                avasionDirs.Add(desiredDodgeDirection);
            }
        }

        // calculate average
        Vector3 direction = new Vector3(0f, 0f, 0f);
        foreach (var dir in avasionDirs)
        {
            direction += dir;
        }
        direction.z = 0f;
        direction.Normalize();

        endPosition = enemy.transform.position + direction * dodgeDistance;
    }
}

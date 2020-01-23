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
        //// get potential threats
        //TestPlayerProjectile[] potentialThreats = FindObjectsOfType<TestPlayerProjectile>();
        //List<Vector3> avasionVectors = new List<Vector3>();

        //// calculate avasion vectors for all threats
        //foreach (var threat in potentialThreats)
        //{
        //    RaycastHit2D hit = Physics2D.Raycast(threat.transform.position + threat.direction * 2f, threat.direction);
        //    bool collides = hit.collider == enemy.collider;
        //    bool withinDistance = hit.distance <= detectionRange;

        //    if (collides && withinDistance)
        //    {
        //        Vector3 desiredDodgeDirection = enemy.transform.position - (Vector3)hit.point;
        //        desiredDodgeDirection.z = 0f;
        //        desiredDodgeDirection.Normalize();
        //        avasionVectors.Add(Vector2.Perpendicular(desiredDodgeDirection));
        //    }
        //}

        //// calculate average
        //Vector3 direction = new Vector3(0f, 0f, 0f);
        //foreach (var vector in avasionVectors)
        //{
        //    direction += vector;
        //}
        //direction.z = 0f;
        //direction.Normalize();

        //endPosition = enemy.transform.position + direction * dodgeDistance;

        // reset state
        complete = false;

        // get potential threats
        Collider2D[] potentialThreats = Physics2D.OverlapCircleAll(enemy.transform.position, detectionRange);
        List<Vector3> avasionDirs = new List<Vector3>();

        foreach (var threat in potentialThreats)
        {
            Vector3 desiredDodgeDirection = enemy.transform.position - (Vector3)threat.ClosestPoint(enemy.transform.position);
            desiredDodgeDirection.z = 0f;
            desiredDodgeDirection.Normalize();
            desiredDodgeDirection = Vector2.Perpendicular(desiredDodgeDirection);
            avasionDirs.Add(desiredDodgeDirection);
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

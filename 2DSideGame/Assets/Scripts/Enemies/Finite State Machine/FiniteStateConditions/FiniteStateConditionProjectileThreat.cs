using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateConditionProjectileThreat : FiniteStateCondition
{
    public Collider2D parentCollider;
    public float distance;

    protected override bool CheckCondition()
    {
        // get potential threats
        TestPlayerProjectile[] potentialThreats = FindObjectsOfType<TestPlayerProjectile>();
        List<Vector3> avasionVectors = new List<Vector3>();

        // calculate avasion vectors for all threats
        foreach (var threat in potentialThreats)
        {
            RaycastHit2D hit = Physics2D.Raycast(threat.transform.position + threat.direction * 2f, threat.direction);
            bool collides = hit.collider == parentCollider;
            bool withinDistance = hit.distance <= distance; 

            if (collides && withinDistance)
            {
                return true;
            }
        }
        return false;
    }
}

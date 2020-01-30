using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    [AddComponentMenu("FSM/Conditions/ProjectileThreat")]
    public class ProjectileThreatCondition : Condition
    {
        public Transform parentTransform;
        public float distance;

        protected override bool CheckCondition()
        {
            //// get potential threats
            //TestPlayerProjectile[] potentialThreats = FindObjectsOfType<TestPlayerProjectile>();
            //List<Vector3> avasionVectors = new List<Vector3>();

            //// calculate avasion vectors for all threats
            //foreach (var threat in potentialThreats)
            //{
            //    RaycastHit2D hit = Physics2D.Raycast(threat.transform.position + threat.direction * 2f, threat.direction);
            //    bool collides = hit.collider == parentCollider;
            //    bool withinDistance = hit.distance <= distance; 

            //    if (collides && withinDistance)
            //    {
            //        return true;
            //    }
            //}
            //return false;

            Collider2D[] potentialThreats = Physics2D.OverlapCircleAll(parentTransform.position, distance);
            foreach (var threat in potentialThreats)
            {
                bool isProj = threat.GetComponent<TestPlayerProjectile>();
                if (isProj)
                    return true;
            }

            return false;
        }
    }
}
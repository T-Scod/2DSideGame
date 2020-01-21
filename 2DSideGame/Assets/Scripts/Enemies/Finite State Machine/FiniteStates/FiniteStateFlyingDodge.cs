using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @Implement: Implement collision avoidance steering behaviour
    /*  private function collisionAvoidance() :Vector3D {
            ahead = ...; // calculate the ahead vector
            ahead2 = ...; // calculate the ahead2 vector
            var mostThreatening :Obstacle = findMostThreateningObstacle();
            var avoidance :Vector3D = new Vector3D(0, 0, 0);
            if (mostThreatening != null) {
                avoidance.x = ahead.x - mostThreatening.center.x;
                avoidance.y = ahead.y - mostThreatening.center.y;
 
                avoidance.normalize();
                avoidance.scaleBy(MAX_AVOID_FORCE);
            } else {
                avoidance.scaleBy(0); // nullify the avoidance force
            }
            return avoidance;
        }
        private function findMostThreateningObstacle() :Obstacle {
            var mostThreatening :Obstacle = null;
            for (var i:int = 0; i < Game.instance.obstacles.length; i++) {
                var obstacle :Obstacle = Game.instance.obstacles[i];
                var collision :Boolean = lineIntersecsCircle(ahead, ahead2, obstacle);
                // "position" is the character's current position
                if (collision && (mostThreatening == null || distance(position, obstacle) < distance(position, mostThreatening))) {
                    mostThreatening = obstacle;
                }
            }
            return mostThreatening;
        }
    */

public class FiniteStateFlyingDodge : FiniteState
{
    public float dodgeSpeed;
    public float dodgeAcceleration;
    public float dodgeDistance;

    private Vector3 direction;
    private Vector3 startPosition;
    private float sqrDistance;
    private Vector3 velocity;

    public override void Execute()
    {
        // calculate velocity
        velocity += direction * dodgeAcceleration;
        velocity = Vector3.ClampMagnitude(velocity, dodgeSpeed);

        enemy.transform.Translate(velocity * Time.deltaTime);

        // complete state if dodge is done
        float currentSqrDistance = (enemy.transform.position - startPosition).sqrMagnitude;
        if (currentSqrDistance >= sqrDistance)
        {
            complete = true;
        }
    }

    //
    // @Summary: Calculates position enemy needs to move to for successfuly dodge
    //
    public override void Startup()
    {
        // get potential threats
        TestPlayerProjectile[] potentialThreats = FindObjectsOfType<TestPlayerProjectile>();
        List<Vector3> avasionVectors = new List<Vector3>();

        // calculate avasion vectors for all threats
        foreach (var threat in potentialThreats)
        {
            RaycastHit2D hit = Physics2D.Raycast(threat.transform.position + threat.direction * 2f, threat.direction);
            bool collides = hit.collider == enemy.collider;
            bool withinDistance = hit.distance <= dodgeDistance;

            if (collides && withinDistance)
            {
                Vector3 desiredDodgeDirection = (enemy.transform.position - (Vector3)hit.point).normalized;
                avasionVectors.Add(desiredDodgeDirection);
            }
        }

        // calculate average
        foreach (var vector in avasionVectors)
        {
            direction += vector;
        }
        direction /= avasionVectors.Count;

        // setup dodge
        startPosition = enemy.transform.position;
        sqrDistance = direction.sqrMagnitude * dodgeSpeed;
    }
}

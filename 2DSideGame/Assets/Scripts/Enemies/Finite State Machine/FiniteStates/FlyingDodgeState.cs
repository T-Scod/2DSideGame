using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    [AddComponentMenu("FSM/States/FlyingDodge")]
    public class FlyingDodgeState : FiniteState
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
        // @Summary: Resets state for next time state is transitioned to
        //
        public override void Shutdown()
        {
            complete = false;
        }

        //
        // @Summary: Calculates position enemy needs to move to for successfuly dodge
        //
        public override void Startup()
        {
            // get potential threats
            Collider2D[] potentialThreats = Physics2D.OverlapCircleAll(enemy.transform.position, detectionRange);
            Vector3 direction = new Vector3(0f, 0f, 0f);

            foreach (var threat in potentialThreats)
            {
                TestPlayerProjectile projectile = threat.GetComponent<TestPlayerProjectile>();
                if (projectile != null)
                {
                    // calculate desired direction to avoid the threat
                    direction = enemy.transform.position - projectile.transform.position;
                    direction.z = 0f;
                    direction.Normalize();
                    direction = Vector2.Perpendicular(direction);

                    // flip direction depending on projectile's future position relative to the enemy
                    Vector3 projFuturePosition = projectile.transform.position + projectile.velocity * Time.deltaTime;
                    // change y direction
                    if (direction.y > 0 && projFuturePosition.y > enemy.transform.position.y ||
                        direction.y < 0 && projFuturePosition.y < enemy.transform.position.y)
                        direction.y = -direction.y;
                    // change x direction
                    if (direction.x > 0 && projFuturePosition.x > enemy.transform.position.x ||
                        direction.x < 0 && projFuturePosition.x < enemy.transform.position.x)
                        direction.x = -direction.x;
                    break;
                }
            }

            endPosition = enemy.transform.position + direction * dodgeDistance;
        }
    }
}
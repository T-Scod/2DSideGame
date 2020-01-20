using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateSeek : FiniteState
{
    public float speed;
    public Transform playerTransform;

    public override void Execute()
    {
        Vector3 direction = (playerTransform.position - enemy.transform.position).normalized;
        enemy.transform.position += direction * speed * Time.deltaTime;
    }
}

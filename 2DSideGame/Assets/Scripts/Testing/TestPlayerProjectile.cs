using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerProjectile : MonoBehaviour
{
    public float speed;
    public Vector3 velocity;
    public Vector3 direction { get => velocity.normalized; }

    private void Update()
    {
        transform.Translate(velocity * Time.deltaTime);
    }

    public void Fire(Vector3 direction)
    {
        velocity = direction * speed;
        velocity.z = 0f;

        Transform child = transform.GetChild(0);
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        angle -= child.transform.eulerAngles.z;
        child.transform.rotation = Quaternion.AngleAxis(angle, Vector3.back);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }
}

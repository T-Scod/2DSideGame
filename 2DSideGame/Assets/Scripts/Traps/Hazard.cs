using UnityEngine;

public class Hazard : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 displacementVector = collision.transform.position - transform.position;
            collision.GetComponent<Rigidbody>().AddForce(displacementVector);
        }
    }
}
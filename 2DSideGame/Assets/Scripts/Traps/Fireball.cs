using UnityEngine;

public class Fireball : MonoBehaviour
{
    public Vector3 moveDirection = new Vector2(1.0f, 0.0f);
    public float moveSpeed = 10.0f;

    private bool m_isAlive = false;

    private void Update()
    {
        if (m_isAlive)
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Rigidbody>().AddForce(moveDirection * moveSpeed);
        }

        m_isAlive = false;
        GetComponent<Animator>().SetTrigger("Dead");
    }

    public void Alive()
    {
        m_isAlive = true;
    }

    public void Dead()
    {
        Destroy(gameObject);
    }
}
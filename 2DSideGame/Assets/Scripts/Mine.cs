using UnityEngine;

public class Mine : MonoBehaviour
{
    private Animator m_animator;
    private LayerMask m_playerLayer;
    private float m_radius;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_radius = GetComponent<CircleCollider2D>().radius;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            m_animator.SetTrigger("Flash");
            m_playerLayer.value = collision.gameObject.layer;
        }
    }

    public void Explode()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, m_radius, m_playerLayer);
        if (player != null)
        {
            player.GetComponent<Rigidbody>().AddExplosionForce(m_radius, transform.position, m_radius);
        }
    }

    public void Dead()
    {
        Destroy(gameObject);
    }
}
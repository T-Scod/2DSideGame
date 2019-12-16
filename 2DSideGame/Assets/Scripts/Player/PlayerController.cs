using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerGroundCheck m_groundDetection = new PlayerGroundCheck();
    public PlayerHealth m_health = new PlayerHealth();
    public PlayerMovement m_movement = new PlayerMovement();

    [HideInInspector]
    public Rigidbody2D m_rb;

    // Start is called before the first frame update
    void Start()
    {
        m_health.Init(this);
        m_groundDetection.Init(this);
        m_movement.Init(this);

        m_movement.Start();

        m_rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        m_movement.Update();
        m_groundDetection.Update();
    }

    private void FixedUpdate()
    {
        m_movement.FixedUpdate();
        m_groundDetection.FixedUpdate();
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_groundDetection.m_groundCheck.position, m_groundDetection.m_groundCheckRadius);
        Gizmos.DrawLine(m_movement.m_wallCheck.position, new Vector2(m_movement.m_wallCheck.position.x + m_movement.m_wallCheckDistance, m_movement.m_wallCheck.position.y));
    }
}
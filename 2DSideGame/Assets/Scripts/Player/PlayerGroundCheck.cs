using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerGroundCheck
{
    [Tooltip("The radius of the ground detection trigger")]
    public float m_groundCheckRadius;
    [Tooltip("The time gap the player has to then jump again even if the player doesnt touch the ground")]
    [SerializeField]
    private float m_groundedRememberTime = 0.2f;

    [Tooltip("The transform that is attached to the player for the ground detection")]
    public Transform m_groundCheck;
    [Tooltip("The ground layer mask that the player can walk on")]
    public LayerMask m_whatIsGround;

    [HideInInspector]
    public float m_groundedRemember = 0;
    [HideInInspector]
    public bool m_isGrounded;
    private PlayerController m_player;

    public void Update()
    {
        RememberGroundedTimer();
    }

    public void FixedUpdate()
    {
        CheckSurroundings();
    }
    
    public void Init(PlayerController player)
    {
        m_player = player;
    }

    public void RememberGroundedTimer()
    {
        m_groundedRemember -= Time.deltaTime;
        if (m_isGrounded)
        {
            m_groundedRemember = m_groundedRememberTime;
        }
    }

    public void CheckSurroundings()
    {
        m_isGrounded = Physics2D.OverlapCircle(m_groundCheck.position, m_groundCheckRadius, m_whatIsGround);
    }
}
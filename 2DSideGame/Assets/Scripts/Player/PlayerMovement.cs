using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerMovement
{
    [Tooltip("The amount of jumps the player can jump in a row")]
    public int m_jumpAmounts = 1;
    [Tooltip("How fast the player can run")]
    public float m_movementSpeed = 10f;
    [Tooltip("The force applied to the player when the jump button is pressed")]
    public float m_jumpForce = 16f;
    [Tooltip("The time gap the player has to then jump again even if the player doesnt touch the ground")]
    public float m_jumpPressedRememberTime = 0.2f;
    [Tooltip("How fast the player comes back down to the ground when they hold down the jump button")]
    public float m_fallMultiplier = 2.5f;
    [Tooltip("How fast the player comes back down to the ground when they press down the jump button")]
    public float m_lowJumpMultiplier = 2f;
    public float m_movementForceInAir;
    [Tooltip("The drag that gets applied to the player when they are in the air and not moving")]
    public float m_airDragMulti = 0.95f;
    [Tooltip("The force that is applied to the player when they hop off a wall")]
    public float m_wallHopForce = 10f;
    [Tooltip("The force thats applied to the player in the direction they are jumping off the wall")]
    public float m_wallJumpForce = 20f;
    [Tooltip("Hop direction the player goes to")]
    public Vector2 m_wallHopDir;
    [Tooltip("The direction the player goes to when they jump off a wall")]
    public Vector2 m_wallJumpDir;

    [Header("--DASH--")]
    [SerializeField]
    [Tooltip("How long the dash should take")]
    private float m_dashTime;
    [SerializeField]
    [Tooltip("How fast the player will dash")]
    private float m_dashSpeed;
    [SerializeField]
    [Tooltip("How far apart the ghost will be placed when dashing")]
    private float m_distanceBetweenGhost;
    [SerializeField]
    [Tooltip("How long we wait before we can dash again ")]
    private float m_dashCooldown;


    [Header("--WALL CHECK--")]
    [Tooltip("The distance of the raycast that detects walls")]
    public float m_wallCheckDistance;
    [Tooltip("The speed at which the player slides down the wall")]
    public float m_wallSlideSpeed;
    [Tooltip("The wall transform that is attached to the player that will be drawn as a gizmo")]
    public Transform m_wallCheck;

    private PlayerController m_player;
    private int m_amountOfJumpsLeft;
    private int m_facingDir = 1;
    private float m_jumpPressedRemember = 0;
    private float m_movementInputDir;
    private float m_dashTimeLeft; // tracks how much longer the dash should be happening
    private float m_lastGhostXPos; // tracks the x position of the last ghost
    private float m_lastDash = -100; // tracks the last dash which is used to check for the cooldown
    private bool m_isFacingRight = true;
    private bool m_canJump = false;
    private bool m_isWalking;
    private bool m_isTouchingWall;
    private bool m_isWallSliding;
    private bool m_isDashing = false; // is player currently dashing?

    // Start is called before the first frame update
    public void Start()
    {
        m_amountOfJumpsLeft = m_jumpAmounts;
        m_wallHopDir.Normalize(); // mg = 1
        m_wallJumpDir.Normalize(); // mg = 1
    }

    // Update is called once per frame
    public void Update()
    {
        CheckInput();
        CheckMovementDirection();
        RememberJumpTimer();
        JumpJuice();
        CheckIfCanJump();
        CheckIfWallSliding();
        JumpJuice();
        CheckDash();
    }

    public void FixedUpdate()
    {
        ApplyMovement();
        RememberTimers();
        CheckSurroundings();
    }

    public void Init(PlayerController player)
    {
        m_player = player;
    }

    private void RememberTimers()
    {
        if ((m_jumpPressedRemember > 0) && (m_player.m_groundDetection.m_groundedRemember > 0))
        {
            m_jumpPressedRemember = 0;
            m_player.m_groundDetection.m_groundedRemember = 0;
            m_player.m_rb.velocity = new Vector2(m_player.m_rb.velocity.x, m_jumpForce);
        }
    }

    public void CheckInput()
    {
        m_movementInputDir = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Dash"))
        {
            if (Time.time >= (m_lastDash + m_dashCooldown))
            {
                Dash();
            }
        }
    }

    private void Dash()
    {
        m_isDashing = true;
        m_dashTimeLeft = m_dashTime;
        m_lastDash = Time.time;
        ObjectPooling.Instance.GetFromPool();
        m_lastGhostXPos = m_player.transform.position.x;
    }

    private void CheckDash()
    {
        if (m_isDashing)
        {
            if (m_dashTimeLeft > 0)
            {
                m_player.m_rb.velocity = new Vector2(m_dashSpeed * m_facingDir, m_player.m_rb.velocity.y);
                m_dashTimeLeft -= Time.deltaTime;

                if (Mathf.Abs(m_player.transform.position.x - m_lastGhostXPos) > m_distanceBetweenGhost)
                {
                    ObjectPooling.Instance.GetFromPool();
                    m_lastGhostXPos = m_player.transform.position.x;
                }
            }

            if (m_dashTimeLeft <= 0 || m_isTouchingWall)
            {
                m_isDashing = false;
            }
        }
    }

    public void CheckMovementDirection()
    {
        if (m_isFacingRight && m_movementInputDir < 0)
        {
            Flip();
        }
        else if (!m_isFacingRight && m_movementInputDir > 0)
        {
            Flip();
        }

        if (m_player.m_rb.velocity.x != 0)
        {
            m_isWalking = true;
        }
        else
        {
            m_isWalking = false;
        }
    }

    public void RememberJumpTimer()
    {
        m_jumpPressedRemember -= Time.deltaTime;
        if (Input.GetKeyDown("space"))
        {
            Jump();
            m_jumpPressedRemember = m_jumpPressedRememberTime;
        }
    }

    public void Jump()
    {
        if (m_canJump && !m_isWallSliding)
        {
            m_player.m_rb.velocity = new Vector2(m_player.m_rb.velocity.x, m_jumpForce);
            m_amountOfJumpsLeft--;
        }
        else if (m_isWallSliding && m_movementInputDir == 0 && m_canJump) // wall hop
        {
            m_isWallSliding = false;
            m_amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(m_wallHopForce * m_wallHopDir.x * -m_facingDir, m_wallHopForce * m_wallHopDir.y);
            m_player.m_rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
        else if ((m_isWallSliding || m_isTouchingWall) && m_movementInputDir != 0 && m_canJump) // wall jump
        {
            m_isWallSliding = false;
            m_amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(m_wallJumpForce * m_wallJumpDir.x * m_movementInputDir, m_wallJumpForce * m_wallJumpDir.y);
            m_player.m_rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
    }

    public void ApplyMovement()
    {
        if (m_player.m_groundDetection.m_isGrounded)
        {
            m_player.m_rb.velocity = new Vector2(m_movementSpeed * m_movementInputDir, m_player.m_rb.velocity.y);
        }
        else if (!m_player.m_groundDetection.m_isGrounded && !m_isWallSliding && m_movementInputDir != 0)
        {
            Vector2 forceAdded = new Vector2(m_movementForceInAir * m_movementInputDir, 0);
            m_player.m_rb.AddForce(forceAdded);

            if (Mathf.Abs(m_player.m_rb.velocity.x) > m_movementSpeed)
            {
                m_player.m_rb.velocity = new Vector2(m_movementSpeed * m_movementInputDir, m_player.m_rb.velocity.y);
            }
        }
        else if (!m_player.m_groundDetection.m_isGrounded && !m_isWallSliding && m_movementInputDir == 0)
        {
            m_player.m_rb.velocity = new Vector2(m_player.m_rb.velocity.x * m_airDragMulti, m_player.m_rb.velocity.y);
        }


        if (m_isWallSliding)
        {
            if (m_player.m_rb.velocity.y < -m_wallSlideSpeed)
            {
                m_player.m_rb.velocity = new Vector2(0, -m_wallSlideSpeed);
            }
        }
    }

    public void Flip()
    {
        if (!m_isWallSliding)
        {
            m_facingDir *= -1;
            m_isFacingRight = !m_isFacingRight;
            m_player.transform.Rotate(0f, 180f, 0f);
        }
    }

    public void CheckIfICanJump()
    {
        if ((m_player.m_groundDetection.m_isGrounded && m_player.m_rb.velocity.y <= 0) || m_isWallSliding)
        {
            m_amountOfJumpsLeft = m_jumpAmounts;
        }

        if (m_amountOfJumpsLeft <= 0)
        {
            m_canJump = false;
        }
        else
        {
            m_canJump = true;
        }
    }

    private void CheckIfWallSliding()
    {
        if (m_isTouchingWall && !m_player.m_groundDetection.m_isGrounded && m_player.m_rb.velocity.y < 0)
        {
            m_isWallSliding = true;
        }
        else
        {
            m_isWallSliding = false;
        }
    }

    public void JumpJuice()
    {
        if (m_player.m_rb.velocity.y < 0)
        {
            m_player.m_rb.velocity += Vector2.up * Physics2D.gravity.y * (m_fallMultiplier - 1) * Time.deltaTime;

        }
        else if (m_player.m_rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            m_player.m_rb.velocity += Vector2.up * Physics2D.gravity.y * (m_lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    public void CheckIfCanJump()
    {
        if ((m_player.m_groundDetection.m_isGrounded && m_player.m_rb.velocity.y <= 0) || m_isWallSliding)
        {
            m_amountOfJumpsLeft = m_jumpAmounts;
        }

        if (m_amountOfJumpsLeft <= 0)
        {
            m_canJump = false;
        }
        else
        {
            m_canJump = true;
        }
    }

    public void CheckSurroundings()
    {
        m_isTouchingWall = Physics2D.Raycast(m_wallCheck.position, m_player.transform.right, m_wallCheckDistance, m_player.m_groundDetection.m_whatIsGround);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float m_movementInputDir;
    private Rigidbody2D m_rb;
    private bool m_isGrounded;
    private bool m_isFacingRight = true;
    private bool m_isWalking;
    private bool m_canJump;
    private bool m_isTouchingWall;
    private bool m_isWallSliding;
    private int m_facingDir = 1;
    private int m_amountOfJumpsLeft;
    private float m_jumpPressedRemember = 0;
    private float m_groundedRemember = 0;

    [Header("--MOVEMENT--")]
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

    [Header("--GROUND CHECK--")]
    [Tooltip("The radius of the ground detection trigger")]
    public float m_groundCheckRadius;
    [Tooltip("The time gap the player has to then jump again even if the player doesnt touch the ground")]
    public float m_groundedRememberTime = 0.2f;
    [Tooltip("The transform that is attached to the player for the ground detection")]
    public Transform m_groundCheck;
    [Tooltip("The ground layer mask that the player can walk on")]
    public LayerMask m_whatIsGround;

    [Header("--WALL CHECK--")]
    [Tooltip("The distance of the raycast that detects walls")]
    public float m_wallCheckDistance;
    [Tooltip("The speed at which the player slides down the wall")]
    public float m_wallSlideSpeed;
    [Tooltip("The wall transform that is attached to the player that will be drawn as a gizmo")]
    public Transform m_wallCheck;

    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_amountOfJumpsLeft = m_jumpAmounts;
        m_wallHopDir.Normalize(); // magn = 1
        m_wallJumpDir.Normalize(); // magn = 1
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        CheckIfCanJump();
        CheckIfWallSliding();
        JumpJuice();
        RememberGroundedTimer();
        RememberJumpTimer();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurroundings();
        RememberTimers();
    }

    public void RememberGroundedTimer()
    {
        m_groundedRemember -= Time.deltaTime;
        if (m_isGrounded)
        {
            m_groundedRemember = m_groundedRememberTime;
        }
    }

    public void RememberJumpTimer()
    {
        m_jumpPressedRemember -= Time.deltaTime;
        if (Input.GetKeyDown("space"))
        {
            m_jumpPressedRemember = m_jumpPressedRememberTime;
        }
    }

    private void RememberTimers()
    {
        if ((m_jumpPressedRemember > 0) && (m_groundedRemember > 0))
        {
            m_jumpPressedRemember = 0;
            m_groundedRemember = 0;
            m_rb.velocity = new Vector2(m_rb.velocity.x, m_jumpForce);
        }
    }

    private void Jump()
    {
        if (m_canJump && !m_isWallSliding)
        {
            m_rb.velocity = new Vector2(m_rb.velocity.x, m_jumpForce);
            m_amountOfJumpsLeft--;
        }
        else if (m_isWallSliding && m_movementInputDir == 0 && m_canJump) // wall hop
        {
            m_isWallSliding = false;
            m_amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(m_wallHopForce * m_wallHopDir.x * -m_facingDir, m_wallHopForce * m_wallHopDir.y);
            m_rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
        else if ((m_isWallSliding || m_isTouchingWall) && m_movementInputDir != 0 && m_canJump) // wall jump
        {
            m_isWallSliding = false;
            m_amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(m_wallJumpForce * m_wallJumpDir.x * m_movementInputDir, m_wallJumpForce * m_wallJumpDir.y);
            m_rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
    }

    public void JumpJuice()
    {
        if (m_rb.velocity.y < 0)
        {
            m_rb.velocity += Vector2.up * Physics2D.gravity.y * (m_fallMultiplier - 1) * Time.deltaTime;

        }
        else if (m_rb.velocity.y > 0 && !Input.GetKey("space"))
        {
            m_rb.velocity += Vector2.up * Physics2D.gravity.y * (m_lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    public void CheckIfCanJump()
    {
        if ((m_isGrounded && m_rb.velocity.y <= 0) || m_isWallSliding)
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

    private void CheckInput()
    {
        m_movementInputDir = Input.GetAxisRaw("Horizontal");
    }

    private void ApplyMovement()
    {
        if (m_isGrounded)
        {
            m_rb.velocity = new Vector2(m_movementSpeed * m_movementInputDir, m_rb.velocity.y);
        }
        else if (!m_isGrounded && !m_isWallSliding && m_movementInputDir != 0)
        {
            Vector2 forceAdded = new Vector2(m_movementForceInAir * m_movementInputDir, 0);
            m_rb.AddForce(forceAdded);

            if (Mathf.Abs(m_rb.velocity.x) > m_movementSpeed)
            {
                m_rb.velocity = new Vector2(m_movementSpeed * m_movementInputDir, m_rb.velocity.y);
            }
        }
        else if (!m_isGrounded && !m_isWallSliding && m_movementInputDir == 0)
        {
            m_rb.velocity = new Vector2(m_rb.velocity.x * m_airDragMulti, m_rb.velocity.y);
        }

        if (m_isWallSliding)
        {
            if (m_rb.velocity.y < -m_wallSlideSpeed)
            {
                m_rb.velocity = new Vector2(0, -m_wallSlideSpeed);
            }
        }
    }

    private void CheckMovementDirection()
    {
        if (m_isFacingRight && m_movementInputDir < 0)
        {
            Flip();
        }
        else if (!m_isFacingRight && m_movementInputDir > 0)
        {
            Flip();
        }

        if (m_rb.velocity.x != 0)
        {
            m_isWalking = true;
        }
        else
        {
            m_isWalking = false;
        }
    }


    private void CheckIfWallSliding()
    {
        if (m_isTouchingWall && !m_isGrounded && m_rb.velocity.y < 0)
        {
            m_isWallSliding = true;
        }
        else
        {
            m_isWallSliding = false;
        }
    }

    private void CheckSurroundings()
    {
        m_isGrounded = Physics2D.OverlapCircle(m_groundCheck.position, m_groundCheckRadius, m_whatIsGround);

        m_isTouchingWall = Physics2D.Raycast(m_wallCheck.position, transform.right, m_wallCheckDistance, m_whatIsGround);
    }

    private void Flip()
    {
        if (!m_isWallSliding)
        {
            m_facingDir *= -1;
            m_isFacingRight = !m_isFacingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_groundCheck.position, m_groundCheckRadius);
        Gizmos.DrawLine(m_wallCheck.position, new Vector2(m_wallCheck.position.x + m_wallCheckDistance, m_wallCheck.position.y));
    }
}
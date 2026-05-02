using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    private float xInput;
    private float yInput;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;

    [Header("Detections")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    private bool isGrounded;
    private bool isAirborne;
    private bool isWallDetected;

    [Header("Wall Interactions")]
    [SerializeField] private Vector2 wallJumpForce;
    [SerializeField] private float wallJumpDuration;
    private bool isWallJumping;

    private bool facingRight = true;
    private int facingDir = 1;

    private bool canDoubleJump = true;

    [Header("Knockback")]
    [SerializeField] private Vector2 knockbackForce;
    [SerializeField] private float knockbackDuration;
    private bool isKnocked;

    [Header("Buffer & Coyote Jumps")]
    [SerializeField] private float bufferJumpAttemptTime = -1f;
    [SerializeField] private float bufferJumpTreshold;

    [SerializeField] private float coyoteJumpLeavingTime = -1f;
    [SerializeField] private float coyoteJumpTreshold = 0.3f;
 

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }


    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.K))
        {
            Knockback();
        }

        UpdateAirborneStatus();

        if(isKnocked)
            return;

        HandleInput();

        HandleFlip();

        HandleWallSlide();

        HandleMovement();

        HandleDetections();

        HandleAnimations();


    }

    private void UpdateAirborneStatus()
    {
        if (isGrounded && isAirborne)
            HandleLanding();
        if (!isGrounded && !isAirborne)
            BecomeAirborne();

        WallDoubleJump();
    }

    private void HandleLanding()
    {
       isAirborne = false;
       canDoubleJump = true;
       BufferJump();
    }
    private void BecomeAirborne()
    {
       isAirborne = true;
       CoyoteJumpLeave();
    }

    private void CoyoteJumpLeave()
    {
        if(rb.velocity.y <= 0)
        coyoteJumpLeavingTime = Time.time;
    }

 
    private void RequestBufferJump()
    {
        if(isAirborne)
          bufferJumpAttemptTime = Time.time;
    }

    private void BufferJump()
    {
        if (Time.time <= bufferJumpAttemptTime + bufferJumpTreshold)
        {
            Jump();
            bufferJumpAttemptTime = Time.time - 1f;
        }
    }

    private void WallDoubleJump()
    {
        if (isWallDetected && !canDoubleJump)
            canDoubleJump = true;
    }
    private void HandleInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpButton();
            RequestBufferJump();
        }
    }

    public void Knockback()
    { 
        if(isKnocked)
            return;
        StartCoroutine(KnockbackRoutine());
        anim.SetTrigger("Knockback");
        rb.velocity = new Vector2(knockbackForce.x * -facingDir, knockbackForce.y);

    }

    private IEnumerator KnockbackRoutine()
    {
        isKnocked = true;
        yield return new WaitForSeconds(knockbackDuration);
        isKnocked = false;
    }
    private void WallJump()
    {
        StopCoroutine(WallJumpRoutine());
        StartCoroutine(WallJumpRoutine());
        Flip();
    }

    private IEnumerator WallJumpRoutine()
    {
        isWallJumping = true;
        rb.velocity = new Vector2(wallJumpForce.x * -facingDir, wallJumpForce.y);

        yield return new WaitForSeconds(wallJumpDuration);
        isWallJumping = false;
    }
    private void JumpButton()
    {
        bool canCoyoteJump = Time.time <= coyoteJumpLeavingTime + coyoteJumpTreshold;
        if (isGrounded || canCoyoteJump )
        {
            Jump();
            coyoteJumpLeavingTime = Time.time - 1f;
        }
        else if (isWallDetected)
        {
            WallJump();
        }
        else if (canDoubleJump)
        {
            DoubleJump();
            isWallJumping = false;
            canDoubleJump = false;
        }
        
    }

    private void Jump() => rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    private void DoubleJump() => rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);

    private void HandleFlip()
    {
        if (xInput < 0 && facingRight || xInput > 0 && !facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
       facingDir = facingDir * -1;
       transform.Rotate(0, 180, 0);
       facingRight = !facingRight;
    }

    private void HandleMovement()
    {
        if (isWallJumping)
            return;
        if (isWallDetected)
            return;
        rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
    }
    private void HandleWallSlide()
    {
        float friction = 0.05f;

        if (isWallDetected && rb.velocity.y < 0)
        {
            if (yInput < 0)
            {
                friction = 0.987f;
            }
            else
            {
                friction = 0.05f;
            }
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * friction);
        }
    }
    private void HandleAnimations()
    {
        anim.SetFloat("xVelocity", rb.velocity.x);
        anim.SetFloat("yVelocity", rb.velocity.y);

        anim.SetBool("isWallDetected", isWallDetected);
        anim.SetBool("isGrounded", isGrounded);
    }
    private void HandleDetections()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDir, wallCheckDistance, groundLayer) ||
                         Physics2D.Raycast(new Vector2(transform.position.x,transform.position.y + 0.35f), Vector2.right * facingDir,wallCheckDistance,groundLayer) || 
                         Physics2D.Raycast(new Vector2(transform.position.x,transform.position.y - 0.35f), Vector2.right * facingDir,wallCheckDistance,groundLayer); 
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, (new Vector2(transform.position.x, transform.position.y - groundCheckDistance)));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, (new Vector2(transform.position.x + (wallCheckDistance * facingDir), transform.position.y)));
        Gizmos.DrawLine((new Vector2(transform.position.x, transform.position.y + 0.35f)),new Vector2((transform.position.x + (wallCheckDistance * facingDir)), transform.position.y + 0.35f));
        Gizmos.DrawLine((new Vector2(transform.position.x, transform.position.y - 0.35f)),new Vector2((transform.position.x + (wallCheckDistance * facingDir)), transform.position.y - 0.35f));
    }
}

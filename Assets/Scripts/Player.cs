using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject deathVfx;
    private Rigidbody2D rb;
    private Animator anim;
    
    private float xInput;
    private float yInput;

    [Header("Movement details")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    private bool canDoubleJump;

    [Header("Buffer & Coyote Jump")]
    [SerializeField] private float bufferJumpTreshold = .25f;
    [SerializeField] private float coyoteJumpTreshold = .5f;
    private float bufferJumpAttemptTime = -1;
    private float coyoteJumpLeavingTime = -1;

    [Header("Detections")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] float groundCheckDistance;
    [SerializeField] float wallCheckDistance;
    private bool isWallDetected;
    private bool isGrounded;
    private bool isAirborne;

    [Header("Wall Interactions")]
    [SerializeField] private Vector2 wallJumpForce;
    [SerializeField] private float wallJumpDuration;
    [SerializeField] private float wallYAxisDistance;
    private bool isWallJumping;

    [Header("Flip")]
    private int facingDir = 1;
    private bool lookingRight = true;

    [Header("Knockback")]
    [SerializeField] private Vector2 knockbackForce;
    [SerializeField] private float knockbackDuration;
    private bool isKnocked;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {   
        UpdateAirborneStatus();
        UpdateWallDoubleJump();
        if (isKnocked)
            return;
        HandleCollisions();
        HandleAnimations();
        HandleInput();
        HandleWallSlide();
        HandleMovement();
        HandleFlip();
    }
    public void Knockback()
    {
        if (isKnocked)
            return;

        StartCoroutine(KnockbackRoutine());
        anim.SetTrigger("knockback");
        rb.velocity = new Vector2(knockbackForce.x * -facingDir, knockbackForce.y);
    }

    private IEnumerator KnockbackRoutine()
    {
        isKnocked = true;
        yield return new WaitForSeconds(knockbackDuration);
        isKnocked = false;
    }

    public void Die()
    {
        Instantiate(deathVfx, transform.position, Quaternion.identity);
        Destroy(gameObject);

    }
    private void UpdateAirborneStatus()
    {
        if (isGrounded && isAirborne)
            HandleLanding();

        if (!isGrounded && !isAirborne)
            BecomeAirborne();
    }
    private void BecomeAirborne()
    {
        isAirborne = true;

        if(rb.velocity.y <= 0)
           AttemptCoyoteJump();
    }
    private void HandleLanding()
    {
        isAirborne = false;
        canDoubleJump = true;
        AttemptBufferJump();
    }

    private void UpdateWallDoubleJump()
    {
        if(isWallDetected && !canDoubleJump)
            canDoubleJump = true;
    }

    private void HandleInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");//horizontal input
        yInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space)) //vertical input
        {
            JumpButton();
            RequestBufferJump();
        }
    }

    #region Buffer & Coyote Jump
    private void AttemptCoyoteJump() => coyoteJumpLeavingTime = Time.time;
    private void CancelCoyoteJump() => coyoteJumpLeavingTime = Time.time - 1;
    private void RequestBufferJump() // it wants to do buffer jump. 
    {
        if (isAirborne)
            bufferJumpAttemptTime = Time.time;
    }
    private void AttemptBufferJump()
    {
        if (Time.time < bufferJumpAttemptTime + bufferJumpTreshold)
        {
            Jump();
            bufferJumpAttemptTime = Time.time - 1;
        }
    }

    #endregion
  
    private IEnumerator WallJumpRoutine()
    {
        isWallJumping = true;
        yield return new WaitForSeconds(wallJumpDuration);
        isWallJumping = false;

    }
    private void WallJump()
    {
        canDoubleJump = true;
        rb.velocity = new Vector2(wallJumpForce.x * -facingDir, wallJumpForce.y);
        Flip();

        StopAllCoroutines();
        StartCoroutine(WallJumpRoutine());
    }
    private void HandleWallSlide()
    {
        bool canWallSlide = isWallDetected && rb.velocity.y < 0;
        float yModifier = yInput < 0 ? .99f : 0.35f;
        //If you pressing down, yInput is going to equals .99f, else it's .35f ..

        if (!canWallSlide)
            return;

        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * yModifier);
    }
    private void DoubleJump()
    {
        StopCoroutine(WallJumpRoutine());
        isWallJumping = false; //neden stopallcoroutines kullanmýyoruz ? true olarak kalabiliyor ve inputu sonsuza dek kitliyor.
        canDoubleJump = false;
        rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
    }
    
    private void Jump() => rb.velocity = new Vector2(rb.velocity.x, jumpForce);
  
    private void JumpButton()
    {
        bool canCoyoteJump = Time.time < coyoteJumpLeavingTime + coyoteJumpTreshold;

        if (isGrounded || canCoyoteJump) // normal zýplama yapmaya çalýþýrken coyotejump aktive edilebilir ondan burada!!
        {
            Jump();
        }
        else if (isWallDetected)
        {
            WallJump(); 
        }
        else if (canDoubleJump)
        {
            DoubleJump();            
        }
        CancelCoyoteJump();
    }

    private void HandleMovement()
    { 
        if (isWallDetected)
            return;

        if(isWallJumping)
            return;

        rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
    }

    private void HandleAnimations()
    {
        anim.SetFloat("xVelocity", rb.velocity.x);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isWallDetected", isWallDetected);
    }
    private void HandleCollisions()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);

        isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDir, wallCheckDistance, groundLayer) ||
          Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + wallYAxisDistance), Vector2.right * facingDir, wallCheckDistance, groundLayer) ||
          Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - wallYAxisDistance), Vector2.right * facingDir, wallCheckDistance, groundLayer);
    }

    private void HandleFlip()
    {
        if (xInput < 0 && lookingRight || xInput > 0 && !lookingRight)
            Flip();
    }   
    private void Flip()
    {
        facingDir = facingDir * -1;
        lookingRight = !lookingRight;
        transform.Rotate(0, 180, 0);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(transform.position,(new Vector2(transform.position.x, transform.position.y - groundCheckDistance)));

        Gizmos.DrawLine(transform.position, (new Vector2(transform.position.x + (wallCheckDistance * facingDir), transform.position.y)));

        Gizmos.DrawLine(new Vector2(transform.position.x,transform.position.y + wallYAxisDistance),
            (new Vector2(transform.position.x + (wallCheckDistance * facingDir), transform.position.y + wallYAxisDistance)));
      
        Gizmos.DrawLine(new Vector2(transform.position.x,transform.position.y - wallYAxisDistance),
            (new Vector2(transform.position.x + (wallCheckDistance * facingDir), transform.position.y - wallYAxisDistance)));
    }
}

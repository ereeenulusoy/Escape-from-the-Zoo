using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    
    private float xInput;
    private float yInput;

    [Header("Movement details")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;

    private bool canDoubleJump;



    private int facingDir = 1;
    private bool lookingRight = true;

    [Header("Collision Info")]
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] float groundCheckDistance;
    [SerializeField] float wallCheckDistance;
   
    private bool isGrounded;
    private bool isAirborne;
    private bool isWallDetected;

    
  
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        HandleInput();
        HandleWallSlide();
        UpdateAirborneStatus();
        HandleFlip();
        HandleMovement();
        HandleCollisions();
        HandleAnimations();

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
    private void UpdateAirborneStatus()
    {
        if (isGrounded && isAirborne)
        {
            HandleLanding();
        }

        if (!isGrounded && !isAirborne)
        {
            BecomeAirborne();
        }
    }

    private void BecomeAirborne()
    {
        isAirborne = true;
    }

    private void HandleLanding()
    {
        isAirborne = false;
        canDoubleJump = true;
    }

    private void HandleInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");//horizontal input
        yInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space)) //vertical input
        {
            JumpButton();
        }
    }

    private void JumpButton()
    {
        if (isGrounded)
        {
            Jump();
        }
        else if (canDoubleJump)
        {
           DoubleJump(); 
        }
    }

    private void DoubleJump()
    {
        canDoubleJump = false;
        rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void HandleMovement()
    {
        if (isWallDetected)
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
        isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDir, wallCheckDistance, groundLayer);
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
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(transform.position,(new Vector2(transform.position.x, transform.position.y - groundCheckDistance)));

        Gizmos.color = Color.red;

        Gizmos.DrawLine(transform.position, (new Vector2(transform.position.x + (wallCheckDistance * facingDir), transform.position.y)));
    }
}

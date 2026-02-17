using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    
    private float xInput;

    [Header("Movement details")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;

    private bool canDoubleJump;


  

    private bool lookingRight = true;

    [Header("Collision Info")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] float groundCheckDistance;
   
    private bool isGrounded;
    private bool isAirborne;

    
  
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        HandleCollisions();
        HandleInput();
        UpdateAirborneStatus();
        HandleFlip();
        HandleMovement();
        HandleAnimations();

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

    public void DoubleJump()
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
        rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
    }

    private void HandleAnimations()
    {
        anim.SetFloat("xVelocity", rb.velocity.x);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isGrounded", isGrounded);
    }
    private void HandleCollisions()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
    }

    private void HandleFlip()
    {
        if (rb.velocity.x < 0 && lookingRight || rb.velocity.x > 0 && !lookingRight)
            Flip();
    }
    private void Flip()
    {
        transform.Rotate(0, 180, 0);
        lookingRight = !lookingRight;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(transform.position,(new Vector2(transform.position.x, transform.position.y - groundCheckDistance)));
    }
}

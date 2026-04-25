using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    private float xInput;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;

    [Header("Detections")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance;
    private bool isGrounded;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();   
        anim = GetComponentInChildren<Animator>();
    }


    private void Update()
    {
        HandleDetections();

        HandleInput();

        HandleMovement();

        HandleAnimations();
    }



    private void HandleInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        if(Input.GetKeyDown(KeyCode.Space))
        {
            JumpButton();
        }
    }

    private void JumpButton()
    {
        if(isGrounded)
        Jump();
    }

    private void Jump() => rb.velocity = new Vector2(rb.velocity.x, jumpForce);

    private void HandleMovement()
    {
        rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
    }
    private void HandleAnimations()
    {
        anim.SetFloat("xVelocity",rb.velocity.x);
    }
    private void HandleDetections()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}

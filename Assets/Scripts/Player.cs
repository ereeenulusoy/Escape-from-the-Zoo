using System;
using System.Collections;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.XR;

public class Player : MonoBehaviour
{

    private Rigidbody2D rb;
    private Animator anim;

    private float xInput;
    private float yInput;

    [SerializeField] private float moveSpeed = 5f;

    [Header("Detections")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    private bool isWallDetected;
    private bool isGrounded;
    private bool isAirborne;

    [Header("Jump")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    private bool canDoubleJump;

    [Header("Flip Function")]
    private bool facingRight = true;
    private int facingDir = 1;

    [Header("Wall Jump")]
    [SerializeField] private Vector2 wallJumpForce;
    [SerializeField] private float wallJumpDuration;
    private bool isWallJumping;



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }
    private void Update()
    {
        
        UpdateAirborneStatus();
        HandleFlip();
        HandleMovement();
        HandleWallSlide();
        HandleInputs();
        HandleAnimations();
        HandleDetections();
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


    private void HandleLanding()
    {
        isAirborne = false;
        canDoubleJump = true;
    }
    private void BecomeAirborne()
    {
        isAirborne = true;
    }

    private void HandleAnimations()
    {
        anim.SetFloat("xVelocity", rb.velocity.x);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isWallDetected", isWallDetected);
    }


    private void HandleWallSlide()
    {
        bool canWallSlide = isWallDetected && rb.velocity.y < 0;
        float yMultiplier = yInput < 0 ? 0.97f : 0.05f;

        if (!canWallSlide)
            return;
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * yMultiplier);

    }
    
    private void WallJump()
    {
       
        StopCoroutine(WallJumpRoutine());
        StartCoroutine(WallJumpRoutine());
        rb.velocity = new Vector2(wallJumpForce.x * - facingDir, wallJumpForce.y);
        Flip();

    }

    private IEnumerator WallJumpRoutine()
    {
        isWallJumping = true;
        yield return new WaitForSeconds(wallJumpDuration);
        isWallJumping = false;
    }
    private void HandleInputs()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
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
        else if(isWallDetected)
        {
            WallJump();
        }
        else if (canDoubleJump)
        {
            DoubleJump();
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
        transform.Rotate(0f, 180f, 0f);
        facingRight = !facingRight;
        facingDir *= -1;
    }

    private void HandleMovement()
    {
        if(isWallJumping)
            return;
        if (isWallDetected)
            return;

        rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
    }
    private void HandleDetections()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDir, wallCheckDistance, groundLayer) ||
                         Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + .45f), Vector2.right * facingDir, wallCheckDistance, groundLayer) ||
                         Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - .45f), Vector2.right * facingDir, wallCheckDistance, groundLayer);

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (wallCheckDistance * facingDir), transform.position.y));
        Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y + .45f), new Vector2(transform.position.x + (wallCheckDistance * facingDir),transform.position.y + .45f));
        Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y - .45f), new Vector2(transform.position.x + (wallCheckDistance * facingDir),transform.position.y - .45f));
        
    }
}

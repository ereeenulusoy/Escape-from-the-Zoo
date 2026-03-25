using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;

    private float xInput;
    private float yInput;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    private bool canDoubleJump = true;

    [Header("Buffer & Coyote Jump")]
    [SerializeField] private float bufferJumpTreshold;
    private float bufferJumpRequestTime = -1f;

    [Header("Flip variables")]
    private bool isLookingRight = true;
    private int facingDir = 1;

    [Header("Wall Jump")]
    [SerializeField] private float wallJumpDuration;
    [SerializeField] private Vector2 wallJumpForce;
    private bool isWallJumping;

    [Header("Detections")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    private bool isWallDetected;
    private bool isGrounded;
    private bool isAirborne;

    [Header("Knockback")]
    [SerializeField] private float knockbackDuration;
    [SerializeField] private Vector2 knockbackForce;
    private bool isKnocked;

    [Header("Player Death")]
    [SerializeField] private GameObject deathFX;

    private void Awake()
    {
      rb = GetComponent<Rigidbody2D>();
      anim = GetComponentInChildren<Animator>();
    }


    private void Update()
    {
        UpdateAirborneStatus();
        if (Input.GetKeyDown(KeyCode.C))
        {
            Knockback();
        }
        if (isKnocked)
            return;
        HandleDetections();
        HandleInput();
        HandleWallSlide();
        HandleFlip();
        HandleMovement();
        HandleAnimations();
    }

    private void UpdateAirborneStatus() // Yere deðerkenki ve yerde olmadýðýmýz "ÝLK" anda deðiþiklik yapmamýzý saðlar.
    {
        if (isGrounded && isAirborne)
        {
            HandleLanding();
            AttemptBufferJump();
        }
        if (!isGrounded && !isAirborne)
        {
            BecomeAirborne();
        }
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
        Destroy(gameObject);
        GameObject newDeathFX = Instantiate(deathFX,transform.position, Quaternion.identity);   
    }
    private void RequestBufferJump()
    {
        if(!isGrounded)
        bufferJumpRequestTime = Time.time;
    }
    private void AttemptBufferJump()
    {
        if (Time.time < bufferJumpRequestTime + bufferJumpTreshold)
        {
            Jump();
            bufferJumpRequestTime = Time.time - 1;
        }
    }
    private void WallJump()
    {
        StopAllCoroutines();
        StartCoroutine(WallJumpRoutine());
        rb.velocity = new Vector2(wallJumpForce.x *(facingDir * -1), wallJumpForce.y);
        Flip();
    }

    private IEnumerator WallJumpRoutine()
    {
        isWallJumping = true;
        canDoubleJump = true;
        yield return new WaitForSeconds(wallJumpDuration);
        isWallJumping = false;
    }
    private void HandleWallSlide()
    {
        bool canWallSlide = isWallDetected && rb.velocity.y < 0;
        float wallFrictionAmount = yInput < 0 ? .99f : .35f ;

        if (!canWallSlide) // böyle yapmak olasý bazý buglarý engelleyebilir. Sektör standardý.
            return;

            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * wallFrictionAmount);
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

    private void JumpButton()
    {
        if (isGrounded)
            Jump();

        else if (isWallDetected && !isWallJumping)
            WallJump();

        else if (canDoubleJump)
            DoubleJump();
        
    }

    private void DoubleJump()
    {
        StopCoroutine(WallJumpRoutine());
        isWallJumping = false;
        canDoubleJump = false;
        rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
    }

    private void Jump() => rb.velocity = new Vector2(rb.velocity.x, jumpForce);
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
    private void HandleMovement()
    {
        if (isWallDetected)
            return;

        if (isWallJumping)
            return;

        rb.velocity = new Vector2(moveSpeed * xInput, rb.velocity.y);
    }

    private void HandleFlip()
    {
        if (xInput < 0 && isLookingRight || xInput > 0 && !isLookingRight)
        {
            Flip();
        }
    }
    private void Flip()
    {
        isLookingRight = !isLookingRight;
        facingDir = facingDir * -1;
        transform.Rotate(0, 180, 0);
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
            Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + .33f), Vector2.right * facingDir, wallCheckDistance, groundLayer)|| 
            Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - .33f), Vector2.right * facingDir, wallCheckDistance, groundLayer);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (wallCheckDistance * facingDir), transform.position.y));
        Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y + .33f), new Vector2(transform.position.x + (wallCheckDistance * facingDir), transform.position.y + .33f));
        Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y - .33f), new Vector2(transform.position.x + (wallCheckDistance * facingDir), transform.position.y - .33f));
    }
}

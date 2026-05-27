using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{

    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D cd;

    private float xInput;
    private float yInput;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    private bool canDoubleJump;
    private float defaultGravityScale;
    private bool canBeControlled = false;

    [Header("Detections")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    private bool isWallDetected;
    private bool isGrounded;
    private bool isAirborne;


    [Header("Flip Function")]
    private bool facingRight = true;
    private int facingDir = 1;

    [Header("Wall Jump")]
    [SerializeField] private Vector2 wallJumpForce;
    [SerializeField] private float wallJumpDuration;
    private bool isWallJumping;

    [Header("Knockback")]
    [SerializeField] private float knockbackDuration;
    [SerializeField] private Vector2 knockbackForce;
    private bool isKnocked;

    [Header("Buffer & Coyote Jump")]
    [SerializeField] private float bufferJumpTreshold;
    [SerializeField] private float coyoteJumpTreshold;
    private float bufferJumpAttempTime = -1f;
    private float coyoteJumpLeavingTime = -1f;

    [Header("VFX")]
    [SerializeField] private GameObject deathVfx;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        cd = GetComponent<CapsuleCollider2D>();
    }
    private void Start()
    {
        defaultGravityScale = rb.gravityScale;
        RespawnFinished(false);
    }
    private void Update()
    {

        UpdateAirborneStatus();

        if (!canBeControlled)
            return;
        if (isKnocked)
            return;
        HandleInputs();
        HandleWallSlide();
        HandleMovement();
        HandleFlip();
        HandleDetections();
        HandleAnimations();
    }
    
    public void RespawnFinished(bool finished)
    {
        
     if(finished)
        {
            canBeControlled = true;
            rb.gravityScale = defaultGravityScale;
            cd.enabled = true;
        }
     else
        {
            canBeControlled = false;
            rb.gravityScale = 0f;
            cd.enabled = false;
        }
    }
    public void Die()
    {

     GameObject newDeathVfx = Instantiate(deathVfx, transform.position, Quaternion.identity);
        
     Destroy(gameObject);

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
        UpdateDoubleJump();
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
        RequestCoyoteTime();
    }

    private void HandleAnimations()
    {
        anim.SetFloat("xVelocity", rb.velocity.x);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isWallDetected", isWallDetected);

    }


    #region Coyote & Buffer Jump
    private void RequestCoyoteTime()
    {
        if (rb.velocity.y <= 0)
            coyoteJumpLeavingTime = Time.time;
    }



    private void AttemptBufferJump()
    {
        if (isAirborne)
            bufferJumpAttempTime = Time.time;
    }

    private void BufferJump()
    {
        if (Time.time < bufferJumpAttempTime + bufferJumpTreshold)
        {
            Jump();
            bufferJumpAttempTime = Time.time - 1f;
        }
    }
    #endregion


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

    private void HandleInputs()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpButton();
            AttemptBufferJump();
        }
    }

    private void JumpButton()
    {
        bool coyoteJump = Time.time < coyoteJumpLeavingTime + coyoteJumpTreshold;
        if (isGrounded || coyoteJump)
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
            canDoubleJump = false;
        }
        coyoteJumpLeavingTime = -1f;
    }

    private void Jump() => rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    private void DoubleJump()
    {
        StopCoroutine(WallJumpRoutine());
        isWallJumping = false;
        rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);

    }

    private void UpdateDoubleJump()
    {
        if (isWallDetected && !canDoubleJump)
        {
            canDoubleJump = true;
        }
    }


    #region Wall Interactions
    private void HandleWallSlide()
    {
        bool canWallSlide = isWallDetected && rb.velocity.y < 0;
        float yMultiplier = yInput < 0 ? 0.995f : 0.05f;

        if (!canWallSlide)
            return;

        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * yMultiplier);

    }
    private void WallJump()
    {

        rb.velocity = new Vector2(wallJumpForce.x * -facingDir, wallJumpForce.y);
        Flip();
        canDoubleJump = true;
        StopCoroutine(WallJumpRoutine());
        StartCoroutine(WallJumpRoutine());

    }

    private IEnumerator WallJumpRoutine()
    {
        isWallJumping = true;
        yield return new WaitForSeconds(wallJumpDuration);
        isWallJumping = false;
    }
    #endregion
    private void HandleFlip()
    {
        if (isWallJumping)
            return;
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
        if (isWallJumping)
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
        Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y + .45f), new Vector2(transform.position.x + (wallCheckDistance * facingDir), transform.position.y + .45f));
        Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y - .45f), new Vector2(transform.position.x + (wallCheckDistance * facingDir), transform.position.y - .45f));

    }
}

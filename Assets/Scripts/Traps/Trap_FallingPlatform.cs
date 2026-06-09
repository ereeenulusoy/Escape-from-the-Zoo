using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Trap_FallingPlatform : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private BoxCollider2D[] colliders;

    [SerializeField] float moveSpeed = .2f;
    [SerializeField] float travelDistance = .6f;
    private Vector3[] wayPointPosition;
    private int wayPointIndex = 1;
    public bool canMove;

    [Header("Platform Fall Details")]
    [SerializeField] private float fallDelay = .5f;
    [SerializeField] private float impactSpeed = 3f;
    [SerializeField] private float impactDuration = .1f;
    private float impactTimer;
    private bool impacted;

    [Header("Recreate Details")]
    [SerializeField] private bool canBeRecreated;
    [SerializeField] private Transform startPosition;
    [SerializeField] private float recreateDelay = 2.5f;
    [SerializeField] private float scaleUpSpeed = 10;
    [SerializeField] private Vector3 targetScale;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponents<BoxCollider2D>();
    }
    private IEnumerator Start()
    {
        AssignWayPointPositions();

        startPosition = transform;

        transform.localScale = new Vector3(.25f, .25f, .25f);

        float startRandomGlide = Random.Range(0, .6f);

        yield return new WaitForSeconds(startRandomGlide);
        canMove = true; // baþlangýçta false tutuyoruz ki hareket etmesinler. rastgele
                        // zaman bekledikten sonra harekete baþlarlar.

    }
    private void Update()
    {
        HandleRecreateScaleUp();
        HandleMovement();
        HandleImpact();
    }

    private void HandleRecreateScaleUp()
    {
        if (transform.localScale.x < targetScale.x)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, scaleUpSpeed * Time.deltaTime);
        }
    }

    private void HandleMovement()
    {
        if (canMove == false)
            return;
        transform.position = Vector2.MoveTowards(transform.position, wayPointPosition[wayPointIndex], moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, wayPointPosition[wayPointIndex]) < .1f)
        {
            wayPointIndex++;

            if (wayPointIndex >= wayPointPosition.Length)
                wayPointIndex = 0;
        }
    }

    private void HandleImpact()
    {
        if (impactTimer < 0)
            return;

        impactTimer -= Time.deltaTime;

        transform.position =
            Vector2.MoveTowards(transform.position, (transform.position + Vector3.down * 10), impactSpeed * Time.deltaTime);
    }

    private void AssignWayPointPositions()
    {
      wayPointPosition = new Vector3[2];

        float yOffset = travelDistance / 2;

        wayPointPosition[0] = transform.position + new Vector3(0, yOffset, 0); 
        wayPointPosition[1] = transform.position + new Vector3(0, -yOffset, 0);    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (impacted)
            return;
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            impacted = true; // 1 kere dokunmalý yoksa sürekli aþaðýya iniyor.

            impactTimer = impactDuration;

            Invoke(nameof(SwitchOffPlatform), fallDelay);

            RecreatePlatform();

            Destroy(gameObject, 2f);
        }
    }
    private void RecreatePlatform()
    {
        if (canBeRecreated == false)
            return;
        GameObject platformPrefab = GameManager.instance.fallingPlatformPrefab;
        GameManager.instance.RecreateObject(platformPrefab, startPosition, recreateDelay);
    }

    private void SwitchOffPlatform()
    {
        anim.SetTrigger("deactivate");

        canMove = false;

        rb.isKinematic = false;   // Unity 6 ve sonrasýnda rb.bodyType = RigidBodyType2D.Dynamic olarak deðiþecek.
        rb.gravityScale = 3.5f;   //Ayný
        rb.drag = .5f;            // rb.linearDamping olarak deðiþecek.
                                  // Air resistance diyebiliriz.
                                  //colliders box collider2D array'inin içerisindeki tüm boxcollider'larýna bu iþlemi yap demek !!!
        foreach (BoxCollider2D collider in colliders)
        {
            collider.enabled = false;
        }      
    }

}

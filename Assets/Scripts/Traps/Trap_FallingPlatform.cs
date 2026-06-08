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
    public bool canMove = true;

    [Header("Platform Fall Details")]
    [SerializeField] private float fallDelay = .5f;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponents<BoxCollider2D>();
    }
    private void Start()
    {
        AssignWayPointPositions();
        transform.position = wayPointPosition[0];
    }
    private void Update()
    {
        HandleMovement();
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

    private void AssignWayPointPositions()
    {
      wayPointPosition = new Vector3[2];

        float yOffset = travelDistance / 2;

        wayPointPosition[0] = transform.position + new Vector3(0, yOffset, 0); 
        wayPointPosition[1] = transform.position + new Vector3(0, -yOffset, 0);    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            Invoke(nameof(SwitchOffPlatform), fallDelay);
        }
    }

    private void SwitchOffPlatform()
    {
        anim.SetTrigger("deactivate");

        canMove = false;

        rb.isKinematic = false;// Unity 6 ve sonrasýnda rb.bodyType = RigidBodyType2D.Dynamic olarak deðiþecek.
        rb.gravityScale = 3.5f; //Ayný
        rb.drag = .5f; // rb.linearDamping olarak deðiþecek.
                       // Air resistance diyebiliriz.

        //colliders box collider2D array'inin içerisindeki tüm boxcollider'larýna bu iþlemi yap demek !!!
        foreach (BoxCollider2D collider in colliders)
        {
            collider.enabled = false;
        }
    }
}

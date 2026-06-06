using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Trap_Saw : MonoBehaviour
{

    [SerializeField] private float moveSpeed;
    [SerializeField] private float cooldown = 0.5f;
    [SerializeField] private Transform[] waypoint;
    
    private Vector3[] waypointPosition;
    private int nextWayPointIndex = 1;
    private int moveDirection = 1;
    private Animator anim;
    private SpriteRenderer sr;

    private bool canMove = true;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        RegisterWaypoint();

        transform.position = waypointPosition[0];
    }


    private void Update()
    {
        anim.SetBool("activate", canMove);

        if (canMove == false)
            return;

        transform.position = Vector2.MoveTowards(transform.position, waypointPosition[nextWayPointIndex], moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, waypointPosition[nextWayPointIndex]) < .1f)
        {
            if (nextWayPointIndex == waypointPosition.Length - 1 || nextWayPointIndex == 0)
            {
                moveDirection = moveDirection * -1;
                StartCoroutine(StopRoutine(cooldown));
            }
            nextWayPointIndex = nextWayPointIndex + moveDirection;
        }
        
    }
    private void RegisterWaypoint()
    {
        waypointPosition = new Vector3[waypoint.Length];

        for (int i = 0; i < waypoint.Length; i++)
        {
            waypointPosition[i] = waypoint[i].position;
        }
    }

    private IEnumerator StopRoutine(float delay)
    {
        canMove = false;
        
        yield return new WaitForSeconds(delay);

        canMove = true;
        sr.flipX = !sr.flipX;
    }
   
}

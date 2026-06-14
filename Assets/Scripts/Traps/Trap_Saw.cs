using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Trap_Saw : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float moveDelay = 1f;
    [SerializeField] private Transform[] wayPoint;
    private Vector3[] wayPointPosition; 
    public int wayPointIndex = 1;
    public int moveDirection = 1; // A-B-C -> C-B-A olarak ileri geri gelmesini saðlayacak. wayPointindex + moveDirection(+1 veya -1).
                                  // Birnevi yön belirtecek.
    private bool canMove = true;

    private Animator anim;
    private SpriteRenderer sr;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        AssignWayPointPositions();
        transform.position = wayPointPosition[0];
    }

    private void Update()
    {
        anim.SetBool("active", canMove);

        if (canMove == false)
            return;
        transform.position = Vector2.MoveTowards(transform.position, wayPointPosition[wayPointIndex], moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, wayPointPosition[wayPointIndex]) < .1f)
        {
            if (wayPointIndex == wayPointPosition.Length - 1 || wayPointIndex == 0)
            {
                moveDirection = -moveDirection;
                StartCoroutine(StopMovement(moveDelay));
                
            }

           wayPointIndex = wayPointIndex + moveDirection;

        }
       
    }
    private void AssignWayPointPositions()
    {
        // Child objelerde Trap_SawWaypoint olanlarý toplayan bir list. getcomponent"S" olma sebebi list / array olmasý.
        List<Trap_SawWaypoint> wayPointList = new List<Trap_SawWaypoint>(GetComponentsInChildren<Trap_SawWaypoint>());

        if (wayPointList.Count != wayPoint.Length)
        {
            wayPoint = new Transform[wayPointList.Count];

            for (int i = 0; i < wayPointList.Count; i++)
            {
                wayPoint[i] = wayPointList[i].transform;
            }
        }


        wayPointPosition = new Vector3[wayPoint.Length]; // LÝST EKLENMEDEN ÖNCEKÝ NOTLAR...
                                                         // Inspector yerine Startta oluþturma sebebimiz scaleable olmasý için.
                                                         // ileride 5 6 noktayý gezdirebiliriz ve tek tek uðraþmadan kendisi hallolur.
                                                         // LÝST EKLENDÝKTEN SONRAKÝ NOTLAR...
                                                         // Burada yarý otomatik hallediyoruz. Yani Transformlarý hala ekleme derdimiz var.
                                                         // Unutsak dahi eklemesi için hepsine Trap_SawWaypoint scripti atayýp
                                                         // List ile GetComponentInChildren<Trap_SawWaypoint> bulup
                                                         // tamamen otomatik ekleme sistemi geliþtiriyoruz.
        for (int i = 0; i < wayPoint.Length; i++)
        {
            wayPointPosition[i] = wayPoint[i].position;
        }
    }
    private IEnumerator StopMovement(float delay)
    {
        canMove = false;

        yield return new WaitForSeconds(delay);

        canMove = true;
        sr.flipX = !sr.flipX;
    }

}

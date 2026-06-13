using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//1- Platformun yukarý aþaðý hareketi.
//2- Platforma dokunduktan bir süre sonra platformun düþürülmesi.
//3- Oyuna canlýlýk katmak amacýyla her platformun hareketini belli bir delayle baþlatma
//4- Oyuna canlýlýk katmak amacýyla platforma basýnca içeri gömülmesi.
public class Trap_FallingPlatform : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private BoxCollider2D[] colliders;

    private Vector3[] wayPoints;
    private int wayPointIndex;

    [SerializeField] private float speed = .75f;
    [SerializeField] private float travelDistance;

    private bool canMove;

    [Header("Platform Fall Details")]
    [SerializeField] private float fallDelay = .5f;

    [SerializeField] private float impactSpeed = 3f;
    [SerializeField] private float impactDuration = .1f;
    private float impactTimer;
    private bool impactHappened;

    [Header("Recreate")]
    [SerializeField] private float cooldown = .5f;
    [SerializeField] private float growthScale = 10;
    [SerializeField] private Transform startTransform;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponents<BoxCollider2D>();
    }

    private IEnumerator Start()
    {
        startTransform = transform;
        transform.localScale = new Vector3(.3f, .3f, .3f);
        SetupWayPoints();
        float randomDelay = Random.Range(0, .6f);
        yield return new WaitForSeconds(randomDelay);
        ActivatePlatform();
    }
    private void Update()
    {
        if (transform.localScale.x < startTransform.localScale.x)
            transform.localScale = Vector3.Lerp(transform.localScale, startTransform.localScale, growthScale * Time.deltaTime);
        HandleMovement();
        HandleImpact();
    }

    private void HandleImpact()
    {
        if (impactTimer < 0)
            return;

        impactTimer -= Time.deltaTime;

        transform.position = 
            Vector2.MoveTowards(transform.position, transform.position + (Vector3.down * 10), impactSpeed * Time.deltaTime);
    }

    private void ActivatePlatform() => canMove = true;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (impactHappened)
            return;

        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            Invoke(nameof(SwitchOffPlatform), fallDelay);
            impactTimer = impactDuration;
            impactHappened = true;
            DestroyMe();
        }
    }
    private void SetupWayPoints()
    {
        wayPoints = new Vector3[2];
        float yOffset = travelDistance / 2; //toplam gidilen yolu 2ye bölüp + - þeklinde alacaðýz.

        wayPoints[0] = transform.position + new Vector3(0, yOffset, 0);
        wayPoints[1] = transform.position + new Vector3(0, -yOffset, 0);

    }

    private void HandleMovement()
    {
        if (canMove == false)
            return;

        transform.position = Vector2.MoveTowards(transform.position, wayPoints[wayPointIndex], speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, wayPoints[wayPointIndex]) < .1f)
        {
            wayPointIndex++;

            if (wayPointIndex >= wayPoints.Length)
                wayPointIndex = 0;
        }
        
    }
    private void SwitchOffPlatform()
    {
        //canMove kapanmalý cd'ler kapanmalý anim triggerý tetiklenmeli rb kinematic'ten dynamic olmalý.

        anim.SetTrigger("deactivate");
        canMove = false;

        rb.isKinematic = false;
        rb.gravityScale = 3.5f;
        rb.drag = 5f;


        foreach (BoxCollider2D collider in colliders)
            collider.enabled = false;
            
    }

    private void DestroyMe()
    {
        GameObject platformPrefab = GameManager.instance.platformPrefab;
        GameManager.instance.RecreateGameObject(platformPrefab, transform, cooldown);
        Destroy(gameObject,5f);
    }
}

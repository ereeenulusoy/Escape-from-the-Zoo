using UnityEngine;

public class Trap_FireButton : MonoBehaviour
{
    public Animator buttonAnim;
    private Trap_Fire trapFire;

    private void Awake()
    {
        buttonAnim = GetComponent<Animator>();
        trapFire = GetComponentInParent<Trap_Fire>();
    }

    private void Start()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            trapFire.SwitchOffFire();
        }
    }

}

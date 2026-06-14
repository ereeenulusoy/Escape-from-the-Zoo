using System.Collections;
using UnityEngine;

public class Trap_Fire : MonoBehaviour
{

    [SerializeField] private float offDuration;
    [SerializeField] private Trap_FireButton fireButton;
    private Animator anim;
    private CapsuleCollider2D fireCollider;
    private bool isActive;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        fireCollider = GetComponent<CapsuleCollider2D>();
        fireButton = GetComponentInChildren<Trap_FireButton>();
    }

    private void Start()
    {
        if (fireButton == null)
            Debug.LogWarning("There is no button to turn " + gameObject.name + " off.");

        SetFire(true);
    }
    private void SetFire(bool active)
    {
        isActive = active;
        anim.SetBool("active", active);
        fireButton.buttonAnim.SetBool("active", active);
        fireCollider.enabled = active;
    }

    public void SwitchOffFire()
    {
        if (isActive == false)
            return;
        StartCoroutine(FireCoroutine());
    }

    private IEnumerator FireCoroutine()
    {
        SetFire(false);
        yield return new WaitForSeconds(offDuration);
        SetFire(true);
    }
}

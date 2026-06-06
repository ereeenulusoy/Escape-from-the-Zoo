using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Trampoline : MonoBehaviour
{
    private Player player;
    private Animator anim;

    [SerializeField] private float pushPower;
    [SerializeField] private float pushDuration;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        player = collision.GetComponent<Player>();

        if (player != null)
        {
            anim.SetTrigger("activate");
            player.Push(transform.up * pushPower, pushDuration);
        }
    }
}

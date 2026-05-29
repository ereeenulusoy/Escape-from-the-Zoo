using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Trap_Trampoline : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private float pushPower;
    [SerializeField] private float duration = .5f;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player != null)
        {
            anim.SetTrigger("activate");
            player.Push(transform.up * pushPower, duration);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Animator anim;

    private bool active;
    private bool canReactivate;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void Start()
    {
        canReactivate = GameManager.instance.canReactivateCheckpoints;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(active && !canReactivate)
            return;

        Player player = collision.gameObject.GetComponent<Player>();
        
        if(player != null)
        {
            ActivateCheckPoint();
         
        }
    }

    private void ActivateCheckPoint()
    {
        active = true;
        anim.SetTrigger("activate");
        GameManager.instance.UpdateSpawnPoint(transform);
    }
}

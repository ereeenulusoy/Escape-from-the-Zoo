using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Arrow : Trap_Trampoline
{
    [Header("Arrow Additional Infos")]
    [SerializeField] private float rotationSpeed = 120f;
    [SerializeField] private float recreateDelay = 1f;
    [SerializeField] private bool rotatingRight = true;
    private float direction = -1;
    [Space]
    [SerializeField] private float scaleUpSpeed = 10;
    [SerializeField] private Vector3 targetScale;


    private void Start()
    {
        transform.localScale = new Vector3(.3f, .3f, .3f);
    }


    private void Update() //Trampoline'deki Update'den baðýmsýz.
    {
        HandleRecreateScaleUp();
        HandleRotation();
    }

    private void HandleRecreateScaleUp()
    {
        if (transform.localScale.x < targetScale.x)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, scaleUpSpeed * Time.deltaTime);
        }
    }

    private void HandleRotation()
    {
        direction = rotatingRight ? -1 : 1;     //-1 olursa saða +1 olursa sola döner.

        transform.Rotate(0, 0, (rotationSpeed * direction) * Time.deltaTime);
    }

    private void DestroyMe()
    {
        GameObject arrowPrefab = GameManager.instance.arrowPrefab;//GameManager'a kaydedilen arrow prefabi.
        GameManager.instance.RecreateObject(arrowPrefab, transform.position, recreateDelay);
        Destroy(gameObject);
    }
}

using Unity.VisualScripting;
using UnityEngine;

public class Trap_Arrow : Trap_Trampoline
{
    [SerializeField] private float rotationSpeed = 120f;
    [SerializeField] private bool rotatingRight = true;
    private int rotatingDirection = 1;


    [Header("Recreate")]
    [SerializeField] private float cooldown = .5f;
    [SerializeField] private float growthScale = 10;
    [SerializeField] private Vector3 normalScale;


    private void Start()
    {
        normalScale = transform.localScale;
        transform.localScale = new Vector3(.3f, .3f, .3f);   
    }

    private void Update()
    {
        if (transform.localScale.x < normalScale.x)
          transform.localScale = Vector3.Lerp(transform.localScale, normalScale, growthScale * Time.deltaTime);

        rotatingDirection = rotatingRight ? 1 : -1;

        transform.Rotate(0, 0, - (rotationSpeed * rotatingDirection) * Time.deltaTime);
    }

    private void DestroyMe()
    {
        GameObject arrowPrefab = GameManager.instance.arrowPrefab;
        GameManager.instance.RecreateGameObject(arrowPrefab, transform, cooldown);
        Destroy(gameObject);
    }
}

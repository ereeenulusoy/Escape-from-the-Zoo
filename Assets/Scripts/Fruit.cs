using UnityEngine;

public enum FruitType {Apple,Banana,Cherry,Kiwi,Melon,Orange,Pineapple,Strawberry}

public class Fruit : MonoBehaviour
{
    private GameManager gameManager;
    private Animator anim;

    public FruitType fruitType;
    [SerializeField] private GameObject pickupVfx;


    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        gameManager = GameManager.instance;
        SetRandomFruitIfNeeded();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            gameManager.AddFruit();
            Destroy(gameObject);

            GameObject pickupFx = Instantiate(pickupVfx , transform.position, Quaternion.identity);
            //Instantiate ettiðimiz fx'i tanýmlamamýz onu destroy edebilmemize olanak tanýr.

        }
    }

    private void SetRandomFruitIfNeeded()
    {
        if (gameManager.SetRandomFruit() == false)
        {
            anim.SetFloat("fruitIndex", (int)fruitType);
            return;
        }
        anim.SetFloat("fruitIndex", Random.Range(0, 8));
    }
}

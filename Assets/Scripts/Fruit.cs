using UnityEngine;

public enum FruitType
{
    Apple,
    Banana,
    Cherry,
    Kiwi,
    Melon,
    Orange,
    Pineapple,
    Strawberry
}

public class Fruit : MonoBehaviour
{
    private GameManager gameManager;
    private Animator anim;

    public FruitType fruitType;

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
            Destroy(gameObject, .2f);
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

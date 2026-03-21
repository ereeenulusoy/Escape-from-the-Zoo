using UnityEngine;

public enum FruitType
{
    Apple,
    Bananas,
    Cherries,
    Kiwi,
    Melon,
    Orange,
    Pineapple,
    Strawberry
}

public class Fruit : MonoBehaviour
{
    public FruitType fruitType;
    private GameManager gameManager;
    private Animator anim;
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        gameManager = GameManager.instance;
        SetRandomFruit();
    }

    private void SetFruitVisualType()
    {
        anim.SetFloat("fruitIndex", (int)fruitType);
    }
    private void SetRandomFruit()
    {
        if (!gameManager.SetRandomFruits())
        {
            SetFruitVisualType();
            return;
        }

        int randomIndex = Random.Range(0, 8);
        anim.SetFloat("fruitIndex", randomIndex);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            gameManager.CollectFruit();
            Destroy(gameObject, 0.2f);
        }
    }
}

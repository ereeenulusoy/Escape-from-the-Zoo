using UnityEngine;

public class Fruit : MonoBehaviour
{
    private GameManager gameManager;
    private Animator anim; //meyvenin animatörüne baðlanýr.

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }
    private void Start()
    {
        gameManager = GameManager.instance;//startta çaðrýlmasýnýn nedeni öncelikle instance'ýn awake'de dolmasýný bekliyoruz.
        SetRandomFruit();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            gameManager.AddFruits(); // gamemanager'da yönetmemizin sebebi eðer fruitte vs yapmýþ olsak
                                     // data kaybýna neden olabilirdi.
            Destroy(gameObject);
        }
    }

    private void SetRandomFruit()
    {
        int randomIndex = Random.Range(0, 8); // it says minimum is inclusive, max is exclusive(we dont add last number.)
        //so, we need to write 8 instead of 7. because if we write 7 as last number, its not going to be included.
        anim.SetFloat("fruitIndex", randomIndex);
    }
}

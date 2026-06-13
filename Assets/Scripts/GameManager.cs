using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Player")]
    public Player player;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float respawnDuration;

    [Header("Checkpoints")]
    public bool canReactivateCheckpoints;

    [Header("Fruits")]
    public bool fruitsAreRandom;
    public int fruitsCollected;
    public int totalFruits;

    [Header("Traps")]
    public GameObject arrowPrefab;
    public GameObject platformPrefab;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        ClaimTotalFruitCount();
    }
    private void ClaimTotalFruitCount()
    {
        Fruit[] fruits = FindObjectsByType<Fruit>(FindObjectsSortMode.None);
        totalFruits = fruits.Length;
    }

    public void UpdateSpawnPoint(Transform newCheckpoint) => respawnPoint = newCheckpoint;
    public void RespawnPlayer() => StartCoroutine(RespawnRoutine());

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDuration);
        GameObject newPlayer = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);
        player = newPlayer.GetComponent<Player>(); // player = newPlayer yazamama sebebimiz player'ýn GameObject türünde olmasýdýr.
                                                   // GetComponent<Player>() ile GameObject'in Player scriptini alýyoruz.
                                                   //PEKÝ NEDEN DÝREKT PLAYER'I DOLDURMUYORUZ?
                                                   //Cevap Insantiate'de saklý. Instantiate edilen her þey **"GameObject"** türündedir. 
                                                   //Tek katmanlý bir biçimde newPlayer'e gerek kalmadan doldurabiliriz ancak okunabilirlik için 2 adýmda hallediyoruz.
                                                   //player = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity).GetComponent<Player>();
    }
    public void AddFruit() => fruitsCollected++;
    public bool SetRandomFruit() => fruitsAreRandom;

    public void RecreateGameObject(GameObject prefab, Transform spawnPosition, float cooldown = 0)
    {
        StartCoroutine(RecreateCoroutine(prefab, spawnPosition, cooldown));
    }

    private IEnumerator RecreateCoroutine(GameObject prefab, Transform spawnPosition, float cooldown)
    {
        Vector3 recreatePosition = spawnPosition.position;

        yield return new WaitForSeconds(cooldown);

        GameObject newObject = Instantiate(prefab, recreatePosition, Quaternion.identity);
    }


}
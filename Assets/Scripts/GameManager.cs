using System.Collections;
using System.Collections.Generic;
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
    public GameObject fallingPlatformPrefab;

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

    public void RecreateObject(GameObject prefab, Transform spawn, float delay = 0)
    {
        StartCoroutine(RereateObjectRoutine(prefab,spawn,delay));
    }
    private IEnumerator RereateObjectRoutine(GameObject prefab, Transform spawn, float delay)
    {
        Vector3 spawnPosition = spawn.position;
        yield return new WaitForSeconds(delay);
        GameObject newObject = Instantiate(prefab,spawnPosition, Quaternion.identity);
    }
    public void AddFruit() => fruitsCollected++;
    public bool SetRandomFruit() => fruitsAreRandom;


}
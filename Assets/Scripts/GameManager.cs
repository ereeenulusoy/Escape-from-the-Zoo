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
    public int fruitsCollected;
    public bool setRandomFruit;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
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
    public bool SetRandomFruit() => setRandomFruit;

}
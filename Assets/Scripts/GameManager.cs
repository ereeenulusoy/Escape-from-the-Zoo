using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float respawnDuration;
    public Player player;

    [Header("Fruit Managment")]
    public bool fruitsHaveRandomLook;
    public int fruitsCollected;


    public static GameManager instance; // static ram'e yazýlmasýný ve diðer scriptlerde deðer atamasýna gerek kalmadan
                                        // kullanýlmasýný saðlar.
                                        // burada player'ý atarsan player da her yerden çaðrýlabilir.
    private void Awake()
    {
        if (instance == null)
            instance = this; // awake'de çaðrýldýðýnda eðer gamemanager (instance) boþsa artýk benim [bu script].
        else
            Destroy(gameObject); //eðer baþka bir instance bulunuyorsa ben kopyayým, kendimi siliyorum.
    }

    public void RespawnPlayer() => StartCoroutine(RespawnRoutine());
    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDuration);
        GameObject newPlayer = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);
        player = newPlayer.GetComponent<Player>();
    }
    public void AddFruits() => fruitsCollected++;
    public bool FruitsHaveRandomLook() => fruitsHaveRandomLook; // getter methodu.
}



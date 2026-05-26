using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Player player;

    [Header("Fruits")]
    public int fruitsCollected;
    public bool setRandomFruit;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddFruit()
    {
        fruitsCollected++;
    }

    public bool SetRandomFruit() => setRandomFruit;

}
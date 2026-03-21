using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Player player;

    public int fruitsCollected;

    public bool setRandomFruits = true;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void CollectFruit() => fruitsCollected++;

    public bool SetRandomFruits() => setRandomFruits;
}

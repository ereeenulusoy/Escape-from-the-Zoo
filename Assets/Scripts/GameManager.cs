using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // static ram'e yazýlmasýný ve diðer scriptlerde deðer atamasýna gerek kalmadan kullanýlýr.
                                        // burada player'ý atarsan player da her yerden çaðrýlabilir.
    public Player player;

    private void Awake()
    {
        if(instance == null)
            instance = this; // awake'de çaðrýldýðýnda eðer gamemanager (instance) boþsa artýk benim [bu script].
        else
            Destroy(gameObject); //eðer baþka bir instance bulunuyorsa ben kopyayým, kendimi siliyorum.
    }

}



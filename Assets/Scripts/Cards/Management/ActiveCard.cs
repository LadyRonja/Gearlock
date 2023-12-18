using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveCard : MonoBehaviour
{

    public static ActiveCard Instance;
    public Card cardBeingPlayed;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }
}

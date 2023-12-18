using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCardPlayer : MonoBehaviour
{
    public GameObject cardPrefab;
    public Card activeCard;
   

    private void Start()
    {
        activeCard = Instantiate(cardPrefab).GetComponent<Card>();
        
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            activeCard.Play();
            
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCardPlayer : MonoBehaviour
{
    public GameObject cardPrefab;
    public PlayCard activeCard;
   

    private void Start()
    {
        activeCard = Instantiate(cardPrefab).GetComponent<PlayCard>();
        
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            activeCard.Play();
            
        }
    }
}

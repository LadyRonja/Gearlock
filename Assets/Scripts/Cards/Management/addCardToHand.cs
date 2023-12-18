using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class addCardToHand : MonoBehaviour
{
    public GameObject card;


    public void addCard()
    {
        Instantiate(card, HandPanel.Instance.transform);
    }
}

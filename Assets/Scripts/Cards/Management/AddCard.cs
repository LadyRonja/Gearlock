using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCard : MonoBehaviour
{
    public GameObject handParent;
    public List<GameObject> cards;
    int card;

    public static AddCard Instance;

    private void Awake()
    {
        if (Instance == null || Instance == this)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    public void AddCardToHand()
    {
        card = Random.Range(0, cards.Count);
        Instantiate(cards[card], handParent.transform);
    }
}

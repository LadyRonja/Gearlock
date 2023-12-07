using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBot : Unit
{
    public GameObject infoTextBigBot;

    public void Start()
    {
        //infoTextBigBot.SetActive(false);
    }
    public void OnMouseEnter()
    {
        PlayCard currentCard = ActiveCard.Instance.transform.GetComponentInChildren<PlayCard>();

        infoTextBigBot.SetActive(true);

        if (currentCard != null)
        {
            if (currentCard.GetType().Equals(typeof(AttackCard)))
            {
                MouseControl.instance.Fight();
                Debug.Log("Changing cursor to Fight");
            }
        }
        
    }

    public void OnMouseExit()
    {
        MouseControl.instance.Default();
        infoTextBigBot.SetActive(false);
    }
}


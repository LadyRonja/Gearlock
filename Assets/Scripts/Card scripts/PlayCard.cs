using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCard : MonoBehaviour
{
    public bool selectCard;
    public void Play()
    {
        if (Input.GetMouseButtonDown(0)) //om man klickar på kortet väljs det
        {
            selectCard = true;
        }
    }

    //TODO kalla på kortens classer

}

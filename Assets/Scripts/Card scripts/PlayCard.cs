using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCard : MonoBehaviour
{
    public bool selectCard;
    public void Play()
    {
        if (Input.GetMouseButtonDown(0)) //om man klickar p� kortet v�ljs det
        {
            selectCard = true;
        }
    }

    //TODO kalla p� kortens classer

}

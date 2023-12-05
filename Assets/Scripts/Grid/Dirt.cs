using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dirt : MonoBehaviour
{
    public MeshRenderer gfx;
    public void OnMouseEnter()
    {
        PlayCard currentCard = ActiveCard.Instance.transform.GetComponentInChildren<PlayCard>();

        if (currentCard != null)
        {
            if (currentCard.GetType().Equals(typeof(DigCard)))
            {
                MouseControl.instance.Dig();
                Debug.Log("Changing cursor to Dig");
            }
        }
    }

    public void OnMouseExit()
    {
        MouseControl.instance.Default();
    }
}

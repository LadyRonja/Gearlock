using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dirt : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Tile myTile;
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        HoverManager.HoverTileEnter(myTile);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HoverManager.HoverTileExit(myTile);
    }
}

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
        HoverManager.CursorManagerEnter(myTile);
    }

    public void OnMouseExit()
    {
        HoverManager.CursorManagerExit();
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

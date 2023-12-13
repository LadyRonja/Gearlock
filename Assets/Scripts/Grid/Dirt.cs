using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public void Highlight()
    {
        Debug.Log($"Implement Highlight on dirt, called on by dirt from tile {myTile.name}");
        gfx.material.color = Color.black;
    }
    public void UnHighlight()
    {
        Debug.Log($"Implement UnHighlight on dirt, called on by dirt from tile {myTile.name}");
        gfx.material.color = Color.white;
    } 
}

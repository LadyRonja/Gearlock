using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dirt : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Tile myTile;
    public MeshRenderer gfx;
    public SpriteRenderer highligther;

    private void Start()
    {
        UnHighlight();
    }

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
        if (highligther != null)
            highligther.gameObject.SetActive(true);
    }
    public void UnHighlight()
    {
        if(highligther != null)
            highligther.gameObject.SetActive(false);
    } 
}

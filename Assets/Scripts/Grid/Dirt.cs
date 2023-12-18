using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dirt : MonoBehaviour
{
    public Tile myTile;
    public MeshRenderer gfx;
    public SpriteRenderer highligther;

    private void Start()
    {
        UnHighlight();
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

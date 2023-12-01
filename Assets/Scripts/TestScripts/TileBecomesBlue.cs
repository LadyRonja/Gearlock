using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileBecomesBlue : MonoBehaviour
{
  /*  public MeshRenderer myMR;
    public Color highlightColor = Color.blue;
    public int highlightRadius = 2;
    private int x;
    private int y;

    private void Start()
    {
        myMR = GetComponent<MeshRenderer>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //ToggleHighlight();
        myMR.material.color = highlightColor;
        HighlightNeighbours();
    }

    private void ToggleHighlight()
    {
        if (!myMR)
        {
            Debug.LogWarning("MeshRenderer not found!");
            return;
        }

        if (myMR.material.color == highlightColor)
        {
            myMR.material.color = Color.white;
        }
        else
        {
            myMR.material.color = highlightColor;
        }
    }

    private void HighlightNeighbours()
    {
        Tile[] allTiles = FindObjectsOfType<Tile>();

        foreach (Tile tile in allTiles)
        {
            if (tile == this) continue; // Skip the current tile

            int distanceX = Mathf.Abs(tile.x - this.x);
            int distanceY = Mathf.Abs(tile.y - this.y);

            if (distanceX <= highlightRadius && distanceY <= highlightRadius)
            {
                HighlightTile(tile);
            }
        }
    }

    private void HighlightTile(Tile tile)
    {
        if (!tile || !tile.myMR) return;

        tile.myMR.material.color = highlightColor;
    }*/
}

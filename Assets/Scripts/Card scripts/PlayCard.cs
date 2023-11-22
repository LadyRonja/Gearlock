using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayCard : MonoBehaviour
{
    public bool selectCard;

    private Tile playerTile; // Reference to the player's current tile

    protected virtual void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Get the mouse click position in world space
            Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Get the clicked tile
            Tile clickedTile = GetClickedTile(clickPosition);

            if (clickedTile != null)
            {
                // Calculate the distance between the clicked tile and the player's tile
                int dist = GetDistance(clickedTile, playerTile);
                Debug.Log("Distance to Player: " + dist);
            }
        }
    }

    protected virtual Tile GetClickedTile(Vector3 clickPosition)
    {
        // Raycast to find the clicked tile
        RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero);

        if (hit.collider != null)
        {
            Tile clickedTile = hit.collider.GetComponent<Tile>();
            return clickedTile;
        }

        return null;
    }

    protected virtual int GetDistance(Tile a, Tile b)
    {
        // Use the Pathfinding.GetDistance method to calculate distance
        return Pathfinding.GetDistance(a, b);
    }


    /*public virtual void Play()
    {
        if (Input.GetMouseButtonDown(0)) //om man klickar på kortet väljs det
        {
            selectCard = true;
        }
    }*/



}

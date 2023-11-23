using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayCard : MonoBehaviour
{
    public bool selectCard;
    public bool playCard;

    protected Tile playerTile; // Reference to the player's current tile
    protected Tile clickedTile;

    protected virtual void Update()
    {
        //TODO
        //Check if card is selected
        //if not activated (the card) return 
        
        if (Input.GetMouseButtonDown(0))
        {
            // Get the mouse click position in world space
            Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Get the clicked tile
           clickedTile = GetClickedTile(clickPosition);

            if (clickedTile != null)
            {
                // Calculate the distance between the clicked tile and the player's tile
                int dist = Pathfinding.GetDistance(clickedTile, playerTile);
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
            Tile hitTile = hit.collider.GetComponent<Tile>();
            return hitTile;
        }

        return null;
    }




    public virtual void Play()
    {
        if (Input.GetMouseButtonDown(0)) //om man klickar på kortet väljs det
        {
            selectCard = true;
            
            if (Input.GetMouseButtonUp(0))
            {
                playCard = true;
            }


        }
        
    }



}

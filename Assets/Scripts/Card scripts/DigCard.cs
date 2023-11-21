using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigCard : PlayCard
{
    public Animator digAnimation;
    
    private Grid grid; // Reference to the Grid script
    private Tile playerTile; // Reference to the player's current tile
    public LayerMask dirtLayer; // LayerMask for dirt sprites

    private void Start()
    {
        grid = Grid.Instance;
        if (grid != null)
        {
            // Assuming the player starts on the center tile (adjust as needed)
            
            //Protected??
            //playerTile = grid.tiles[grid.coloumns / 2, grid.rows / 2];
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Get the mouse click position in world space
            Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Check if the clicked tile is next to the player
            CheckAndDestroyDirt(clickPosition);
        }
    }

    private void CheckAndDestroyDirt(Vector3 clickPosition)
    {
        if (grid != null && playerTile != null)
        {
            // Get the clicked tile
            Tile clickedTile = GetClickedTile(clickPosition);

            // Check if the clicked tile is next to the player
            if (IsNextToPlayer(clickedTile))
            {
                // Check if there is a dirt sprite in the clicked tile
                Collider2D dirtCollider = Physics2D.OverlapPoint(clickPosition, dirtLayer);

                if (dirtCollider != null)
                {
                    // Play digging animation and Destroy the dirt sprite

                    //playerAnimator.SetTrigger("Digging");
                    Destroy(dirtCollider.gameObject);
                    Debug.Log("Dirt destroyed!");
                }
            }
        }
    }

    private Tile GetClickedTile(Vector3 clickPosition)
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

    private bool IsNextToPlayer(Tile tile)
    {
        if (tile != null && playerTile != null)
        {
            // Check if the clicked tile is one of the valid neighboring cells
            return Mathf.Abs(tile.x - playerTile.x) + Mathf.Abs(tile.y - playerTile.y) == 1;
        }

        return false;
    }
}

    




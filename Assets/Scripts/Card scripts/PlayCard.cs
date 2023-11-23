using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UIElements;

public abstract class PlayCard : MonoBehaviour //lägg till abstract
{
    public bool selectCard;
    public bool playCard;
    public float distance;
    public CardType currentCardType;


    //protected Tile clickedTile;
    protected UnitSelector chosenUnit;
    protected AttackCard attackCard;
    protected DigCard digCard;
    

    protected virtual void Start()
    {
        chosenUnit = GetComponent<UnitSelector>();
    }

    protected virtual void Update()
    {
        //TODO
        //Check if card is selected
        //if not activated (the card) return 

        ClickOnUnit();


    }

    public enum CardType
    {
        Dig,
        Attack
    }

    public void ClickOnUnit()
    {
        if (!selectCard)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            // Get the mouse position in screen coordinates
            Vector3 mousePosition = Input.mousePosition;

            // Cast a ray from the camera to the mouse position
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            // Perform a raycast to check for objects at the click position
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if the clicked object has a Tile component
                Tile clickedTile = hit.collider.GetComponent<Tile>();

                if (clickedTile != null)
                {
                    // Use GridManager to find the player's unit
                    Unit playerUnit = FindPlayerUnit();

                    if (playerUnit != null)
                    {
                        // Calculate the distance between the player's unit and the clicked tile
                        distance = Vector3.Distance(playerUnit.transform.position, clickedTile.transform.position);

                        // Log the distance
                        Debug.Log($"Distance to clicked tile: {distance}");
                    }
                    else
                    {
                        Debug.LogWarning("No player unit found.");
                    }
                }
                
                if (distance < 14) //om avståndet från player och clicked tile är mindre än 20 dvs 2 rutor
                {
                    playCard = true;
                    Debug.Log("You can play this card");

                    if(currentCardType == CardType.Attack)
                    {
                        attackCard.Attack();
                    }

                    if (currentCardType == CardType.Dig) 
                    {
                        
                    }

                }
                else
                {
                    playCard = false;
                    Debug.Log("You can NOT play this card");
                }
            }
        }
    }


    private Unit FindPlayerUnit()
    {
        
        GridManager gridManager = GridManager.Instance;

        if (gridManager != null && gridManager.tiles != null)
        {
            for (int x = 0; x < gridManager.tiles.GetLength(0); x++)
            {
                for (int y = 0; y < gridManager.tiles.GetLength(1); y++)
                {
                    Tile tile = gridManager.tiles[x, y];
                    if (tile != null && tile.occupant != null && tile.occupant.playerBot)
                    {
                        return tile.occupant;
                    }
                }
            }
        }

        return null;
    }

    public virtual void Play()
    {

        selectCard = true;
        Debug.Log("you have selceted a card");

    }
}

   


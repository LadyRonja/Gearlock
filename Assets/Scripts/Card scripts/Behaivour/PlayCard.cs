using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UI;
using UnityEngine.UIElements;

public abstract class PlayCard : MonoBehaviour //lägg till abstract
{
    
   

    // Raycast
    // if a tile was found
    // see if tile.occupant != null
    // set selectedUnit to tile.occupant
    // go to unit verification
    // if no unit was selected, keep looking

    //CardEffectComplete(); call this function when the card is used

    public enum CardState
    {
        Inactive,
        VerifyUnitSelection,
        SelectingUnit,
        VerifyTileSelection,
        SelectTile,
        Executing,
        Finished
    }

    public CardState myState = CardState.Inactive;
    public BotSpecialization requiredSpecialization = BotSpecialization.None;
    public Tile selectedTile = null;
    public Unit selectedUnit = null;

    

    public bool selectCard;
    public bool playCard;
    public int distance;

    //protected Tile clickedTile;
    protected AttackCard attackCard;
    protected DigCard digCard;
    protected Unit unitToExecuteCardBehaviour;

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        switch (myState)
        {
            case CardState.Inactive:
                selectedTile = null;
                selectedUnit = UnitSelector.Instance.selectedUnit;
                break;
            case CardState.VerifyUnitSelection:
                VerifyUnitSelection();
                //Can the selected bot play this
                break;
            case CardState.SelectingUnit:
                SelectUnit();
                //choose a bot
                break;
            case CardState.VerifyTileSelection:
                // Same as unit verification but for tiles
                //is the tile next to player RAYCAST
                VerifyTileSelection();
                break;
            case CardState.SelectTile:
                // Same as unit selection but for tiles
                //select a tile that the bot will excecute its behaviour 
                break;
            case CardState.Executing:

                //PlayCard(); // excecute the thing on the card
                myState = CardState.Finished;

                break;
            case CardState.Finished:
                // Let the card manager know the card is finished and can go to discard
                break;
            default:
                // Error - Reached default of switch-state-machine
                // Go inactive
                break;
        }


    }

    protected virtual void VerifyUnitSelection()
    {
        // Verify that there is a selected unit, and that it's legal
        // Use UnitSelectior.Instance

        //can Player bot play this card 

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
                // Check if the clicked object has a Unit component
                Unit clickedUnit = hit.collider.GetComponent<Unit>();
                Debug.Log("Unit clicked");

                if (clickedUnit != null)
                {
                    // Update the selected unit in UnitSelector
                    UnitSelector.Instance.UpdateSelectedUnit(clickedUnit);
                    // Change state to verify tile selection
                    myState = CardState.VerifyTileSelection;
                }

                else
                {
                    myState = CardState.SelectingUnit;
                }
            }

            // If valid
            //myState = CardState.VerifyTileSelection;
            // else
            //myState = CardState.SelectingUnit;
        }
    }

    protected virtual void SelectUnit()
    {
        // Have the player select a unit/ bot
        // Remember to update the UnitSelector.Instance.UpdateSelectedUnit()
        // Once a unit is selcted, set myState to verify it.
       
       

    }

    protected virtual void VerifyTileSelection()
    {
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
                Debug.Log("Tile clicked");

                if (clickedTile != null)
                {
                    //verifiedUnit = VerifyUnitSelection;
                }
            }
        }


            // if valid 
            myState = CardState.VerifyTileSelection;
        //else
        myState = CardState.SelectTile;
    }

    protected virtual void SelectTile()
    {

    }





    public void ClickOnTile()
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
                        unitToExecuteCardBehaviour = FindPlayerUnit();

                        if (unitToExecuteCardBehaviour != null)
                        {
                            // Calculate the distance between the player's unit and the clicked tile
                            //distance = Vector3.Distance(playerUnit.transform.position, clickedTile.transform.position);
                            distance = Pathfinding.GetDistance(unitToExecuteCardBehaviour.standingOn, clickedTile);

                            // Log the distance
                            Debug.Log($"Distance to clicked tile: {distance}");
                        }
                        else
                        {
                            Debug.LogWarning("No player unit found.");
                        }
                    }
                
                    if (distance == 1) //om avståndet från player och clicked tile är mindre än 20 dvs 2 rutor
                    {
                        playCard = true;
                        Debug.Log("You can play this card");

                        if (playCard == true)
                        {
                            ExecuteBehaivour(clickedTile, unitToExecuteCardBehaviour);
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


    protected Unit FindPlayerUnit()
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
        Debug.Log("you have selceted a card: " + this.gameObject.name);
    }


    public abstract void ExecuteBehaivour(Tile onTile, Unit byUnit);
}

   


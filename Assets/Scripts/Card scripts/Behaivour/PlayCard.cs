using Unity.VisualScripting;
using UnityEngine;
public enum CardState
{
    Inactive,
    VerifyUnitSelection,
    SelectingUnit,
    SelectingTile,
    VerifyTileSelection,
    Executing,
    Finished
}

public abstract class PlayCard : MonoBehaviour
{
    public int range = 1;
    [Space]
    public BotSpecialization requiredSpecialization = BotSpecialization.None;
    public bool canTargetDirtTiles = false;
    public bool canTargetOccupiedTiles = true;
    public bool goesToDiscardAfterPlay = true;

    [HideInInspector] public CardState myState = CardState.Inactive;
    [HideInInspector] public Tile selectedTile = null;
    [HideInInspector] public Unit selectedUnit = null;

    protected virtual void Start()
    {
        myState = CardState.Inactive;
    }

    protected virtual void Update()
    {
        // Depending on the state of the card, determine behaivor
        switch (myState)
        {
            case CardState.Inactive:
                // Essentially do nothing.
                selectedTile = null;
                selectedUnit = UnitSelector.Instance.selectedUnit;

                break;
            case CardState.VerifyUnitSelection:
                // Verify if the selected unit is legal
                VerifyUnitSelection();

                break;
            case CardState.SelectingUnit:
                // Select a new unit
                // This state is only entered after a failed verification
                SelectUnit();

                break;
            case CardState.SelectingTile:
                // Select a tile to attempt to use the card on
                SelectTile();

                break;
            case CardState.VerifyTileSelection:
                // Verify if the selected tile is legal
                VerifyTileSelection();

                break;
            case CardState.Executing:
                // Execute the cards behaivour
                ExecuteBehaivour(selectedTile, selectedUnit);
                myState = CardState.Finished;
                DEBUGCardStateUI.Instance.DEBUGUpdateUI(CardState.Executing, "Playing card!");

                break;
            case CardState.Finished:
                // Let the card manager know the card is finished and can go to discard
                CardManager.Instance.CardEffectComplete();
                MovementManager.Instance.takingMoveAction = true;
                myState = CardState.Inactive;
                DEBUGCardStateUI.Instance.DEBUGUpdateUI(CardState.Inactive, "--");

                break;
            default:
                Debug.LogError("Reached end of state-machine, new case not added to switch?");
                Debug.Log("Going inactive");
                myState = CardState.Inactive;

                break;
        }
    }

    protected virtual void VerifyUnitSelection()
    {
        // If no unit is selected - go to select unit.
        if(selectedUnit == null)
        {
            Debug.Log("No unit selected, please select a unit");
            myState = CardState.SelectingUnit;
            DEBUGCardStateUI.Instance.DEBUGUpdateUI(CardState.VerifyUnitSelection, "Select a Unit.");
            return;
        }

        // Player can only play cards on player-cntrolled units
        if(selectedUnit.playerBot == false)
        {
            Debug.Log("Selected unit is not controlled by the player");
            selectedUnit = null;
            myState= CardState.SelectingUnit;
            DEBUGCardStateUI.Instance.DEBUGUpdateUI(CardState.VerifyUnitSelection, "Select a friendly Unit");
            return;
        }

        // If the card has a special requirment, make sure the selected bot clears that requirment
        if(requiredSpecialization != BotSpecialization.None)
        {
            if(selectedUnit.mySpecialization != requiredSpecialization) 
            {
                Debug.Log("Selected unit does not match special requirment");
                selectedUnit = null;
                myState = CardState.SelectingUnit;
                DEBUGCardStateUI.Instance.DEBUGUpdateUI(CardState.VerifyUnitSelection, "Select a unit that can use that card");
                return;
            }
        }

        // If reached this bit of the code, the bot is verified and get's to cast the card.
        myState = CardState.SelectingTile;
        DEBUGCardStateUI.Instance.DEBUGUpdateUI(CardState.SelectingTile, "Select a tile");
    }

    protected virtual void SelectUnit()
    {
        // Click on a tile
        // If it has an occupant
        // That is now the selected unit
        // Send to verify
        if (Input.GetMouseButtonDown((int)MouseButton.Left))
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.TryGetComponent<Tile>(out Tile clickedTile))
                    {
                        if(clickedTile.occupant != null)
                        {
                            selectedUnit = clickedTile.occupant;
                            UnitSelector.Instance.UpdateSelectedUnit(selectedUnit);
                            myState = CardState.VerifyUnitSelection;
                            Debug.Log("New unit selected: " + selectedUnit.unitName);
                            return;
                        }
                    }
                }
            }
            Debug.Log("No unit selected");
        }
    }


    protected virtual void SelectTile()
    {
        // Click on a tile
        // That is now the selected tile
        // Send to verify
        if (Input.GetMouseButtonDown((int)MouseButton.Left))
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.TryGetComponent<Tile>(out Tile clickedTile))
                    {
                        selectedTile = clickedTile;
                        myState = CardState.VerifyTileSelection;
                        Debug.Log("New tile selected: " + selectedTile.transform.name);
                        return;
                    }
                }
            }
            Debug.Log("No tile selected");
        }
    }

    protected virtual void VerifyTileSelection()
    {
       // If no selected tile is passed
       if(selectedTile == null)
       {
            Debug.LogError("No tile selected for verification - error in structure");
            myState= CardState.SelectingTile;
            DEBUGCardStateUI.Instance.DEBUGUpdateUI(CardState.VerifyTileSelection, "Contact a programmer, no tile selected");
            return;
       }

       // Check if the tile contains dirt
       if(!canTargetDirtTiles && selectedTile.containsDirt)
       {
            Debug.Log("Tile cannot contain dirt for this card");
            myState = CardState.SelectingTile;
            DEBUGCardStateUI.Instance.DEBUGUpdateUI(CardState.VerifyTileSelection, "This card can't be played on a tile with dirt");
            return;
       }

        // Check if the tile contains a unit
        if (!canTargetOccupiedTiles && selectedTile.occupant != null)
        {
            Debug.Log("Tile cannot contain a unit for this card");
            myState = CardState.SelectingTile;
            DEBUGCardStateUI.Instance.DEBUGUpdateUI(CardState.VerifyTileSelection, "This card can't be played on a tile with a unit");
            return;
        }

        // Check if the tile is in range
        if (range > 0)
       {
            int dist = Pathfinding.GetDistance(selectedTile, selectedUnit.standingOn);
            
            // Unless range is 0, cannot target own tile
            if(!(dist <= range && dist != 0))
            {
                Debug.Log("Tile is out of range, unless range is 0 it can not target the same tile as the user");
                DEBUGCardStateUI.Instance.DEBUGUpdateUI(CardState.VerifyTileSelection, "Out of range, can't play on self");
                myState = CardState.SelectingTile;
                return;
            }
       }

        // If reached this bit of the code, the card is valid and can be executed
        myState = CardState.Executing;
    }

    public virtual void Play()
    {
        Debug.Log("card is being played:" + this.name);
        myState = CardState.VerifyUnitSelection;
        DEBUGCardStateUI.Instance.DEBUGUpdateUI(CardState.SelectingUnit, "Select a unit");
        MovementManager.Instance.takingMoveAction = false;
    }


    public abstract void ExecuteBehaivour(Tile onTile, Unit byUnit);

    public void CancelPlay()
    {
        Debug.Log("Card is returned to inactive play");
        DEBUGCardStateUI.Instance.DEBUGUpdateUI(CardState.Inactive, "--");
        selectedTile = null;
        selectedUnit= null;
        myState = CardState.Inactive;
        MovementManager.Instance.takingMoveAction = true;
    } 
}

   


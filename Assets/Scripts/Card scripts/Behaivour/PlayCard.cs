using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.UI;

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
    [Header("Info displayed to player")]
    public string cardName = "--";
    public Sprite cardFrame;
    public Sprite illustration;
    [TextArea(6, 6)] 
    public string cardDescription;
    public int range = 1;

    [Header("Card restrictions")]
    public BotSpecialization requiredSpecialization = BotSpecialization.None;
    public bool hasToTargetDirtTiles = true;
    public bool canNotTargetDirtTiles = false;
    public bool hasToTargetOccupiedTiles = true;
    public bool canNotTargetOccupiedTiles = false;
    public bool goesToDiscardAfterPlay = true;

    private bool unitsHighligthed = false;
    private bool tilesHighligthed = false;


    [HideInInspector] public CardState myState = CardState.Inactive;
    [HideInInspector] public Tile selectedTile = null;
    [HideInInspector] public Unit selectedUnit = null;

    protected virtual void Start()
    {
        myState = CardState.Inactive;

        if(canNotTargetOccupiedTiles && hasToTargetOccupiedTiles)
            Debug.LogError($"WARNING: {cardName} both has to and is unable to target occupied Tiles!" );
        if (canNotTargetDirtTiles && hasToTargetDirtTiles)
            Debug.LogError($"WARNING: {cardName} both has to and is unable to target dirt covered Tiles!");       
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
                unitsHighligthed = false;
                tilesHighligthed = false;

                break;
            case CardState.VerifyUnitSelection:
                // Verify if the selected unit is legal
                VerifyUnitSelection();

                break;
            case CardState.SelectingUnit:
                // Select a new unit
                // This state is only entered after a failed verification
                if(!unitsHighligthed)
                {
                    HighlightUnits();
                    unitsHighligthed = true;
                }
                SelectUnit();

                break;
            case CardState.SelectingTile:
                // Select a tile to attempt to use the card on
                if(!tilesHighligthed)
                {
                    GridManager.Instance.UnhighlightAll();
                    selectedUnit.standingOn.Highlight(Color.blue);
                    HighlightTiles();
                    tilesHighligthed = true;
                }
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
                GridManager.Instance.UnhighlightAll();
                if(UnitSelector.Instance.selectedUnit != null)
                        UnitSelector.Instance.UpdateSelectedUnit(UnitSelector.Instance.selectedUnit);
                tilesHighligthed = false; 
                unitsHighligthed = false;
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
            HandleIllegalSelection("No unit selected, please select a unit",
                                    "Select a Unit.");

            return;
        }

        // Player can only play cards on player-cntrolled units
        if(selectedUnit.playerBot == false)
        {
            HandleIllegalSelection("Selected unit is not controlled by the player",
                                    "Select a friendly Unit");

            return;
        }

        // If the card has a special requirment, make sure the selected bot clears that requirment
        if(requiredSpecialization != BotSpecialization.None)
        {
            if(selectedUnit.mySpecialization != requiredSpecialization) 
            {
                HandleIllegalSelection("Selected unit does not match special requirment",
                                        "Select a unit that can use that card");
                return;
            }
        }

        // If reached this bit of the code, the bot is verified and get's to cast the card.
        myState = CardState.SelectingTile;
        DEBUGCardStateUI.Instance.DEBUGUpdateUI(CardState.SelectingTile, "Select a tile");

        void HandleIllegalSelection(string errorMessage, string cardStateText)
        {
            Debug.Log(errorMessage);
            selectedUnit = null;
            myState = CardState.SelectingUnit;
            DEBUGCardStateUI.Instance.DEBUGUpdateUI(CardState.VerifyUnitSelection, cardStateText);
            GridManager.Instance.UnhighlightAll();
            unitsHighligthed = false;
        }
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
                    Tile clickedTile = null;

                    if(hit.collider.gameObject.TryGetComponent<Unit>(out Unit clickedUnit)) 
                    {
                        clickedTile = clickedUnit.standingOn;
                        PickUnit(clickedTile);
                        return;
                    }

                    if (hit.collider.gameObject.TryGetComponent<Tile>(out clickedTile))
                    {
                        if(clickedTile.occupant != null)
                        {
                            PickUnit(clickedTile);
                            return;
                        }
                    }
                }
            }

            void PickUnit(Tile onTile)
            {
                selectedUnit = onTile.occupant;
                UnitSelector.Instance.UpdateSelectedUnit(selectedUnit);
                myState = CardState.VerifyUnitSelection;
                Debug.Log("New unit selected: " + selectedUnit.unitName);
            }

            Debug.Log("No unit selected");
        }
    }

    protected virtual void HighlightUnits()
    {
        // Find legal units
        List<Unit> legalUnits = new();
        if (requiredSpecialization != BotSpecialization.None)
            legalUnits = UnitStorage.Instance.playerUnits.Where(u => u.mySpecialization == requiredSpecialization).ToList();
        else
            legalUnits = UnitStorage.Instance.playerUnits;

        // If no legal units, put card back in hand
        if (legalUnits.Count == 0)
        {
            Debug.Log("No legal unit found, removing card from played");
            CancelPlay();
            CardManager.instance.ClearActiveCard();
            return;
        }

        // Highlight the units
        foreach (Unit u in legalUnits)
        {
            u.standingOn.Highlight();
        }
    }

    protected virtual void HighlightTiles()
    {
        // Find legal tiles
        List<Tile> legalTiles = new();
        /*List<Tile> allTiles = new();

        foreach (Tile t in GridManager.Instance.tiles)
            allTiles.Add(t);*/

        legalTiles.AddRange(GridManager.Instance.tiles);
        List<Tile> tilesToRemove = new();
        // Remove illegal tiles
        foreach(Tile t in legalTiles)
        {
            // Dirt Check
            if(hasToTargetDirtTiles && !t.containsDirt)
                tilesToRemove.Add(t);

            if(canNotTargetDirtTiles && t.containsDirt) 
                tilesToRemove.Add(t);

            // Unit Check
            if(hasToTargetOccupiedTiles && !t.occupied)
                tilesToRemove.Add(t);

            if(canNotTargetOccupiedTiles && t.occupied)
                tilesToRemove.Add(t);

            // Range check
            if (Pathfinding.GetDistance(t, selectedUnit.standingOn) > range || Pathfinding.GetDistance(t, selectedUnit.standingOn) == 0)
                tilesToRemove.Add(t);
        }

        foreach (Tile t in tilesToRemove)
        {
            if(legalTiles.Contains(t))
                legalTiles.Remove(t);
        }

        /*
        if(canTargetDirtTiles)
        {
            List<Tile> tilesToAdd = allTiles.Where( t => t.containsDirt 
                                                    && Pathfinding.GetDistance(t, selectedUnit.standingOn) <= range 
                                                    && t.occupant != selectedUnit).ToList();

            legalTiles.AddRange(tilesToAdd);
        }

        if(canTargetOccupiedTiles)
        {
            List<Tile> tilesToAdd = allTiles.Where(t => t.occupied
                                                    && Pathfinding.GetDistance(t, selectedUnit.standingOn) <= range
                                                    && t.occupant != selectedUnit).ToList();

            legalTiles.AddRange(tilesToAdd);
        }
        else
        {
            List<Tile> tilesToAdd = allTiles.Where(t => !t.occupied
                                                    && Pathfinding.GetDistance(t, selectedUnit.standingOn) <= range).ToList();

            legalTiles.AddRange(tilesToAdd);
        }

        // Remove illegal tiles
        List<Tile> tilesToRemove = new();
        foreach(Tile t in legalTiles)
        {
            if(!canTargetDirtTiles && t.containsDirt)
            {
                tilesToRemove.Add(t);
            }
            
            if(canTargetDirtTiles && !t.containsDirt)
            {
                tilesToRemove.Add(t);
            }
        
        }*/

        // If no legal tiles, put card back in hand
        if(legalTiles.Count == 0)
        {
            Debug.Log("No legal tile found, removing card from played");
            CancelPlay();
            CardManager.instance.ClearActiveCard();
            return;
        }

        // Highlight the tiles
        foreach (Tile t in legalTiles)
        {
            t.Highlight();
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
                    Tile clickedTile = null;

                    if(hit.collider.gameObject.TryGetComponent<Dirt>(out Dirt clickedDirt))
                    {
                        clickedTile = clickedDirt.myTile;
                        PickTile(clickedTile);
                        return;
                    }

                    if (hit.collider.gameObject.TryGetComponent<Unit>(out Unit clickedUnit))
                    {
                        clickedTile = clickedUnit.standingOn;
                        PickTile(clickedTile);
                        return;
                    }

                    if (hit.collider.gameObject.TryGetComponent<Tile>(out clickedTile))
                    {
                        PickTile(clickedTile);
                        return;
                    }
                }
            }

            void PickTile(Tile thisTile)
            {
                selectedTile = thisTile;
                myState = CardState.VerifyTileSelection;
                Debug.Log("New tile selected: " + selectedTile.transform.name);
            }
            Debug.Log("No tile selected");
        }
    }

    protected virtual void VerifyTileSelection()
    {
       // If no selected tile is passed
       if(selectedTile == null)
        {
            HandleIllegalSelection("No tile selected for verification - error in structure",
                                    "Contact a programmer, no tile selected");
            return;
       }

       // Check if the tile contains dirt
       if(hasToTargetDirtTiles && !selectedTile.containsDirt)
       {
            HandleIllegalSelection("Tile must contain dirt for this card",
                                    "This card must be played on a tile with dirt");
            return;
       }

       if(canNotTargetDirtTiles && selectedTile.containsDirt)
       {
            HandleIllegalSelection("Tile cannot contain dirt for this card",
                                    "This card can not be played on a tile with dirt");
            return;
       }

        // Check if the tile contains a unit
        if (hasToTargetOccupiedTiles && !selectedTile.occupied)
        {
            HandleIllegalSelection("Tile must contain a unit for this card",
                                    "This card has to be played on a tile with a unit on it");
            return;
        }

        if(canNotTargetOccupiedTiles && selectedTile.occupied)
        {
            HandleIllegalSelection("Tile can not contain a unit for this card",
                                    "This card has to be played on a tile without a unit on it");
            return;
        }


        // Check if the tile is in range
        if (range > 0)
        {
            int dist = Pathfinding.GetDistance(selectedTile, selectedUnit.standingOn);
            
            // Unless range is 0, cannot target own tile
            if(!(dist <= range && dist != 0))
            {
                HandleIllegalSelection("Tile is out of range, unless range is 0 it can not target the same tile as the user",
                                        "Tile selected was not within card range");
                return;
            }
       }

        // If reached this bit of the code, the card is valid and can be executed
        myState = CardState.Executing;

        void HandleIllegalSelection(string errorMessage, string cardStateText)
        {
            Debug.Log(errorMessage);
            DEBUGCardStateUI.Instance.DEBUGUpdateUI(CardState.VerifyTileSelection, cardStateText);
            myState = CardState.SelectingTile;
            GridManager.Instance.UnhighlightAll();
            tilesHighligthed = false;
        }
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
        GridManager.Instance.UnhighlightAll();
        UnitSelector.Instance.UpdateSelectedUnit(UnitSelector.Instance.selectedUnit);
    } 
}

   


using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine.EventSystems;
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

public abstract class Card : MonoBehaviour
{
    public enum CardType
    {
        Dig,
        Attack,
        Attack2x,
        DiggerBot,
        FighterBot,
        Dynamite
    }
    [Header("Info displayed to player")]
    public string cardName = "--";
    public Sprite cardFrame;
    public Color frameColor = Color.white;
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
    private bool cardExecutionCalled = false;

    [HideInInspector] public CardState myState = CardState.Inactive;
    public CardType myType;
    [HideInInspector] public Tile selectedTile = null;
    [HideInInspector] public Unit selectedUnit = null;

    protected virtual void Start()
    {
        myState = CardState.Inactive;

        if (canNotTargetOccupiedTiles && hasToTargetOccupiedTiles)
            Debug.LogError($"WARNING: {cardName} both has to and is unable to target occupied Tiles!");
        if (canNotTargetDirtTiles && hasToTargetDirtTiles)
            Debug.LogError($"WARNING: {cardName} both has to and is unable to target dirt covered Tiles!");
    }

    protected virtual void Update()
    {
        // Depending on the state of the card, determine behaivor
        switch (myState)
        {
            case CardState.Inactive:
                // Ensure all things are ready for being played
                selectedTile = null;
                selectedUnit = UnitSelector.Instance.selectedUnit;
                unitsHighligthed = false;
                tilesHighligthed = false;
                cardExecutionCalled = false;

                break;
            case CardState.VerifyUnitSelection:
                // Verify if the selected unit is legal
                VerifyUnitSelection(CardTargetFinder.FindLegalUnits(this));

                break;
            case CardState.SelectingUnit:
                // Select a new unit
                // This state is only entered after a failed verification
                if (!unitsHighligthed)
                {
                    List<Tile> tilesWithHighlightContent = new();
                    List<Unit> unitsToHighlight = CardTargetFinder.FindLegalUnits(this);
                    foreach (Unit u in unitsToHighlight)
                    {
                        tilesWithHighlightContent.Add(u.standingOn);
                    }

                    CardTargetFinder.HighlightContent(tilesWithHighlightContent, true, false);
                    unitsHighligthed = true;
                }
                SelectUnit();

                break;
            case CardState.SelectingTile:
                // Select a tile to attempt to use the card on
                if (!tilesHighligthed)
                {
                    CardTargetFinder.UnhighlightAllContent();
                    selectedUnit.standingOn.Highlight(Color.blue);
                    HighlightTiles();
                    tilesHighligthed = true;
                }
                SelectTile();

                break;
            case CardState.VerifyTileSelection:
                // Verify if the selected tile is legal
                VerifyTileSelection(CardTargetFinder.FindLegalTiles(this));

                break;
            case CardState.Executing:
                // Execute the cards behaivour
                if (!cardExecutionCalled)
                {
                    ExecuteBehaivour(selectedTile, selectedUnit);
                    DEBUGCardStateUI.Instance.DEBUGUpdateUI(CardState.Executing, "Playing card!");
                    cardExecutionCalled = true;
                }
                break;
            case CardState.Finished:
                // Let the card manager know the card is finished and can go to discard
                CardManager.Instance.CardEffectComplete();
                MovementManager.Instance.takingMoveAction = true;
                myState = CardState.Inactive;
                CardTargetFinder.UnhighlightAllContent();
                if (UnitSelector.Instance.selectedUnit != null)
                    UnitSelector.Instance.UpdateSelectedUnit(UnitSelector.Instance.selectedUnit);
                ActiveCard.Instance.cardBeingPlayed = null;
                tilesHighligthed = false;
                unitsHighligthed = false;
                cardExecutionCalled = false;
                DEBUGCardStateUI.Instance.DEBUGUpdateUI(CardState.Inactive, "--");

                break;
            default:
                Debug.LogError("Reached end of state-machine, new case not added to switch?");
                Debug.Log("Going inactive");
                myState = CardState.Inactive;

                break;
        }
    }

    protected virtual void VerifyUnitSelection(List<Unit> legalUnits)
    {
        // If no unit is selected - go to select unit.
        if (selectedUnit == null)
        {
            HandleIllegalSelection("No unit selected, please select a unit",
                                    "Select a Unit.");

            return;
        }

        if (!legalUnits.Contains(selectedUnit))
        {
            HandleIllegalSelection("Illegal unit selected", "Please select a highlighted unit");

            return;
        }

        // If reached this bit of the code, the bot is verified and get's to cast the card.
        myState = CardState.SelectingTile;
        DEBUGCardStateUI.Instance.DEBUGUpdateUI(CardState.SelectingTile, "Select a tile");

        void HandleIllegalSelection(string errorMessage, string cardStateText)
        {
            //Debug.Log(errorMessage);
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

                    if (hit.collider.gameObject.TryGetComponent<Unit>(out Unit clickedUnit))
                    {
                        clickedTile = clickedUnit.standingOn;
                        PickUnit(clickedTile);
                        return;
                    }

                    if (hit.collider.gameObject.TryGetComponent<Tile>(out clickedTile))
                    {
                        if (clickedTile.occupant != null)
                        {
                            PickUnit(clickedTile);
                            return;
                        }
                    }
                }
            }

            // Also check UI elements for the mini potraits
            PointerEventData ped = new PointerEventData(GraphicsRayCastAssistance.Instance.eventSystem);
            ped.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            GraphicsRayCastAssistance.Instance.caster.Raycast(ped, results);
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.TryGetComponent<UnitMiniPanel>(out UnitMiniPanel ump))
                {
                    PickUnit(ump.myUnit.standingOn);
                    return;
                }
            }

            void PickUnit(Tile onTile)
            {
                selectedUnit = onTile.occupant;
                UnitSelector.Instance.UpdateSelectedUnit(selectedUnit);
                myState = CardState.VerifyUnitSelection;
            }
        }
    }

    protected virtual void HighlightTiles()
    {
        // Find legal tiles
        List<Tile> legalTiles = CardTargetFinder.FindLegalTiles(this);

        // Deterime what to highlight
        bool highlightDirt = hasToTargetDirtTiles;
        bool highlightUnits = hasToTargetOccupiedTiles;

        // Highlight relevant data
        CardTargetFinder.HighlightContent(legalTiles, highlightUnits, highlightDirt);
    }

    protected virtual void SelectTile()
    {
        // Click on a tile, unit, or dirt
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

                    if (hit.collider.gameObject.TryGetComponent<Dirt>(out Dirt clickedDirt))
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

            // Also check UI elements for the mini potraits
            PointerEventData ped = new PointerEventData(GraphicsRayCastAssistance.Instance.eventSystem);
            ped.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            GraphicsRayCastAssistance.Instance.caster.Raycast(ped, results);
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.TryGetComponent<UnitMiniPanel>(out UnitMiniPanel ump))
                {
                    PickTile(ump.myUnit.standingOn);
                    return;
                }
            }

            void PickTile(Tile thisTile)
            {
                selectedTile = thisTile;
                myState = CardState.VerifyTileSelection;
            }
        }
    }

    protected virtual void VerifyTileSelection(List<Tile> legalTargets)
    {
        // If no selected tile is passed
        if (selectedTile == null)
        {
            HandleIllegalSelection("No tile selected for verification - error in structure",
                                    "Contact a programmer, no tile selected");
            return;
        }

        // We already gather all legal tiles to highlight them, no need to do that twice
        if (!legalTargets.Contains(selectedTile))
        {
            HandleIllegalSelection("Illegal target selected",
                                    "Please select a highlighted target");
            return;
        }

        // If reached this bit of the code, the card is valid and can be executed
        myState = CardState.Executing;

        // When an illegal selection is made, log it and return to the selection stage
        void HandleIllegalSelection(string errorMessage, string cardStateText)
        {
            //Debug.Log(errorMessage);
            DEBUGCardStateUI.Instance.DEBUGUpdateUI(CardState.VerifyTileSelection, cardStateText);
            myState = CardState.SelectingTile;
            CardTargetFinder.UnhighlightAllContent();
            tilesHighligthed = false;
        }
    }

    // Called to start playing the process of playing the card
    public virtual void Play()
    {
        // Since units may be selected before playing, start by verifying the selected unit to play the card with.
        myState = CardState.VerifyUnitSelection;
        DEBUGCardStateUI.Instance.DEBUGUpdateUI(CardState.SelectingUnit, "Select a unit");

        // Disable the ability to move units while playing cards
        MovementManager.Instance.takingMoveAction = false;
    }


    public abstract void ExecuteBehaivour(Tile onTile, Unit byUnit);

    /// <summary>
    /// Left Abstract to force reminder when implementing new subclass
    /// Call this once the card behaivour is complete
    /// </summary>
    public abstract void ConfirmCardExecuted();

    public void CancelPlay()
    {
        DEBUGCardStateUI.Instance.DEBUGUpdateUI(CardState.Inactive, "--");

        selectedTile = null;
        selectedUnit = null;
        myState = CardState.Inactive;

        ActiveCard.Instance.cardBeingPlayed = null;
        MovementManager.Instance.takingMoveAction = true;

        CardTargetFinder.UnhighlightAllContent();
        UnitSelector.Instance.UpdateSelectedUnit(UnitSelector.Instance.selectedUnit);

        HoverManager.RepeatLastCursor(); // TODO: Fix this function so it shoots a raycast and checks what it hits instead
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public struct HoverHits
{
    public HoverHits(Unit u, Tile t, Dirt d)
    {
        unit = u;
        tile = t;
        dirt = d;
    }

    public Unit unit;
    public Tile tile;
    public Dirt dirt;
}

// I am sorry for how I wrote this - R
public class HoverManager : MonoBehaviour
{
    private static HoverManager instance;
    public HoverHits myHits = new HoverHits(null, null, null);

    public static HoverManager Instance { get => GetInstance(); private set => instance = value; }

    private void Awake()
    {
        if(instance == null || instance == this)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    private void Update()
    {
        CheckHover();
    }
    public void CheckHover()
    {
        if (TutorialBasic.Instance.IsInTutorial)
        {
            if (TutorialBasic.Instance.BasicIndexesToPreventRaycastingOn.Contains(TutorialBasic.Instance.BasicTutorialIndex))
            {
                MouseControl.Instance.SetCursor(Cursors.Default, true); // TODO: Only do this once
                return;
            }
        }

        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        Tile oldTile = myHits.tile;

        /*if(myHits.tile != null)
            HoverTileExit(myHits.tile);*/

        if(myHits.unit != null)
            myHits.unit.HoverTextUnitExit();

        myHits.unit = null;
        myHits.tile = null;
        myHits.dirt = null;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider == null)
                return;

            // Check for unit
            if(hit.collider.gameObject.TryGetComponent<Unit>(out Unit u))
            {
                myHits.unit = u;
                myHits.tile = u.standingOn;
            }

            // Check for dirt
            else if(hit.collider.gameObject.TryGetComponent<Dirt>(out Dirt d))
            {
                myHits.dirt = d;
                myHits.tile = d.myTile;
            }

            // Check for tile
            if(myHits.tile == null) 
            {
                if (hit.collider.gameObject.TryGetComponent<Tile>(out Tile t))
                    myHits.tile = t;
            }

            if(myHits.tile != oldTile)
            {
                if (oldTile != null)
                    HoverTileExit(oldTile);
            }

            if (myHits.tile != null) { 
                HoverTileEnter(myHits.tile);
                if (Input.GetMouseButtonDown((int)MouseButton.Left))
                    myHits.tile.OnClick();
            }

            if (myHits.unit != null)
                myHits.unit.HoverTextUnit();
        }
    }

    public static void HoverTileEnter(Tile tile)
    {
        if (tile.myHighligther == null) 
            return;

        if (tile.highlightedForMovement)
        {
            List<Tile> pathable = Pathfinding.FindPath(tile, UnitSelector.Instance.selectedUnit.standingOn, UnitSelector.Instance.selectedUnit.movePointsCur, false);

            if (tile.myHighligther.color == Color.blue || tile.myHighligther.color == Color.yellow)
            { }// Do nothings
            else if(pathable != null)
            {
                if(pathable.Count <= UnitSelector.Instance.selectedUnit.movePointsCur)
                    tile.Highlight(Color.green);
                else
                    tile.Highlight(Color.red);
            }
            else
                tile.Highlight(Color.red);


            CursorManagerEnter(tile);
            return;
        }


        // If a card is not being played
        // Highlight white or red depending on blocked status
        // Yellow/blue takes higher priority for enemy/friendly units
        if (ActiveCard.Instance.cardBeingPlayed == null)
        {
            if (tile.myHighligther.color == Color.blue || tile.myHighligther.color == Color.yellow)
            { }// Do nothings
            else if (UnitSelector.highlightingMovementTiles || tile.blocked)
                tile.Highlight(Color.red);
            else
                tile.Highlight();
        }
        else if (tile.highlighted)
        {
            // If a card is being played, highlight as green if it's not on the selected unit

            if (tile.myHighligther.color != Color.blue)
                tile.Highlight(Color.green);
        }

        CursorManagerEnter(tile);
    }

    public static void HoverTileExit(Tile tile)
    {
        if (tile.myHighligther == null) 
            return;

        // If a card is not being played
        // Remove highlight
        if (ActiveCard.Instance.cardBeingPlayed == null)
        {
            if (!tile.highlightedForMovement)
            {
                if (tile.myHighligther.color != Color.blue && tile.myHighligther.color != Color.yellow)
                    tile.UnHighlight();
            }
            else
            {
                if (tile.myHighligther.color != Color.blue && tile.myHighligther.color != Color.yellow)
                    tile.Highlight();
            }
        }
        else if (tile.highlighted)
        {
            // If a card is being played
            // Keep it highligthed
            // Keep it blue for the selected unit

            if (tile.myHighligther.color != Color.blue)
                tile.Highlight();
        }

        CursorManagerExit();
    }

    public static void CursorManagerEnter(Tile tile)
    {
        // OBS:
        // Order of if statements matter a lot in this function!
        // Be very mindful of refactoring and additions

        // TODO: 
        // Use local functions to make this whole function more readable

        Unit selectedUnit = UnitSelector.Instance.selectedUnit;
        Card selectedCard = ActiveCard.Instance.cardBeingPlayed;

        // No bot selected  - default
        if(selectedUnit == null)
        {
            MouseControl.Instance.SetCursor(Cursors.Default, true);
            return;
        }

        // Going forward, there will always be a playerbot selected If this if gets passed over
        if (!selectedUnit.playerBot) 
        {
            MouseControl.Instance.SetCursor(Cursors.Default, true);
            return;
        }

        // Going forward, there will always be a card in play, if this If gets passed over
        if (selectedCard == null)
        {
            if(tile.occupied)
            {
                MouseControl.Instance.SetCursor(Cursors.Default, true);
                return;
            }
            else if(tile.blocked)
            {
                MouseControl.Instance.SetCursor(Cursors.Default, true);
                return;
            }

            // If tile has pickup, use pickup cursor,
            // otherwise, use footsteps
            if(tile.myPickUp != null)
            {
                // Determine if the pickup is reachable even theoretically
                if(selectedUnit.movePointsCur >= Pathfinding.GetDistance(selectedUnit.standingOn, tile))
                {
                    // Determine the pickup is reachable
                    List<Tile> path = Pathfinding.FindPath(selectedUnit.standingOn, tile);
                    if(path != null)
                    {
                        // Determine the pickup is in range
                        if (selectedUnit.movePointsCur >= path.Count)
                            MouseControl.Instance.SetCursor(Cursors.Pickup, true);
                        else
                            MouseControl.Instance.SetCursor(Cursors.Pickup, false);
                    }
                    else
                        MouseControl.Instance.SetCursor(Cursors.Pickup, false);
                }
                else
                    MouseControl.Instance.SetCursor(Cursors.Pickup, false);
            }
            else
            {
                // Determine if the tile is reachable even theoretically
                if (selectedUnit.movePointsCur >= Pathfinding.GetDistance(selectedUnit.standingOn, tile))
                {
                    // Determine the tile is reachable
                    List<Tile> path = Pathfinding.FindPath(selectedUnit.standingOn, tile);
                    if (path != null)
                    {
                        // Determine the tile is in range
                        if (selectedUnit.movePointsCur >= path.Count)
                            MouseControl.Instance.SetCursor(Cursors.Walk, true);
                        else
                            MouseControl.Instance.SetCursor(Cursors.Walk, false);
                    }
                    else
                        MouseControl.Instance.SetCursor(Cursors.Walk, false);
                }
                else
                    MouseControl.Instance.SetCursor(Cursors.Walk, false);
            }
            return;
        }

        // Playerbot selected, dig card, over rock          - pickaxe (red if can't reach)
            if (selectedCard.GetType() == typeof(DigCard))
                if (selectedUnit.mySpecialization == BotSpecialization.Digger)
                    if (tile.containsDirt)
                    {
                        if (Pathfinding.GetDistance(selectedUnit.standingOn, tile) <= selectedCard.range)                          
                            MouseControl.Instance.SetCursor(Cursors.Dig, true);
                        else
                            MouseControl.Instance.SetCursor(Cursors.Dig, false);

                        return;
                    }

        // Playerbot selected, attack card, over attack     - sword (red if can't reach)
        if (selectedCard.GetType() == typeof(AttackCard))
            if (tile.occupied)
            {
                if (Pathfinding.GetDistance(selectedUnit.standingOn, tile) <= selectedCard.range)
                    MouseControl.Instance.SetCursor(Cursors.Fight, true);
                else
                    MouseControl.Instance.SetCursor(Cursors.Fight, false);

                return;
            }

        // Playerbot selected, construct card, over free space  - spanner (red if can't reach)
        if (selectedCard.GetType() == typeof(SpawnUnitCard))
            if (!tile.blocked)
            {
                if (Pathfinding.GetDistance(selectedUnit.standingOn, tile) <= selectedCard.range)
                    MouseControl.Instance.SetCursor(Cursors.Construct, true);
                else
                    MouseControl.Instance.SetCursor(Cursors.Construct, false);

                return;
            }

        MouseControl.Instance.SetCursor(Cursors.Default, false);
    }

    public static void CursorManagerExit()
    {
        MouseControl.Instance.SetCursor(Cursors.Default, true);
    }

    public static HoverManager GetInstance()
    {
        if (instance != null)
            return instance;

        GameObject newManager = new GameObject();
        instance = newManager.AddComponent<HoverManager>();
        return instance;
    }
}

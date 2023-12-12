using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class HoverManager : MonoBehaviour
{
    static Tile lastHit;
    public static void HoverTileEnter(Tile tile)
    {
        if (tile.myHighligther == null) 
            return;

        // If a card is not being played
        // Highlight white or red depending on blocked status
        // Yellow/blue takes higher priority for enemy/friendly units
        if (ActiveCard.Instance.transform.childCount == 0)
        {
            if (tile.myHighligther.color == Color.blue || tile.myHighligther.color == Color.yellow)
            { }// Do nothings
            else if (!tile.blocked)
                tile.Highlight();

            else
                tile.Highlight(Color.red);
        }
        else if (tile.myHighligther.gameObject.activeSelf == true)
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
        if (ActiveCard.Instance.transform.childCount == 0)
        {
            if (tile.myHighligther.color != Color.blue && tile.myHighligther.color != Color.yellow)
                tile.UnHighlight();
        }
        else if (tile.myHighligther.gameObject.activeSelf == true)
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

        lastHit = tile;

        Unit selectedUnit = UnitSelector.Instance.selectedUnit;
        PlayCard selectedCard = ActiveCard.Instance.cardBeingPlayed;

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

    public static void RepeatLastCursor()
    {
        if(lastHit != null)
            CursorManagerEnter(lastHit);
        else
            MouseControl.Instance.SetCursor(Cursors.Default, true);
    }
}

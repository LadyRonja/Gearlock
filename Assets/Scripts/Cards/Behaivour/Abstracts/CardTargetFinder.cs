using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class CardTargetFinder
{
    public static List<Unit> FindLegalUnits(Card forCard)
    {
        // Find legal units
        List<Unit> legalUnits = new();
        if (forCard.requiredSpecialization != BotSpecialization.None)
            legalUnits = UnitStorage.Instance.playerUnits.Where(u => u.mySpecialization == forCard.requiredSpecialization).ToList();
        else
            legalUnits = UnitStorage.Instance.playerUnits;

        #region No legal targets behaivour
        // If no legal units, put card back in hand
        /*if (legalUnits.Count == 0)
        {
            //Debug.Log("No legal unit found, removing card from played");
            CancelPlay();
            CardManager.instance.ClearActiveCard();
            return;
        }*/
        #endregion

        // Remove duplicates
        legalUnits = legalUnits.Distinct().ToList();

        return legalUnits;
    }

    public static List<Tile> FindLegalTiles(Card forCard)
    {
        // Find legal tiles
        List<Tile> legalTiles = new();

        legalTiles.AddRange(GridManager.Instance.tiles);
        List<Tile> tilesToRemove = new();
        // Remove illegal tiles
        foreach (Tile t in legalTiles)
        {
            // Wall check
            if(forCard.canNotTargetWalls && t.walled)
                tilesToRemove.Add(t);

            // Dirt Check
            if (forCard.hasToTargetDirtTiles && !t.containsDirt)
                tilesToRemove.Add(t);

            if (forCard.canNotTargetDirtTiles && t.containsDirt)
                tilesToRemove.Add(t);

            // Unit Check
            if (forCard.hasToTargetOccupiedTiles && !t.occupied)
                tilesToRemove.Add(t);

            if (forCard.canNotTargetOccupiedTiles && t.occupied)
                tilesToRemove.Add(t);

            // Range check
            if (Pathfinding.GetDistance(t, forCard.selectedUnit.standingOn) > forCard.range || Pathfinding.GetDistance(t, forCard.selectedUnit.standingOn) == 0)
                tilesToRemove.Add(t);
        }

        foreach (Tile t in tilesToRemove)
        {
            if (legalTiles.Contains(t))
                legalTiles.Remove(t);
        }

        #region No legal targets behaivour
        // If no legal tiles, put card back in hand
        /*if (legalTiles.Count == 0)
        {
            //Debug.Log("No legal tile found, removing card from played");
            CancelPlay();
            CardManager.instance.ClearActiveCard();
            return;
        }*/
        #endregion

        // Remove duplicates
        legalTiles = legalTiles.Distinct().ToList();

        return legalTiles;
    }

    public static void HighlightContent(List<Tile> onTiles, bool highlightUnits, bool highlightDirt)
    {
        foreach (Tile t in onTiles)
        {
            t.Highlight();

            if (highlightUnits)
            {
                if(t.occupant != null)
                {
                    t.occupant.Highlight();
                    List<UnitMiniPanel> panelsToHighlight = UnitStorage.Instance.playerPanels.Where(ump => ump.myUnit == t.occupant).ToList();
                    if (panelsToHighlight.Count != 0)
                        panelsToHighlight[0].Highlight();
                }
            }

            if(highlightDirt)
            {
                if (t.dirt != null)
                    t.dirt.Highlight();
            }
        }
    }

    public static void UnhighlightAllContent()
    {
        GridManager.Instance.UnhighlightAll();
        foreach (Unit u in UnitStorage.Instance.playerUnits)
            u.UnHighlight();
        foreach (Unit u in UnitStorage.Instance.enemyUnits)
            u.UnHighlight();
        foreach (UnitMiniPanel ump in UnitStorage.Instance.playerPanels)
            ump.UnHighlight();
    }
}

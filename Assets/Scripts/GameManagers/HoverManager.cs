using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class HoverManager : MonoBehaviour
{
    public static void HoverTileEnter(Tile tile)
    {
        if (tile.myHighligther == null) return;
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
    }

    public static void HoverTileExit(Tile tile)
    {
        if (tile.myHighligther == null) return;
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
    }

    public static void CursorManagerEnter(Tile tile)
    {
        // Playerbot selected, dig card, over rock          - pickaxe (red if can't reach)
        if(UnitSelector.Instance.selectedUnit != null)
            if(UnitSelector.Instance.selectedUnit.playerBot)
                if(UnitSelector.Instance.selectedUnit.mySpecialization == BotSpecialization.Digger) { }
                    //if(tile.containsDirt)

        // Playerbot selected, attack card, over attack     - sword (red if can't reach)
        // Playerbot selected, no card, tile w/ pickup      - pickup
        // Playerbot selected, no card, tile                - footprints (red if can't reach)
        // No bot selected                                  - default

    }

    public static void CursorManagerExit(Tile tile)
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class HoverManager : MonoBehaviour
{
    public static void HoverTileEnter(Tile tile)
    {
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
}

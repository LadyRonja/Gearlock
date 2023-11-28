using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class TileClicker : MonoBehaviour
{
   public static TileClicker Instance;

    public Tile debugInspectorTile;

    private void Awake()
    {
        #region Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
        #endregion
    }

    public void SpawnDirt(Tile tile)
    {
        DirtSpawner.Instance.SpawnDirt(tile);
    }

    public void ToggleBlockedDebug(Tile tile)
    {
        tile.blocked = !tile.blocked;
        tile.targetable = !tile.targetable;

        if (tile.blocked)
            tile.myMR.material.color = Color.black;
        else
            tile.myMR.material.color = Color.white;
    }

    public void UpdateSelectedUnit(Tile tile) 
    { 
        if(tile.occupant != null)
            UnitSelector.Instance.UpdateSelectedUnit(tile.occupant);
    }

    public void HandleMoveClick(Tile tile)
    {
        // Identify all the times movement can't occur
        if (!tile.targetable) return;
        if (UnitSelector.Instance.selectedUnit == null) return;
        if (!UnitSelector.Instance.selectedUnit.playerBot) return;
        if (!TurnManager.Instance.isPlayerTurn) return;

        MovementManager.Instance.MoveUnit(UnitSelector.Instance.selectedUnit, tile);
    }
}

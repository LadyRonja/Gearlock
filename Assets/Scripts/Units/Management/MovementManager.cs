using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public static MovementManager Instance;
    public bool takingMoveAction = true;

    private void Awake()
    {
        #region Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
        #endregion
    }

    public void MoveUnit(Unit unit, Tile toTile)
    {
        if (unit == null) return;
        if (!takingMoveAction) return;
        if (unit.movePointsCur <= 0) return;

        List<Tile> path = Pathfinding.FindPath(unit.standingOn, toTile, unit.movePointsCur);
        if (path == null) 
        {
            // TODO: Indicate no available path
            return;
        }

        unit.StartMovePath(path);
    }
}

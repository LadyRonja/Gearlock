using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public static MovementManager Instance;
    public Unit debugUnitToMove;

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
        List<Tile> path = Pathfinding.FindPath(unit.standingOn, toTile);
        if (path == null) 
        {
            // TODO: Indicate no available path
            return;
        }

        unit.StartMovePath(path);
    }
}

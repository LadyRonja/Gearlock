using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Unit
{
    public override Unit FindTargetUnit()
    {
        return FindNearestPlayerUnit(true);
    }

    public override List<Tile> CalculatePathToTarget(Tile targetTile)
    {
        List<Tile> output = Pathfinding.FindPath(standingOn, targetTile, movePointsCur, true);
        if (output == null)
        {
            //Debug.Log("No path found");
            return null;
        }
        if(output.Count == 0)
        {
            //Debug.Log("Path is 0 long");
            return null;
        }

        if (output[output.Count - 1] == targetTile)
            output.RemoveAt(output.Count - 1);

        return output;
    }
}

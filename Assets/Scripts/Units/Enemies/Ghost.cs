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
        if (output[output.Count - 1] == targetTile)
            output.RemoveAt(output.Count - 1);

        return output;
    }
}

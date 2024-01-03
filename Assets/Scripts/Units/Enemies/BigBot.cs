using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBot : Unit
{
    public override Unit FindTargetUnit()
    {
        Unit unitToReturn = FindNearestPlayerUnit(false);
        if (unitToReturn == null)
            return FindNearestPlayerUnit(true);
        else
            return unitToReturn;
    }

    public override List<Tile> CalculatePathToTarget(Tile targetTile)
    {
        List<Tile> output = Pathfinding.FindPath(standingOn, targetTile, movePointsCur, false);
        if (output == null)
        {
            output = Pathfinding.FindPath(standingOn, targetTile, movePointsCur, true);
            if (output == null)
                return null;
        }
        if (output.Count == 0)
        {
            //Debug.Log("Path is 0 long");
            return null;
        }

        if (output[output.Count - 1] == targetTile)
            output.RemoveAt(output.Count - 1);

        return output;
    }

    public override IEnumerator MovePath(List<Tile> path)
    {
        if (path == null)
            yield break;

        if (path.Count == 0)
            yield break;

        for (int i = 0; i < path.Count; i++)
        {
            if (path[i].containsDirt)
            {
                yield return new WaitForSeconds(0.5f);
                PlayActionAnimation();
                path[i].RemoveDirt(false);
                yield return new WaitForSeconds(0.3f);
                yield return StartCoroutine(MoveStep(path[i]));
                break;
            }

            yield return StartCoroutine(MoveStep(path[i]));
        }

        doneMoving = true;
        MovementManager.Instance.takingMoveAction = true;
        UnitSelector.Instance.HighlightAllTilesMovableTo();
        yield return null;
    }
}


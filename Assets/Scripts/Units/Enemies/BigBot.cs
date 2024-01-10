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
        List<Tile> output = Pathfinding.FindPath(standingOn, targetTile, movePointsCur, false, false);
        if (output == null)
        {
            output = Pathfinding.FindPath(standingOn, targetTile, movePointsCur, true, false);
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
                FlipEnemyTowardsDirt(path[i]);
                yield return new WaitForSeconds(0.5f);
                
                PlayActionAnimation();
                path[i].RemoveDirt(false);
                yield return new WaitForSeconds(2f);
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

    private void FlipEnemyTowardsDirt(Tile dirtTile)
    {
        // Get the current position of the dirt
        Vector3 dirtPosition = dirtTile.transform.position;

        // Determine the direction to flip based on the dirt tile's position
        bool shouldBeRight = transform.position.x - dirtPosition.x > 0;
        bool shouldBeLeft = transform.position.x - dirtPosition.x < 0;
        float destinationEndX = gfx.localScale.x;

        if (shouldBeRight)
            destinationEndX = Mathf.Abs(destinationEndX);
        else if (shouldBeLeft)
            destinationEndX = Mathf.Abs(destinationEndX) * -1f;

        // Flip the enemy's direction
        Vector3 flipScale = gfx.transform.localScale;
        flipScale.x = destinationEndX;
        gfx.transform.localScale = flipScale;
    }
}


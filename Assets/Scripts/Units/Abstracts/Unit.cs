using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Unit : MonoBehaviour, IDamagable
{
    [Header("Generics")]
    public string unitName = "Unnamed Unit";
    public bool playerBot = false;

    [Header("Stats")]
    public int healthMax = 5;
    public int healthCur = 5;
    public int movePointsMax = 3;
    public int movePointsCur = 3;
    public int power = 1;

    [Header("Movement")]
    public bool doneMoving = true;
    public Tile standingOn;

    [Header("Components")]
    public Transform gfx;
    public SpriteRenderer mySR;

    private void Start()
    {
        mySR = gfx.GetComponent<SpriteRenderer>();
    }

    public virtual void TakeDamage(int amount)
    {
        healthCur -= amount;
        if (healthCur < 0)
        {
            healthCur = 0;
            Die();
        }
    }

    public virtual void Die()
    {
        bool destroyedInEditor = false;
#if UNITY_EDITOR
        DestroyImmediate(this.gameObject);
        destroyedInEditor = true;
#endif

        if(!destroyedInEditor)
            Destroy(this.gameObject);
    }

    /// <summary>
    /// Exclude starting tile.
    /// </summary>
    /// <param name="path"></param>
    public void StartMovePath(List<Tile> path)
    {
        doneMoving = false;
        StartCoroutine(MovePath(path));
    }

    public IEnumerator MovePath(List<Tile> path)
    {
        if (path == null) 
            yield break; 

        if(path.Count == 0)
            yield break;

        for (int i = 0; i < path.Count; i++)
        {
            yield return StartCoroutine(MoveStep(path[i]));
        }

        doneMoving = true;
        yield return null;
    }

    private IEnumerator MoveStep(Tile toTile)
    {
        movePointsCur--;
        Vector3 startPos = this.transform.position;
        Vector3 endPos = toTile.transform.position;
        endPos.y += mySR.bounds.size.y / 2f;

        float timeToMove = 0.5f;
        float timePassed = 0;
        float jumpHeight = 3f;

        while (timePassed < timeToMove)
        {
            transform.position = Vector3.Lerp(startPos, endPos, (timePassed / timeToMove));
            
            // Jumping
            float yOffSet = gfx.localPosition.y;
            yOffSet = Mathf.Max(0, jumpHeight * Mathf.Sin(timePassed / timeToMove * Mathf.PI));
            gfx.localPosition = new Vector3(gfx.localPosition.x, yOffSet, gfx.localPosition.z);

            timePassed += Time.deltaTime;
            UnitSelector.Instance.UpdateUI();
            yield return null;
        }
        standingOn.UpdateOccupant(null);
        standingOn = toTile;
        toTile.UpdateOccupant(this);

        yield return null;
    }

    public virtual Unit FindTargetUnit()
    {
        return FindNearestPlayerUnit(false);
    }

    protected Unit FindNearestPlayerUnit(bool ignoreWalls)
    {
        // Check all units the player has
        List<Unit> targets = UnitStorage.Instance.playerUnits;
        if(targets.Count == 0)
        {
            Debug.LogError("Player has no units registered");
            return null;
        }

        // See how far away the first one is, depending on if the unit can walk through walls or not
        Unit nearestFoundUnit = targets[0];

        int nearestDistance = int.MaxValue;
        if (ignoreWalls)
            nearestDistance = Pathfinding.GetDistance(standingOn, nearestFoundUnit.standingOn);
        else
        {
            List<Tile> path = Pathfinding.FindPath(standingOn, nearestFoundUnit.standingOn);
            if (path != null)
                nearestDistance = path.Count;
            else
                nearestDistance = int.MaxValue;
        }

        // Compare the the currently closest unit to each other unit, if the distance to another unit is shorter,
        // update which one is beign measured from
        for (int i = 1; i < targets.Count; i++)
        {
            int dist = int.MaxValue;
            if (ignoreWalls)
                dist = Pathfinding.GetDistance(standingOn, targets[i].standingOn);
            else
            {
                List<Tile> path = Pathfinding.FindPath(standingOn, targets[i].standingOn);
                if (path != null)
                    dist = path.Count;
                else
                    dist = int.MaxValue;
            }

            // If units are equally close, use a tiebreaker
            if (dist < nearestDistance)
            {
                nearestDistance = dist;
                nearestFoundUnit = targets[i];
            }
            else if (dist == nearestDistance)
            {
                // Tie breaker currently often goes for lowest health, but sometimes for highest health
                int rand = UnityEngine.Random.Range(0, 100);
                int tieBreakerGoodLuckPercentage = 75;
                if (rand <= tieBreakerGoodLuckPercentage)
                    if (nearestFoundUnit.healthCur < targets[i].healthCur) { 
                        nearestFoundUnit = targets[i];
                        Debug.Log($"Tiebreaker went for lower health target: " + rand);
                    }
                else
                    if (nearestFoundUnit.healthCur > targets[i].healthCur)
                    {
                        nearestFoundUnit = targets[i];
                        Debug.Log($"Tiebreaker went for higher health target: " + rand);
                    }
            }
        }
        return nearestFoundUnit;
    }

    public virtual List<Tile> CalculatePathToTarget(Tile targetTile)
    {
        List<Tile> output = Pathfinding.FindPath(standingOn, targetTile, movePointsCur);

        return output;
    }
}

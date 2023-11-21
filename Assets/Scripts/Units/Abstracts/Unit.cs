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

    private IEnumerator MovePath(List<Tile> path)
    {
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
}

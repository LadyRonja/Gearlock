using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;

public enum BotSpecialization
{
    None,
    Digger
}

public abstract class Unit : MonoBehaviour, IDamagable, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [Header("Generics")]
    public string unitName = "Unnamed Unit";
    public bool playerBot = false;
    public BotSpecialization mySpecialization = BotSpecialization.None;
    public Sprite portrait;
    public GameObject infoTextUnit;

    [Header("Stats")]
    public int healthMax = 5;
    public int healthCur = 5;
    public int movePointsMax = 3;
    public int movePointsCur = 3;
    public int power = 1;
    public int attackRange = 1;

    [Header("Movement")]
    public bool doneMoving = true;
    public Tile standingOn;

    [Header("Components")]
    public Transform gfx;
    public MeshRenderer myMR;
    public SpriteRenderer mySR;
    public SpriteRenderer highligtherArrow;

    [Header("DoTween")]
    public Ease currentEase;
    private Vector3 startPos;

    private void Start()
    {
        if(infoTextUnit!= null)
            infoTextUnit.SetActive(false);

        //myMR = gfx.GetComponent<MeshRenderer>();
        if(myMR == null)
        {
            mySR = gfx.GetComponent<SpriteRenderer>();
        }
        if(highligtherArrow != null)
            highligtherArrow.gameObject.SetActive(false);
    }

    public virtual void TakeDamage(int amount)
    {
        if(UnitSelector.Instance.selectedUnit == this)
            UnitSelector.Instance.UpdateUI();

        healthCur -= amount;
        StartCoroutine(FlashDamage(0.7f));

        //test DoTween 
        ShakeUnit();


        if (UnitSelector.Instance.selectedUnit == this)
            UnitSelector.Instance.UpdateUI(true);

        if(playerBot)
            UnitSelector.Instance.UpdatePlayerUnitUI();

        if (healthCur <= 0)
        {
            healthCur = 0;
            Die();
            GameoverManager.Instance.CheckGameOver();
        }
    }

    protected IEnumerator FlashDamage(float time)
    {
       /* if (myMR == null)
            yield return null;*/

        Color startColor = mySR.color;
        mySR.material.color = Color.red;
        float timePassed = 0;
        float timeToFlash = time;

        while (timePassed < timeToFlash)
        {
            mySR.material.color = Color.Lerp(Color.red, startColor, (timePassed / timeToFlash));

            timePassed += Time.deltaTime;
            yield return null;
        }


        yield return null;
    }
    
    //test elin 
    protected IEnumerator FadeAndDestroy(float fadeTime)
    {
        float timePassed = 0;
        Color startColor = mySR.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0);

        while (timePassed < fadeTime)
        {
            mySR.color = Color.Lerp(startColor, endColor, timePassed / fadeTime);
            timePassed += Time.deltaTime;
            yield return null;
        }

        Destroy(this.gameObject);
    }

    public virtual void Die()
    {
        UnitStorage.Instance.RemoveUnit(this);
        UnitSelector.Instance.UpdateSelectedUnit(null, true);
        standingOn.UpdateOccupant(null);
        if (playerBot)
        {
            GameoverManager.Instance.CheckGameOver();
            UnitSelector.Instance.UpdatePlayerUnitUI();
        }
        StartCoroutine(FadeAndDestroy(0.5f));
        //Destroy(this.gameObject);
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

    protected IEnumerator MoveStep(Tile toTile)
    {
        movePointsCur--;

        Vector3 startPos = this.transform.position;
        Vector3 endPos = toTile.transform.position;
        if (myMR != null)
            endPos.y += (myMR.bounds.size.y / 2f) * 0.1f;
        else
            endPos.y += mySR.bounds.size.y / 2f;

        float timeToMove = 0.5f;
        float timePassed = 0;
        float jumpHeight = 3f;
        bool hasChangedHighlight = false;

        while (timePassed < timeToMove)
        {
            transform.position = Vector3.Lerp(startPos, endPos, (timePassed / timeToMove));

            CameraController.Instance.MoveToTarget(this.transform.position, 0.01f);
            
            // Jumping
            float yOffSet = gfx.localPosition.y;
            yOffSet = Mathf.Max(0, jumpHeight * Mathf.Sin(timePassed / timeToMove * Mathf.PI));
            gfx.localPosition = new Vector3(gfx.localPosition.x, yOffSet, gfx.localPosition.z);

            if(!hasChangedHighlight && timePassed > timeToMove / 2f)
            {
                hasChangedHighlight = true;
                if (UnitSelector.Instance.selectedUnit == this)
                {
                    Color currentColor = standingOn.myHighligther.color;
                    toTile.Highlight(currentColor);
                    standingOn.UnHighlight();
                }            
            }

            timePassed += Time.deltaTime;
            UnitSelector.Instance.UpdateUI();
            yield return null;
        }
        gfx.position = transform.position;
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
        if (targets.Count == 0)
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
                {
                    if (nearestFoundUnit.healthCur < targets[i].healthCur)
                        nearestFoundUnit = targets[i];
                    else if (nearestFoundUnit.healthCur > targets[i].healthCur)
                        nearestFoundUnit = targets[i];
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        HoverManager.HoverTileEnter(standingOn);
        HoverTextUnit();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HoverManager.HoverTileExit(standingOn);
        HoverTextUnitExit();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        TileClicker.Instance.UpdateSelectedUnit(standingOn);
    }

    public virtual void Highlight()
    {
        if(highligtherArrow != null)
            highligtherArrow.gameObject.SetActive(true);

    }

    public virtual void UnHighlight()
    {
        if (highligtherArrow != null)
            highligtherArrow.gameObject.SetActive(false);
    }

    protected void HoverTextUnit()
    {
        if (infoTextUnit != null)
            infoTextUnit.SetActive(true);
    }

    protected void HoverTextUnitExit()
    {
        if (infoTextUnit != null)
            infoTextUnit.SetActive(false);
    }

    //test elin DoTween shake unit
    public void ShakeUnit()
    {      
        transform.DOShakePosition(duration: 0.5f, strength: new Vector3(2f, 0f, 0f), vibrato: 10, randomness: 0, fadeOut: false);      
    }
    public void OnDisable()
    {
        transform.DOKill();
    }
}

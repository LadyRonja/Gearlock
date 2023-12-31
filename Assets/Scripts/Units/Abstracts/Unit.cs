using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;
using TMPro;
using Spine;
using static UnityEngine.EventSystems.EventTrigger;

public enum BotSpecialization
{
    None,
    Digger,
    Constructor
}

public abstract class Unit : MonoBehaviour, IDamagable, IPointerDownHandler
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
    public List<GameObject> MovePointBase;
    public List<GameObject> MovePointLight;
    public List<GameObject> MovePointDark;
    int maxMovePoints = 4;
    float animationLiftHeight = 3f;
    private int lastDisabledLightIndex = -1;
    Vector3 endMovePosition;

    [Header("Components")]
    public Transform gfx;
    public MeshRenderer myMR;
    public SpriteRenderer mySR;
    public SpriteRenderer highligtherArrow;    

    [Header("Health Bar")]
    public GameObject healthBar;
    public Image healthFill;
    public TMP_Text healthText;

    [Header("Sounds")]
    public List<AudioClip> getSelectedSound = new();
    public List<AudioClip> takeDamageSound = new();
    public List<AudioClip> deathSound = new();
    public List<AudioClip> getSpawnedSound = new();
    public List<AudioClip> startMovingSound = new();
    public List<AudioClip> finishedStepSound = new();

    [Header("Highlighter Bounce")]
    protected float highlighterYOffSet = 0;
    protected Vector3 highlighterStartPos = Vector3.zero;
    protected AnimationCurve highligtherCurve;

    [Header("DoTween")]
    public Ease currentEase;
    public Ease actionAnimEase;

    [Header("SpineAnimation")]
    public bool aktiveSpineAnimation = false; // if a unit has spine this i checked
    public string idleAnimation;
    public string jumpAnimation;

    private bool isJumping = false;

    private TurnManager turnManager;

    public SkeletonAnimation skeletonAnimation;

    public void Awake()
    {
        DOTween.SetTweensCapacity(7812, 50); // takes away warning in unity
    }

    private void Start()
    {
        //EnableMovePointLights();
        //IdleAnimationSprite();
        float rand = UnityEngine.Random.Range(0.0f, 0.4f);
        Invoke("IdleAnimSpriteTwo", rand);

        if (highligtherArrow != null)
        {
            highlighterStartPos = highligtherArrow.transform.position;
            highligtherCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
            highligtherCurve.preWrapMode = WrapMode.PingPong;
            highligtherCurve.postWrapMode = WrapMode.PingPong;
        }

        turnManager =GetComponent<TurnManager>();

        // Initialize health text
        if (healthText != null)
            healthText.text = $"HP: {healthCur}/{healthMax}";

        // Check if the child object with the name "GFX (Spine)" exists
        Transform spineGFX = transform.Find("GFX (Spine)"); // TODO: Why?

        //skeletonAnimation = spineGFX.GetComponent<SkeletonAnimation>();
        if (skeletonAnimation != null)
        {
            aktiveSpineAnimation = true;
            PlayIdleAnimation();
        }

        if (infoTextUnit != null)
            infoTextUnit.SetActive(false);

        //myMR = gfx.GetComponent<MeshRenderer>();
        if (myMR == null)
        {
            mySR = gfx.GetComponent<SpriteRenderer>();
        }
        if (highligtherArrow != null)
            highligtherArrow.gameObject.SetActive(false);
    }

    protected virtual void Update()
    {
        HighlighterBounce();
    }

    protected void HighlighterBounce()
    {
        if (highligtherArrow == null)
            return;

        highlighterYOffSet = highligtherCurve.Evaluate(Time.time % highligtherCurve.length);
        Vector3 currentPos = highligtherArrow.transform.position;
        highligtherArrow.transform.position = new Vector3(currentPos.x, highlighterStartPos.y + highlighterYOffSet, currentPos.z);
    }

    public virtual void TakeDamage(int amount)
    {
        if (UnitSelector.Instance.selectedUnit == this)
            UnitSelector.Instance.UpdateUI();

        healthCur -= amount;
        StartCoroutine(FlashDamage(0.7f));

        //test DoTween 
        ShakeUnit();

        // Update health bar
        UpdateHealthBar();
        UpdateHealthText();

        if (playerBot)
            GameStats.Instance.IncreaseDamageTaken(amount);
        else
            GameStats.Instance.IncreaseDamageDealt(amount);

        if (UnitSelector.Instance.selectedUnit == this)
            UnitSelector.Instance.UpdateUI(true);

        if (playerBot)
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
        // Delay before starting the flash damage animation
        float delay = 0.4f;
        yield return new WaitForSeconds(delay);

        Color startColor = mySR.color;
        mySR.material.color = Color.red;
        float timePassed = 0;
        float timeToFlash = time;

        AudioHandler.PlayRandomEffectFromList(takeDamageSound);

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
        //AudioHandler.PlayRandomEffectFromList(deathSound);

        transform.DOKill();
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
            GameStats.Instance.IncreaseRobotsLost();
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
        MovementManager.Instance.takingMoveAction = false;
        UnitSelector.Instance.UnHighlightAllTilesMoveableTo();
        doneMoving = false;
        StartCoroutine(MovePath(path));
    }

    public virtual IEnumerator MovePath(List<Tile> path)
    {    
        if (path == null)
        {
            MovementManager.Instance.takingMoveAction = true;
            yield break;
        }

        if (path.Count == 0)
        {
            MovementManager.Instance.takingMoveAction = true;
            yield break;
        }

        for (int i = 0; i < path.Count; i++)
        {
            yield return StartCoroutine(MoveStep(path[i]));
        }
        doneMoving = true;
        
        MovementManager.Instance.takingMoveAction = true;
        UnitSelector.Instance.HighlightAllTilesMovableTo();      

        yield return null;
    }

    protected IEnumerator MoveStep(Tile toTile)
    {
        movePointsCur--;
        
        Vector3 startPos = this.transform.position;
        Vector3 endPos = toTile.transform.position;
        endMovePosition = endPos;
        endPos.z -= 0.1f;
        
        if (myMR != null)
            endPos.y += myMR.bounds.size.y / 2f;
        else
            endPos.y += mySR.bounds.size.y / 2f ;         

        float timeToMove = 0.5f;
        float timePassed = 0;
        float jumpHeight = 3f;
        bool hasChangedHighlight = false;
        bool shouldBeRight = standingOn.x - toTile.x > 0;
        bool shouldBeLeft = standingOn.x - toTile.x < 0;
        float startXScale = gfx.localScale.x;
        Vector3 startSize = gfx.transform.localScale;
        float destinationEndX = startXScale;
        if (shouldBeRight)
            destinationEndX = MathF.Abs(gfx.localScale.x);
        else if(shouldBeLeft)
            destinationEndX = MathF.Abs(gfx.localScale.x) * -1f;

        while (timePassed < timeToMove)
        {
            transform.position = Vector3.Lerp(startPos, endPos, (timePassed / timeToMove));
            Vector3 flipScale = gfx.transform.localScale;
            flipScale.x = Mathf.Lerp(startXScale, destinationEndX, (timePassed / timeToMove));
            gfx.transform.localScale = flipScale;

            CameraController.Instance.MoveToTarget(this.transform.position, 0.01f);

            // Jumping          
            float yOffSet = gfx.localPosition.y;
            yOffSet = Mathf.Max(0, jumpHeight * Mathf.Sin(timePassed / timeToMove * Mathf.PI));
            gfx.localPosition = new Vector3(gfx.localPosition.x, yOffSet, gfx.localPosition.z);
          
            PlayJumpAnimation();
       
            // After the halfway point, change the highlighted tile
            if (!hasChangedHighlight && timePassed > timeToMove / 2f)
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
        startSize.x = destinationEndX;
        gfx.localScale = startSize;
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

    public void OnPointerDown(PointerEventData eventData)
    {
        TileClicker.Instance.UpdateSelectedUnit(standingOn);

        // Pass the clicked tile to FlipOnXAxis
        FlipOnXAxis(standingOn);
    }

    public virtual void Highlight()
    {
        if (highligtherArrow != null)
            highligtherArrow.gameObject.SetActive(true);
    }

    public virtual void UnHighlight()
    {
        if (highligtherArrow != null)
            highligtherArrow.gameObject.SetActive(false);
    }

    public virtual void HoverTextUnit()
    {
        if (infoTextUnit != null)
        {
            infoTextUnit.SetActive(true);
        }
    }

    public virtual void HoverTextUnitExit()
    {
        if (infoTextUnit != null)
        {
            infoTextUnit.SetActive(false);
        }
    }

    //test elin DoTween shake unit
    public void ShakeUnit()
    {
        // Delay before starting the shaking animation
        float delay = 0.4f;
        //transform.DOShakePosition(duration: 0.5f, strength: new Vector3(2f, 0f, 0f), vibrato: 10, randomness: 0, fadeOut: false);
        transform.DOShakePosition(duration: 0.5f, strength: new Vector3(2f, 0f, 0f), vibrato: 10, randomness: 0, fadeOut: false)
        .SetDelay(delay);
    }
    public void OnDisable()
    {
        transform.DOKill();
    }
    public void OnDestroy()
    {
        transform.DOKill();
    }
    public void PlayIdleAnimation()
    {
        
        if (skeletonAnimation != null && !isJumping)
        {
            skeletonAnimation.AnimationState.SetAnimation(0, idleAnimation, true);
        }
    }

    // Call this method to play the jump animation
    public void PlayJumpAnimation()
    {
        if (skeletonAnimation != null)
        {
            TrackEntry entry = skeletonAnimation.AnimationState.SetAnimation(0, jumpAnimation, false);
            entry.AnimationStart = 1f;
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null && healthFill != null)
        {
            float healthPercentage = (float)healthCur / (float)healthMax;
            healthFill.fillAmount = healthPercentage;
        }
    }

    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            int healthClamp = (int)Mathf.Clamp(healthCur, 0, 10000);
            healthText.text = $"HP: {healthClamp}/{healthMax}";
        }
    }

    public void PlayActionAnimation()
    {
        // Store the initial position for later use
        Vector3 initialPosition = gfx.transform.position;

        // Define the target position for the animation
        Vector3 targetPosition = new Vector3(initialPosition.x, initialPosition.y + animationLiftHeight, initialPosition.z);
        endMovePosition = new Vector3(targetPosition.x, initialPosition.y, targetPosition.z);
        

        // Define tilt angles based on the unit's flip condition
        float tiltAngle = (gfx.transform.localScale.x < 0) ? -45f : 45f;
        float resetAngle = 0f;
        // Use DoTween to move the unit up on the y-axis
        gfx.transform.DOMove(new Vector3(initialPosition.x, initialPosition.y + animationLiftHeight, initialPosition.z), 0.2f)
            .SetEase(actionAnimEase)
            .OnComplete(() =>
            {
                // After reaching the top, tilt on the x-axis
                gfx.transform.DORotate(new Vector3(0f, 0f, tiltAngle), 0.3f)
                    .SetEase(actionAnimEase)
                    .OnComplete(() =>
                    {                  
                        // After tilting, tilt back on the x-axis
                        gfx.transform.DORotate(new Vector3(0f, 0f, resetAngle), 0.3f)
                            .SetEase(actionAnimEase)
                            .OnComplete(() =>
                            {
                                // Move back down to the initial position on the y-axis
                                gfx.transform.DOMove(new Vector3(endMovePosition.x, initialPosition.y, endMovePosition.z), 0.2f)
                                    .SetEase(actionAnimEase);                            
                            });
                    });
            });
    }

    public void FlipOnXAxis(Tile clickedTile)
    {
        if (clickedTile != null)
        {
            // Compare the clicked tile's x position with the unit's standing on tile's x position
            if (clickedTile.x > standingOn.x)
            {
                // Clicked on a tile to the right, flip the unit to face left
                gfx.localScale = new Vector3(-Mathf.Abs(gfx.localScale.x), gfx.localScale.y, gfx.localScale.z);
            }
            else if (clickedTile.x < standingOn.x)
            {
                // Clicked on a tile to the left, flip the unit to face right
                gfx.localScale = new Vector3(Mathf.Abs(gfx.localScale.x), gfx.localScale.y, gfx.localScale.z);
            }
            // If clicked on the same tile or vertically, don't change the facing direction
        }
    }

    public void IdleAnimSpriteTwo()
    {
        float originalYScale = gfx.transform.localScale.y;
        // Move the target up and down in sync with scaling
        DG.Tweening.Sequence moveSequence = DOTween.Sequence();
        moveSequence.Append(gfx.transform.DOMoveY(gfx.transform.position.y + 0.1f, 1f)/*.SetEase(Ease.InOutQuad)*/)
            .Join(gfx.transform.DOScaleY(originalYScale + 0.05f, 1f)/*.SetEase(Ease.InOutQuad)*/)
            .Append(gfx.transform.DOMoveY(gfx.transform.position.y - 0.1f, 1f)/*.SetEase(Ease.InOutQuad)*/)
            .Join(gfx.transform.DOScaleY(originalYScale, 1f)/*.SetEase(Ease.InOutQuad)*/)
            .SetLoops(-1, LoopType.Yoyo);

    }
}



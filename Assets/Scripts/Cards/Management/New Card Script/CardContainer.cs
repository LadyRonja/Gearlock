using System.Collections.Generic;
using System.Linq;
using config;
using DefaultNamespace;
using events;
using UnityEngine;
using UnityEngine.UI;

public class CardContainer : MonoBehaviour
{
    [Header("Constraints")]
    [SerializeField]
    private bool forceFitContainer;

    [Header("Alignment")]
    [SerializeField]
    private CardAlignment alignment = CardAlignment.Center;

    [SerializeField]
    private bool allowCardRepositioning = true;

    [Header("Rotation")]
    [SerializeField]
    [Range(0f, 90f)]
    private float maxCardRotation;

    [SerializeField]
    private float maxHeightDisplacement;

    [SerializeField]
    public ZoomConfig zoomConfig;

    [SerializeField]
    private AnimationSpeedConfig animationSpeedConfig;

    [SerializeField]
    private CardPlayConfig cardPlayConfig;

    [Header("Events")]
    [SerializeField]
    private EventsConfig eventsConfig;

    private List<CardWrapper> cards = new();
    private Canvas canvas;

    private RectTransform rectTransform;
    private CardWrapper currentDraggedCard;

    public static CardContainer Instance;


    public static bool clickToPlayToggle = true;
    public float panelPosYHigh;
    public float panelPosYLow;
    float bigSize = 2.7f;

    public AudioClip PlayCardSound;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }
    private void Start()
    {
        canvas = GetComponent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        InitCards();
    }

    public void InitCards()
    {
        SetUpCards();
        SetCardsAnchor();
    }

    private void SetCardsRotation()
    {
        for (var i = 0; i < cards.Count; i++)
        {
            cards[i].targetRotation = GetCardRotation(i);
            cards[i].targetVerticalDisplacement = GetCardVerticalDisplacement(i);
        }
    }

    private float GetCardVerticalDisplacement(int index)
    {
        if (cards.Count < 3) return 0;
        // Associate a vertical displacement based on the index in the cards list
        // so that the center card is at max displacement while the edges are at 0 displacement
        return maxHeightDisplacement *
               (1 - Mathf.Pow(index - (cards.Count - 1) / 2f, 2) / Mathf.Pow((cards.Count - 1) / 2f, 2));
    }

    private float GetCardRotation(int index)
    {
        if (cards.Count < 3) return 0;
        // Associate a rotation based on the index in the cards list
        // so that the first and last cards are at max rotation, mirrored around the center
        return -maxCardRotation * (index - (cards.Count - 1) / 2f) / ((cards.Count - 1) / 2f);
    }

    void Update()
    {
        UpdateCards();
        UpdatePanelSize();
        UpdatePanelPosition();

        if (Input.GetMouseButtonDown(1) && ActiveCard.Instance.transform.childCount > 0)
        {
            ActiveCard.Instance.cardBeingPlayed.CancelPlay();
            CardManager.Instance.ClearActiveCard();
        }
        if (Input.GetKeyDown(KeyCode.P))
            DataHandler.Instance.toggleClick = !DataHandler.Instance.toggleClick;
    }

    public void SetUpCards()
    {
        cards.Clear();
        foreach (Transform card in transform)
        {
            var wrapper = card.GetComponent<CardWrapper>();
            if (wrapper == null)
            {
                wrapper = card.gameObject.AddComponent<CardWrapper>();
                wrapper.enabled = true;
            }

            cards.Add(wrapper);

            AddOtherComponentsIfNeeded(wrapper);

            // Pass child card any extra config it should be aware of
            wrapper.zoomConfig = zoomConfig;
            wrapper.animationSpeedConfig = animationSpeedConfig;
            wrapper.eventsConfig = eventsConfig;
            wrapper.container = this;
        }
    }

    private void AddOtherComponentsIfNeeded(CardWrapper wrapper)
    {
        var canvas = wrapper.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = wrapper.gameObject.AddComponent<Canvas>();
        }

        canvas.overrideSorting = true;

        if (wrapper.GetComponent<GraphicRaycaster>() == null)
        {
            wrapper.gameObject.AddComponent<GraphicRaycaster>();
        }
    }

    private void UpdateCards()
    {
        if (transform.childCount != cards.Count)
        {
            InitCards();
        }

        if (cards.Count == 0)
        {
            return;
        }

        SetCardsPosition();
        SetCardsRotation();
        SetCardsUILayers();
        UpdateCardOrder();
    }

    private void SetCardsUILayers()
    {
        for (var i = 0; i < cards.Count; i++)
        {
            cards[i].uiLayer = zoomConfig.defaultSortOrder + i;
        }
    }

    private void UpdateCardOrder()
    {
        if (!DataHandler.Instance.toggleInverseCamera || currentDraggedCard == null) return;

        // Get the index of the dragged card depending on its position
        var newCardIdx = cards.Count(card => currentDraggedCard.transform.position.x > card.transform.position.x);
        var originalCardIdx = cards.IndexOf(currentDraggedCard);
        if (newCardIdx != originalCardIdx)
        {
            cards.RemoveAt(originalCardIdx);
            if (newCardIdx > originalCardIdx && newCardIdx < cards.Count - 1)
            {
                newCardIdx--;
            }

            cards.Insert(newCardIdx, currentDraggedCard);
        }
        // Also reorder in the hierarchy
        currentDraggedCard.transform.SetSiblingIndex(newCardIdx);
    }

    private void SetCardsPosition()
    {
        // Compute the total width of all the cards in global space
        var cardsTotalWidth = cards.Sum(card => card.width * card.transform.lossyScale.x);
        // Compute the width of the container in global space
        var containerWidth = rectTransform.rect.width * transform.lossyScale.x;
        if (forceFitContainer && cardsTotalWidth > containerWidth)
        {
            DistributeChildrenToFitContainer(cardsTotalWidth);
        }
        else
        {
            DistributeChildrenWithoutOverlap(cardsTotalWidth);
        }
    }

    private void DistributeChildrenToFitContainer(float childrenTotalWidth)
    {
        // Get the width of the container
        var width = rectTransform.rect.width * transform.lossyScale.x;
        // Get the distance between each child
        var distanceBetweenChildren = (width - childrenTotalWidth) / (cards.Count - 1);
        // Set all children's positions to be evenly spaced out
        var currentX = transform.position.x - width / 2;
        foreach (CardWrapper child in cards)
        {
            var adjustedChildWidth = child.width * child.transform.lossyScale.x;
            child.targetPosition = new Vector2(currentX + adjustedChildWidth / 2, transform.position.y);
            currentX += adjustedChildWidth + distanceBetweenChildren;
        }
    }

    private void DistributeChildrenWithoutOverlap(float childrenTotalWidth)
    {
        var currentPosition = GetAnchorPositionByAlignment(childrenTotalWidth);
        foreach (CardWrapper child in cards)
        {
            var adjustedChildWidth = child.width * child.transform.lossyScale.x;
            child.targetPosition = new Vector2(currentPosition + adjustedChildWidth / 2, transform.position.y);
            currentPosition += adjustedChildWidth;
        }
    }

    private float GetAnchorPositionByAlignment(float childrenWidth)
    {
        var containerWidthInGlobalSpace = rectTransform.rect.width * transform.lossyScale.x;
        switch (alignment)
        {
            case CardAlignment.Left:
                return transform.position.x - containerWidthInGlobalSpace / 2;
            case CardAlignment.Center:
                return transform.position.x - childrenWidth / 2;
            case CardAlignment.Right:
                return transform.position.x + containerWidthInGlobalSpace / 2 - childrenWidth;
            default:
                return 0;
        }
    }

    private void SetCardsAnchor()
    {
        foreach (CardWrapper child in cards)
        {
            child.SetAnchor(new Vector2(0, 0.5f), new Vector2(0, 0.5f));
        }
    }

    public void OnCardDragStart(CardWrapper card)
    {
        currentDraggedCard = card;
    }

    public void OnCardDragEnd()
    {
        AudioHandler.PlaySoundEffect(PlayCardSound);

        if (IsCursorInPlayArea() || DataHandler.Instance.toggleClick)
        {
            if (TurnManager.Instance.TurnEnd)
            {
                if (KeepCard.Instance.transform.childCount < 2)
                {
                    CardManager.Instance.siblingIndex = currentDraggedCard.transform.GetSiblingIndex();
                    CardWrapper newCard = Instantiate(currentDraggedCard, KeepCard.Instance.transform);
                    newCard.transform.localScale = new Vector3(2, 2, 2);
                    newCard.GetComponent<CardWrapper>().kept = true;


                    for (int i = 0; i < KeepCard.Instance.transform.childCount; i++)
                    {
                        KeepCard.Instance.transform.GetChild(i).gameObject.GetComponent<CardWrapper>().enabled = false;
                        // Debug.Log("disabled wrapper");
                    }

                    if (cardPlayConfig.destroyOnPlay)
                    {
                        // Destroy the original card
                        DestroyCard(currentDraggedCard);
                    }
                }

                else
                    return;
            }
            else if (MovementManager.Instance.takingMoveAction || ActiveCard.Instance.cardBeingPlayed != null)
            {
                CardManager.Instance.ClearActiveCard();
                CardManager.Instance.siblingIndex = currentDraggedCard.transform.GetSiblingIndex();
                // Instantiate a copy of the currently dragged card as a child of ActiveCard
                CardWrapper newCard = Instantiate(currentDraggedCard, ActiveCard.Instance.transform);
                newCard.transform.localScale = new Vector3(bigSize, bigSize, bigSize);
                CardManager.Instance.isDisplaying = false;
                // Set the new card as the active card
                ActiveCard.Instance.cardBeingPlayed = newCard.GetComponent<Card>();
                ActiveCard.Instance.transform.GetChild(0).gameObject.GetComponent<CardWrapper>().enabled = false;
                Invoke("SetActiveCard", 0.1f);

                if (cardPlayConfig.destroyOnPlay)
                {
                    // Destroy the original card
                    DestroyCard(currentDraggedCard);
                }
            }
        }
        currentDraggedCard = null;

    }

    public void DestroyCard(CardWrapper card)
    {
        cards.Remove(card);
        eventsConfig.OnCardDestroy?.Invoke(new CardDestroy(card));
        Destroy(card.gameObject);
    }

    private bool IsCursorInPlayArea()
    {
        if (cardPlayConfig.playArea == null) return false;

        var cursorPosition = Input.mousePosition;
        var playArea = cardPlayConfig.playArea;
        var playAreaCorners = new Vector3[4];
        playArea.GetWorldCorners(playAreaCorners);
        return cursorPosition.x > playAreaCorners[0].x &&
               cursorPosition.x < playAreaCorners[2].x &&
               cursorPosition.y > playAreaCorners[0].y &&
               cursorPosition.y < playAreaCorners[2].y;

    }

    public void SetActiveCard()
    {
        UnitSelector.Instance.UnHighlightAllTilesMoveableTo();

        ActiveCard.Instance.transform.GetChild(0).gameObject.GetComponent<Card>().Play();
        ActiveCard.Instance.transform.GetChild(0).gameObject.GetComponent<Card>().myState = CardState.VerifyUnitSelection;
    }

    private void UpdatePanelSize()
    {
        if (gameObject.GetComponent<RectTransform>() != null)
        {
            float newPanelSize = 855 - 90 * transform.childCount;

            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, gameObject.GetComponent<RectTransform>().anchoredPosition.y);
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(-newPanelSize * 2, gameObject.GetComponent<RectTransform>().sizeDelta.y);
        }
    }

    private void UpdatePanelPosition()
    {


        if (gameObject.GetComponent<RectTransform>() != null)
        {
            if (!GameoverManager.Instance.gameIsOver)
            {
                float panelRectX = gameObject.GetComponent<RectTransform>().anchoredPosition.x;
                if (TurnManager.Instance.isPlayerTurn)
                    gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(panelRectX, -Screen.height / panelPosYHigh);

                else if (!TurnManager.Instance.isPlayerTurn)
                    gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(panelRectX, -Screen.height / panelPosYLow);
            }
            else
            {
                gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -2000);
            }
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using config;
using DefaultNamespace;
using events;
using UnityEngine;
using UnityEngine.UI;

public class CardContainer : MonoBehaviour {
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
    private ZoomConfig zoomConfig;

    [SerializeField]
    private AnimationSpeedConfig animationSpeedConfig;

    [SerializeField]
    private CardPlayConfig cardPlayConfig;
    
    [Header("Events")]
    [SerializeField]
    private EventsConfig eventsConfig;
    
    private List<CardWrapper> cards = new();

    private RectTransform rectTransform;
    private CardWrapper currentDraggedCard;
    public static CardContainer Instance;

    float bigSize = 2.7f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }
    private void Start() {
        rectTransform = GetComponent<RectTransform>();
        InitCards();
    }

    private void InitCards() {
        SetUpCards();
        SetCardsAnchor();
    }

    private void SetCardsRotation() {
        for (var i = 0; i < cards.Count; i++) {
            cards[i].targetRotation = GetCardRotation(i);
            cards[i].targetVerticalDisplacement = GetCardVerticalDisplacement(i);
        }
    }

    private float GetCardVerticalDisplacement(int index) {
        if (cards.Count < 3) return 0;
        // Associate a vertical displacement based on the index in the cards list
        // so that the center card is at max displacement while the edges are at 0 displacement
        return maxHeightDisplacement *
               (1 - Mathf.Pow(index - (cards.Count - 1) / 2f, 2) / Mathf.Pow((cards.Count - 1) / 2f, 2));
    }

    private float GetCardRotation(int index) {
        if (cards.Count < 3) return 0;
        // Associate a rotation based on the index in the cards list
        // so that the first and last cards are at max rotation, mirrored around the center
        return -maxCardRotation * (index - (cards.Count - 1) / 2f) / ((cards.Count - 1) / 2f);
    }

    void Update() {
        UpdateCards();

        if (Input.GetMouseButtonDown(1) && ActiveCard.Instance.transform.childCount > 0)
        {
            CardManager.Instance.ClearActiveCard();

        }
    }

    void SetUpCards() {
        cards.Clear();
        foreach (Transform card in transform) {
            var wrapper = card.GetComponent<CardWrapper>();
            if (wrapper == null) {
                wrapper = card.gameObject.AddComponent<CardWrapper>();
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

    private void AddOtherComponentsIfNeeded(CardWrapper wrapper) {
        var canvas = wrapper.GetComponent<Canvas>();
        if (canvas == null) {
            canvas = wrapper.gameObject.AddComponent<Canvas>();
        }

        canvas.overrideSorting = true;

        if (wrapper.GetComponent<GraphicRaycaster>() == null) {
            wrapper.gameObject.AddComponent<GraphicRaycaster>();
        }
    }

    private void UpdateCards() {
        if (transform.childCount != cards.Count) {
            InitCards();
        }

        if (cards.Count == 0) {
            return;
        }

        SetCardsPosition();
        SetCardsRotation();
        SetCardsUILayers();
        UpdateCardOrder();
    }

    private void SetCardsUILayers() {
        for (var i = 0; i < cards.Count; i++) {
            cards[i].uiLayer = zoomConfig.defaultSortOrder + i;
        }
    }

    private void UpdateCardOrder() {
        if (!allowCardRepositioning || currentDraggedCard == null) return;

        // Get the index of the dragged card depending on its position
        var newCardIdx = cards.Count(card => currentDraggedCard.transform.position.x > card.transform.position.x);
        var originalCardIdx = cards.IndexOf(currentDraggedCard);
        if (newCardIdx != originalCardIdx) {
            cards.RemoveAt(originalCardIdx);
            if (newCardIdx > originalCardIdx && newCardIdx < cards.Count - 1) {
                newCardIdx--;
            }

            cards.Insert(newCardIdx, currentDraggedCard);
        }
        // Also reorder in the hierarchy
        currentDraggedCard.transform.SetSiblingIndex(newCardIdx);
    }

    private void SetCardsPosition() {
        // Compute the total width of all the cards in global space
        var cardsTotalWidth = cards.Sum(card => card.width * card.transform.lossyScale.x);
        // Compute the width of the container in global space
        var containerWidth = rectTransform.rect.width * transform.lossyScale.x;
        if (forceFitContainer && cardsTotalWidth > containerWidth) {
            DistributeChildrenToFitContainer(cardsTotalWidth);
        }
        else {
            DistributeChildrenWithoutOverlap(cardsTotalWidth);
        }
    }

    private void DistributeChildrenToFitContainer(float childrenTotalWidth) {
        // Get the width of the container
        var width = rectTransform.rect.width * transform.lossyScale.x;
        // Get the distance between each child
        var distanceBetweenChildren = (width - childrenTotalWidth) / (cards.Count - 1);
        // Set all children's positions to be evenly spaced out
        var currentX = transform.position.x - width / 2;
        foreach (CardWrapper child in cards) {
            var adjustedChildWidth = child.width * child.transform.lossyScale.x;
            child.targetPosition = new Vector2(currentX + adjustedChildWidth / 2, transform.position.y);
            currentX += adjustedChildWidth + distanceBetweenChildren;
        }
    }

    private void DistributeChildrenWithoutOverlap(float childrenTotalWidth) {
        var currentPosition = GetAnchorPositionByAlignment(childrenTotalWidth);
        foreach (CardWrapper child in cards) {
            var adjustedChildWidth = child.width * child.transform.lossyScale.x;
            child.targetPosition = new Vector2(currentPosition + adjustedChildWidth / 2, transform.position.y);
            currentPosition += adjustedChildWidth;
        }
    }

    private float GetAnchorPositionByAlignment(float childrenWidth) {
        var containerWidthInGlobalSpace = rectTransform.rect.width * transform.lossyScale.x;
        switch (alignment) {
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

    private void SetCardsAnchor() {
        foreach (CardWrapper child in cards) {
            child.SetAnchor(new Vector2(0, 0.5f), new Vector2(0, 0.5f));
        }
    }

    public void OnCardDragStart(CardWrapper card) {
        currentDraggedCard = card;
    }

    public void OnCardDragEnd() {
        // If card is in play area, play it!
        if (IsCursorInPlayArea())
        {
            CardManager.Instance.ClearActiveCard();
            CardManager.Instance.siblingIndex = currentDraggedCard.transform.GetSiblingIndex();
            // Instantiate a copy of the currently dragged card as a child of ActiveCard
            CardWrapper newCard = Instantiate(currentDraggedCard, ActiveCard.Instance.transform);
            newCard.transform.localScale = new Vector3(bigSize, bigSize, bigSize);

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
        currentDraggedCard = null;
    }
    
    public void DestroyCard(CardWrapper card) {
        cards.Remove(card);
        eventsConfig.OnCardDestroy?.Invoke(new CardDestroy(card));
        Destroy(card.gameObject);
    }

    private bool IsCursorInPlayArea() {
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

        // Play the card
        ActiveCard.Instance.transform.GetChild(0).gameObject.GetComponent<Card>().Play();
        ActiveCard.Instance.transform.GetChild(0).gameObject.GetComponent<Card>().myState = CardState.VerifyUnitSelection;

    }


}

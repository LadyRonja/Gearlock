using System.Collections;
using System.Collections.Generic;
using config;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static Card;

public class CardManager : MonoBehaviour
{
    public static CardManager instance;
    public List<GameObject> discardPile;
    public List<GameObject> drawPile;
    public GameObject dig;
    public GameObject attack;
    public GameObject attack2x;
    public GameObject diggerBot;
    public GameObject fighterBot;
    public GameObject handParent;
    public GameObject discardPileObject;
    public GameObject brokenFighter;
    public GameObject brokenDigger;
    public GameObject dynamite;
    public TextMeshProUGUI DrawAmount;
    [HideInInspector] public int siblingIndex;
    public bool isDisplaying = false;

    public bool useList = false;
    public List<GameObject> cards;


    public static CardManager Instance
    {
        get { return instance; }
        private set { instance = value; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        // The starting deck is added to the draw pile

        if(useList)
        {
            foreach (GameObject item in cards) 
            { 
                drawPile.Add(item);
            }
        }
        else
        {
            drawPile.Add(dig);
            drawPile.Add(dig);
            drawPile.Add(dig);
            drawPile.Add(attack);
            drawPile.Add(diggerBot);
            drawPile.Add(fighterBot);
        }


        DealHand();
    }


    private void Update()
    {
        // Temporary code to "play" card with space, until card plays completely with code
        if (Input.GetKeyDown(KeyCode.Space))
            CardEffectComplete();

        if (DrawAmount != null)
            DrawAmount.text = drawPile.Count.ToString();
    }
    public void DealHand()
    {
        for (int i = 0; handParent.transform.childCount < 5 && i < 5; i++)
        {

            if (drawPile.Count == 0 && discardPileObject.transform.childCount != 0)
            {
                ClearDiscard();
            }

            if (drawPile.Count >= 1)
            {
                Instantiate(drawPile[0], handParent.transform);
                drawPile.RemoveAt(0);
            }
        }

    }


    public void ClearDiscard() // removes all cards from discard, and adds them to draw pile. Shuffles draw pile.
    {
        if (discardPileObject != null)
        {
            List<GameObject> cardsToAddToDrawPile = new List<GameObject>();
            for (int i = discardPileObject.transform.childCount - 1; i >= 0; i--)
            {
                GameObject card = discardPileObject.transform.GetChild(i).gameObject;

                Card.CardType cardType = card.GetComponent<Card>().myType;

                if (cardType == Card.CardType.Dig)
                {
                    DestroyImmediate(card);
                    cardsToAddToDrawPile.Add(dig);
                }
                else if (cardType == Card.CardType.Attack)
                {
                    DestroyImmediate(card);
                    cardsToAddToDrawPile.Add(attack);
                }
                else if (cardType == Card.CardType.Attack2x)
                {
                    DestroyImmediate(card);
                    cardsToAddToDrawPile.Add(attack2x);
                }
                else if (cardType == Card.CardType.DiggerBot)
                {
                    DestroyImmediate(card);
                    cardsToAddToDrawPile.Add(diggerBot);
                }
                else if (cardType == Card.CardType.FighterBot)
                {
                    DestroyImmediate(card);
                    cardsToAddToDrawPile.Add(fighterBot);
                }
                else if (cardType == Card.CardType.Dynamite)
                {
                    DestroyImmediate(card);
                    cardsToAddToDrawPile.Add(dynamite);
                }
                else{
                    Debug.LogError("Card not identified, please refactor this function");
                }
            }

            // Add the cards to drawPile
            drawPile.AddRange(cardsToAddToDrawPile);

            ShuffleDrawPile();
            //Debug.Log(discardPileObject.transform.childCount);
        }
    }


    public void ShuffleDrawPile() // Shuffles draw pile by going randomly switching each card with another.
    {
        System.Random random = new System.Random();

        for (int i = drawPile.Count - 1; i >= 0; i--)
        {
            int j = random.Next(0, i + 1);

            GameObject temp = drawPile[i];
            drawPile[i] = drawPile[j];
            drawPile[j] = temp;
        }
    }

    public void EndTurnDiscardHand() // When the player ends their turn, any remaining cards in hand is discarded.
    {
        for (int i = 0; i < handParent.transform.childCount; i++)
        {
            GameObject CardInHand = handParent.transform.GetChild(i).gameObject;
            GameObject DiscardedCard = Instantiate(CardInHand, DiscardPile.Instance.transform);
            DiscardedCard.transform.rotation = Quaternion.identity;
            DiscardedCard.transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
            DiscardedCard.GetComponent<CardWrapper>().enabled = false;

            Destroy(CardInHand); 
        }
    }

    public void ClearActiveCard() // Any card that was being played is returned to hand.
    {
        if (ActiveCard.Instance.transform.childCount > 0)
        {
            for (int i = 0; i < 1; i++)
            {
                //Debug.Log("removing active, returning to hand");
                GameObject card = ActiveCard.Instance.transform.GetChild(0).gameObject;
                Card.CardType cardType = card.GetComponent<Card>().myType;

                if (cardType == Card.CardType.Dig)
                {
                    DestroyImmediate(card);
                    GameObject newCard = Instantiate(dig, HandPanel.Instance.transform);
                    newCard.transform.SetSiblingIndex(siblingIndex);
                }
                else if (cardType == Card.CardType.Attack)
                {
                    DestroyImmediate(card);
                    GameObject newCard = Instantiate(attack, HandPanel.Instance.transform);
                    newCard.transform.SetSiblingIndex(siblingIndex);
                }
                else if (cardType == Card.CardType.Attack2x)
                {
                    DestroyImmediate(card);
                    GameObject newCard = Instantiate(attack2x, HandPanel.Instance.transform);
                    newCard.transform.SetSiblingIndex(siblingIndex);
                }
                else if (cardType == Card.CardType.DiggerBot)
                {
                    DestroyImmediate(card);
                    GameObject newCard = Instantiate(diggerBot, HandPanel.Instance.transform);
                    newCard.transform.SetSiblingIndex(siblingIndex);
                }
                else if (cardType == Card.CardType.FighterBot)
                {
                    DestroyImmediate(card);
                    GameObject newCard = Instantiate(fighterBot, HandPanel.Instance.transform);
                    newCard.transform.SetSiblingIndex(siblingIndex);
                }
                else if (cardType == Card.CardType.Dynamite)
                {
                    DestroyImmediate(card);
                    GameObject newCard = Instantiate(dynamite, HandPanel.Instance.transform);
                    newCard.transform.SetSiblingIndex(siblingIndex);
                }
            }
        }
    }

    public void CardEffectComplete() // After the card effects have happened, discard the card.
    {
        for (int i = ActiveCard.Instance.transform.childCount - 1; i >= 0; i--)
        {
            GameObject PlayedCard = ActiveCard.Instance.transform.GetChild(i).gameObject;
            PlayedCard.GetComponent<MouseOverCard>().isBeingPlayed = false;
            PlayedCard.transform.localScale = new Vector3(2, 2, 2);


            if (!ActiveCard.Instance.cardBeingPlayed.goesToDiscardAfterPlay)
            {

                //if (PlayedCard.name == "Fighter(Clone)")
                //    Instantiate(brokenFighter, pos);

                //else if (PlayedCard.name == "Digger(Clone)")
                //    Instantiate(brokenDigger, pos);

                Destroy(PlayedCard);
            }
            else
                PlayedCard.transform.parent = DiscardPile.Instance.transform;

            //Debug.Log(PlayedCard);
        }
    }

    public void AddNewCard(GameObject cardToAdd)
    {
        GameObject newCard = Instantiate(cardToAdd, discardPileObject.transform);
        newCard.GetComponent<MouseOverCard>().inHand = false;
    }

    public void RetrieveKeptCards()
    {
        if (KeepCard.Instance.transform.childCount > 0)
        {
            for (int i = 0; i < KeepCard.Instance.transform.childCount; i++)
            {
                GameObject KeptCard = KeepCard.Instance.transform.GetChild(i).gameObject;
                GameObject ReturnedCard = Instantiate(KeptCard, HandPanel.Instance.transform);
                ReturnedCard.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);                

                for (int j = 0; j < HandPanel.Instance.transform.childCount; j++)
                {
                    HandPanel.Instance.transform.GetChild(j).gameObject.GetComponent<CardWrapper>().enabled = true;
                    HandPanel.Instance.transform.GetChild(j).gameObject.GetComponent<CardWrapper>().kept = false;
                }

                Destroy(KeptCard);
            }
        }
    }

    public void ReturnKept()
    {
        if (KeepCard.Instance.transform.childCount > 0)
        {
            for (int i = 0; i < KeepCard.Instance.transform.childCount + 1; i++)
            {
                GameObject KeptCard = KeepCard.Instance.transform.GetChild(i).gameObject;
                GameObject ReturnedCard = Instantiate(KeptCard, HandPanel.Instance.transform);

                for (int j = 0; j < HandPanel.Instance.transform.childCount; j++)
                {
                    HandPanel.Instance.transform.GetChild(j).gameObject.GetComponent<CardWrapper>().enabled = true;
                }


                Destroy(KeptCard);
            }
        }
    }
}
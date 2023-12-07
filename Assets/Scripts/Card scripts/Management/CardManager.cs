using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

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
    bool startingHand;
    public TextMeshProUGUI DrawAmount;

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

        startingHand = true; //changed from TRUE
        drawPile.Add(dig);
        drawPile.Add(dig);
        drawPile.Add(dig);
        //drawPile.Add(dig);
        drawPile.Add(attack);
        //drawPile.Add(attack);

        //drawPile.Add(attack2x);


        //drawPile.Add(diggerBot);
        drawPile.Add(diggerBot);
        drawPile.Add(fighterBot);



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
        // Gives player start hand, 1 digger and 1 dig + 3 random cards.
        //if (startingHand)
        //{
        //    //drawPile.Remove(dig);
        //    //drawPile.Remove(diggerBot);
        //    //Instantiate(dig, handParent.transform);
        //    //Instantiate(diggerBot, handParent.transform);

        //    //ShuffleDrawPile();

        //    for (int i = 0; i < 3; i++)
        //    {
        //        Instantiate(drawPile[0], handParent.transform);
        //        drawPile.RemoveAt(0);
        //    }
        //    startingHand = false;
        //}

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
            Debug.Log("discard script executed");
            for (int i = discardPileObject.transform.childCount - 1; i >= 0; i--)
            {
                GameObject card = discardPileObject.transform.GetChild(i).gameObject;

                if (card.name == "Dig(Clone)")
                {
                    DestroyImmediate(card);
                    cardsToAddToDrawPile.Add(dig);
                }
                else if (card.name == "Attack(Clone)")
                {
                    DestroyImmediate(card);
                    cardsToAddToDrawPile.Add(attack);
                }
                else if (card.name == "Attack2x(Clone)")
                {
                    DestroyImmediate(card);
                    cardsToAddToDrawPile.Add(attack2x);
                }
                else if (card.name == "Digger Bot(Clone)")
                {
                    DestroyImmediate(card);
                    cardsToAddToDrawPile.Add(diggerBot);
                }
                else if (card.name == "Fighter Bot(Clone)")
                {
                    DestroyImmediate(card);
                    cardsToAddToDrawPile.Add(fighterBot);
                }
                else if (card.name == "Dynamite(Clone)")
                {
                    DestroyImmediate(card);
                    cardsToAddToDrawPile.Add(dynamite);
                }
            }

            // Add the cards to drawPile
            drawPile.AddRange(cardsToAddToDrawPile);

            ShuffleDrawPile();
            Debug.Log(discardPileObject.transform.childCount);
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
        for (int i = handParent.transform.childCount - 1; i >= 0; i--)
        {
            GameObject CardInHand = handParent.transform.GetChild(i).gameObject;
            CardInHand.transform.parent = DiscardPile.Instance.transform;
            CardInHand.GetComponent<MouseOverCard>().inHand = false;
        }
    }

    public void ClearActiveCard() // Any card that was being played is returned to hand.
    {
        for (int i = ActiveCard.Instance.transform.childCount - 1; i >= 0; i--)
        {
            GameObject CardBeingPlayed = ActiveCard.Instance.transform.GetChild(i).gameObject;
            CardBeingPlayed.transform.parent = HandPanel.Instance.transform;
            CardBeingPlayed.transform.localScale = new Vector3(2, 2, 2);
            CardBeingPlayed.GetComponent<MouseOverCard>().inHand = true;
            CardBeingPlayed.GetComponent<MouseOverCard>().isBeingPlayed = false;
            ActiveCard.Instance.cardBeingPlayed = null;
        }
    }

    public void CardEffectComplete() // After the card effects have happened, discard the card.
    {
        for (int i = ActiveCard.Instance.transform.childCount - 1; i >= 0; i--)
        {
            GameObject PlayedCard = ActiveCard.Instance.transform.GetChild(i).gameObject;
            PlayedCard.GetComponent<MouseOverCard>().isBeingPlayed = false;

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

            Debug.Log(PlayedCard);
        }
    }

    public void AddNewCard(GameObject cardToAdd)
    {
        GameObject newCard = Instantiate(cardToAdd, discardPileObject.transform);
        newCard.GetComponent<MouseOverCard>().inHand = false;
    }

    public void RetrieveKeptCards()
    {
        for (int i = KeepCard.Instance.transform.childCount - 1; i >= 0; i--)
        {
            GameObject keptCard = KeepCard.Instance.transform.GetChild(i).gameObject;
            keptCard.transform.parent = HandPanel.Instance.transform;
            keptCard.GetComponent<MouseOverCard>().inHand = true;
        }
    }

    public void ReturnKept()
    {
        if (KeepCard.Instance.transform.childCount > 0)
        {
            for (int i = 0; i < KeepCard.Instance.transform.childCount -1; i++)
            {
                GameObject Keptcard = KeepCard.Instance.transform.GetChild(0).gameObject;
                Keptcard.transform.parent = HandPanel.Instance.transform;
                //Keptcard.GetComponent<MouseOverCard>().keeping = false;
                Keptcard.GetComponent<MouseOverCard>().inHand = true;
            }
        }
    }
}

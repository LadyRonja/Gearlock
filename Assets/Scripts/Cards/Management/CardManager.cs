using System.Collections;
using System.Collections.Generic;
using config;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static Card;
using static UnityEngine.GraphicsBuffer;
using DG.Tweening;

public class CardManager : MonoBehaviour
{
    public static CardManager instance;

    [Header("GameObjects")]
    public GameObject dig;
    public GameObject attack;
    public GameObject attack2x;
    public GameObject diggerBot;
    public GameObject fighterBot;
    public GameObject dynamite;
    
    [HideInInspector] public int digInDiscard;
    [HideInInspector] public int attackInDiscard;
    [HideInInspector] public int attack2xInDiscard;
    [HideInInspector] public int diggerInDiscard;
    [HideInInspector] public int fighterInDiscard;
    [HideInInspector] public int dynamiteInDiscard;
    [HideInInspector] public int totalCardsInDiscard;

    [Header("References")]
    public GameObject handParent;
    public GameObject discardPileObject;
    public GameObject brokenFighter;
    public GameObject brokenDigger;
    public GameObject genericCard;
    public GameObject drawSpawnPosition;
    public GameObject drawPileText;
    public GameObject discardPileText;

    [Header("Transforms")]
    public Transform discardIcon;
    public Transform drawIcon;
    public Transform discardSpawn;
    public TextMeshProUGUI DrawAmount;
    [HideInInspector] public int siblingIndex;

    [Header("AudioClips")]
    public AudioClip PlayShuffleSound;
    public AudioClip PlayDrawSound;

    [Header("Eases")]
    public Ease cardEase;
    public Ease drawEase;

    [Header("Booleans")]
    public bool isDisplaying = false;
    public bool useList = false;
    public bool cardChoice = false;

    [Header("Lists")]
    public List<GameObject> discardPile;
    public List<GameObject> drawPile;
    public List<GameObject> cards;



    private Dictionary<Card.CardType, GameObject> cardTypeToPrefab;

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
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }


        cardTypeToPrefab = new Dictionary<Card.CardType, GameObject>
        {
            { Card.CardType.Dig, dig },
            { Card.CardType.Attack, attack },
            { Card.CardType.Attack2x, attack2x },
            { Card.CardType.DiggerBot, diggerBot },
            { Card.CardType.FighterBot, fighterBot },
            { Card.CardType.Dynamite, dynamite }
        };
    }


    void Start()
    {
        // Do not deal on tutorial
        if (TutorialBasic.Instance.IsInTutorial)
            return;
        else if (TutorialAdvanced.Instance.IsInTutorial)
            return;

        SetUpStartHand();
    }


    private void Update()
    {
        // Temporary code to "play" card with space, until card plays completely with code
        //if (Input.GetKeyDown(KeyCode.Space))
        //    CardEffectComplete();

        if (DrawAmount != null)
            DrawAmount.text = drawPile.Count.ToString();

        drawPileText.GetComponent<TextMeshProUGUI>().text = drawPile.Count.ToString();
        discardPileText.GetComponent<TextMeshProUGUI>().text = discardPile.Count.ToString();
        //DrawPile.Instance.UpdateDrawDisplay();
    }

    public void SetUpStartHand()
    {
        // The starting deck is added to the draw pile

        if (useList)
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
            drawPile.Add(diggerBot);
            drawPile.Add(attack);
            drawPile.Add(attack);
            drawPile.Add(dynamite);
            //drawPile.Add(fighterBot);
        }

        DealHand();
    }

    public void DealHand()
    {
        AudioHandler.PlaySoundEffect(PlayShuffleSound);

        for (int i = 0; handParent.transform.childCount < 5 && i < 5; i++)
        {
            if (drawPile.Count == 0 && discardPile.Count > 0)
            {
                ClearDiscard();
            }

            if (drawPile.Count >= 1)
            {
                GameObject cardClone = Instantiate(drawPile[0], drawSpawnPosition.transform.position, Quaternion.identity);
                cardClone.transform.SetParent(handParent.transform, false);
                cardClone.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                cardClone.transform.DOScale(new Vector3(2f, 2f, 2f), 0.8f).SetEase(drawEase);
                drawPile.RemoveAt(0);
            }
        }

    }


    public void ClearDiscard()
    {
        if (discardPile.Count > 0)
        {
            StartCoroutine(AnimateCardToDrawPileWithDelay(discardPile.Count));

            drawPile.AddRange(discardPile);
            discardPile.Clear();

            ShuffleDrawPile();
        }
        DiscardPile.Instance.UpdateDiscardDisplay();
    }

    public void AnimateCardToDrawPile()
    {

        // Instantiate a copy of the card
        GameObject cardCopy = Instantiate(genericCard, discardSpawn.transform);
        cardCopy.transform.position = discardIcon.transform.position;
        cardCopy.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        cardCopy.transform.SetAsFirstSibling();
        //cardCopy.GetComponent<CardWrapper>().enabled = false;

        //Parent the copy to the drawPileTransform
        //cardCopy.transform.parent = drawPileTransform;

        // Define control point for the Bezier curve
        Vector3 startPos = cardCopy.transform.position;
        Vector3 endPos = drawIcon.transform.position;
        Vector3 controlPoint = (startPos + endPos) / 2 + Vector3.up * 80.0f;

        // Animate the copy along the Bezier curve using DOTween
        cardCopy.transform.DOPath(new Vector3[] { startPos, controlPoint, endPos }, 1, PathType.CatmullRom)
            .SetEase(Ease.OutQuint)  // You can adjust the ease function as needed
            .OnComplete(() => Destroy(cardCopy))
            .SetDelay(0.1f);   // Destroy the copy when the animation is complete

    }

    public void ShuffleDrawPile() // Shuffles draw pile by going randomly switching each card with another.
    {
        System.Random random = new System.Random();

        //AudioHandler.PlaySoundEffect(PlayShuffleSound);

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
            //GameObject DiscardedCard = Instantiate(CardInHand, DiscardPile.Instance.transform);
            //DiscardedCard.transform.rotation = Quaternion.identity;
            //DiscardedCard.transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
            //DiscardedCard.GetComponent<CardWrapper>().enabled = false;

            Card.CardType cardType = CardInHand.GetComponent<Card>().myType;
            discardPile.Add(cardTypeToPrefab[cardType]);

            Destroy(CardInHand);
        }
        DiscardPile.Instance.UpdateDiscardDisplay();
    }

    public void ClearActiveCard()
    {
        if (ActiveCard.Instance.transform.childCount > 0)
        {
            GameObject playedCard = ActiveCard.Instance.transform.GetChild(0).gameObject;
            Card.CardType cardType = playedCard.GetComponent<Card>().myType;

            if (cardTypeToPrefab.TryGetValue(cardType, out GameObject prefab))
            {
                DestroyImmediate(playedCard);

                GameObject newCard = Instantiate(prefab, HandPanel.Instance.transform);
                newCard.transform.SetSiblingIndex(siblingIndex);
            }
            else
            {
                Debug.LogError("Card type not found in dictionary.");
            }
        }
    }

    public void CardEffectComplete() // After the card effects have happened, discard the card.
    {
        //for (int i = ActiveCard.Instance.transform.childCount - 1; i >= 0; i--)
        //{
        GameObject playedCard = ActiveCard.Instance.transform.GetChild(0).gameObject;
        Card.CardType cardType = playedCard.GetComponent<Card>().myType;
        if (cardType != Card.CardType.DiggerBot && cardType != Card.CardType.FighterBot && cardType != Card.CardType.Dynamite)
            discardPile.Add(cardTypeToPrefab[cardType]);
        Destroy(playedCard);

        DiscardPile.Instance.UpdateDiscardDisplay();
    }

    public void AddNewCard(GameObject cardToAdd)
    {
        //GameObject newCard = Instantiate(cardToAdd, discardPileObject.transform);
        //newCard.GetComponent<MouseOverCard>().inHand = false;
        Card.CardType cardType = cardToAdd.GetComponent<Card>().myType;

        discardPile.Add(cardTypeToPrefab[cardType]);

        GameObject toDiscard = Instantiate(cardToAdd, AddedToDiscard.Instance.transform);
        toDiscard.transform.DOMove(new Vector3(discardIcon.position.x, discardIcon.position.y, discardIcon.position.z), 1).SetEase(cardEase);
        toDiscard.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 1).SetEase(cardEase);
        Destroy(toDiscard, 1);

        DiscardPile.Instance.UpdateDiscardDisplay();
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

    private IEnumerator AnimateCardToDrawPileWithDelay(int children)
    {
        for (int i = 0; i < children; i++)
        {
            AnimateCardToDrawPile();
            yield return new WaitForSeconds(0.05f);
        }

    }

}

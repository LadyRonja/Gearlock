using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField] private static CardManager instance;
    public List<GameObject> discardPile;
    public List<GameObject> drawPile;
    public GameObject dig;
    public GameObject attack;
    public GameObject attack2x;
    public GameObject diggerBot;
    public GameObject fighterBot;
    public GameObject handParent;
    bool startingHand;
    public GameObject discardPileObject;

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
        startingHand = true;
        drawPile.Add(dig);
        drawPile.Add(dig);
        drawPile.Add(dig);
        drawPile.Add(dig);
        drawPile.Add(attack);
        drawPile.Add(attack);
        drawPile.Add(attack2x);
        drawPile.Add(diggerBot);
        drawPile.Add(diggerBot);
        drawPile.Add(fighterBot);
    }

    public void DealHand()
    {
        if (startingHand)
        {
            drawPile.Remove(dig);
            drawPile.Remove(diggerBot);
            Instantiate(dig, handParent.transform);
            Instantiate(diggerBot, handParent.transform);

            ShuffleDrawPile();

            for (int i  = 0; i < 3; i++)
            {
                Instantiate(drawPile[0], handParent.transform);
                drawPile.RemoveAt(0);
            }
            startingHand = false;
        }

        else
        {
            for (int i = 0; i < 5; i++)
            {
                if (drawPile.Count < 1)
                    ClearDiscard();

                Instantiate(drawPile[0], handParent.transform);
                drawPile.RemoveAt(0);


            }
        }

    }


    public void ClearDiscard()
    {

        if (discardPileObject != null)
        {
            List<GameObject> cardsToAddToDrawPile = new List<GameObject>();

            for (int i = discardPileObject.transform.childCount - 1; i >= 0; i--)
            {
                GameObject card = discardPileObject.transform.GetChild(i).gameObject;

                if (card.name == "Dig(Clone)")
                {
                    Destroy(card);
                    cardsToAddToDrawPile.Add(dig);
                }
                else if (card.name == "Attack(Clone)")
                {
                    Destroy(card);
                    cardsToAddToDrawPile.Add(attack);
                }
                else if (card.name == "Attack2x(Clone)")
                {
                    Destroy(card);
                    cardsToAddToDrawPile.Add(attack2x);
                }
                else if (card.name == "Digger(Clone)")
                {
                    Destroy(card);
                    cardsToAddToDrawPile.Add(diggerBot);
                }
                else if (card.name == "Fighter(Clone)")
                {
                    Destroy(card);
                    cardsToAddToDrawPile.Add(fighterBot);
                }
            }

            // Add the cards to drawPile
            drawPile.AddRange(cardsToAddToDrawPile);

            ShuffleDrawPile();
        }
    }


    public void ShuffleDrawPile()
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

    public void EndTurnDiscardHand()
    {
        for (int i = handParent.transform.childCount - 1; i >= 0; i--)
        {
            GameObject CardInHand = handParent.transform.GetChild(i).gameObject;
            CardInHand.transform.parent = DiscardPile.Instance.transform;
        }
    }

}

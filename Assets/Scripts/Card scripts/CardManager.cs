using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardManager : MonoBehaviour
{

    public List<GameObject> discardPile;
    public List<GameObject> drawPile;
    public GameObject dig;
    public GameObject attack;
    public GameObject attack2x;
    public GameObject diggerBot;
    public GameObject fighterBot;
    public GameObject handParent;
    bool startingHand;

    public static CardManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
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

            for (int i  = 0; i < 3; i++)
            {
                int randomCard = Random.Range(0, drawPile.Count -1);
                Instantiate(drawPile[randomCard], handParent.transform);
                drawPile.RemoveAt(randomCard);
            }
            startingHand = false;
        }

        else
        {
            for (int i = 0; i < 5; i++)
            {
                if (drawPile.Count < 1)
                    break;

                int randomCard = Random.Range(0, drawPile.Count -1);
                Instantiate(drawPile[randomCard], handParent.transform);
                drawPile.RemoveAt(randomCard);
                CheckDrawPileSize();
            }
        }

    }


    void CheckDrawPileSize()
    {
        if (drawPile.Count < 1)
        {
            //TODO: shuffle discard pile and add to draw pile
            return;
        }
    }

}

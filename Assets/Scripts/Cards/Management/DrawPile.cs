using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;

public class DrawPile : MonoBehaviour
{
    public GameObject counterCircle;
    public GameObject counterText;

    public int digCards = 0;
    public int dynamiteCards = 0;
    public int attackCards = 0;
    public int attackTwiceCards = 0;
    public int diggerBotCards = 0;
    public int fighterBotCards = 0;
    public int totalCards = 0;

    public static DrawPile Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }



    public void CountCardsInDraw()
    {
        ResetCount();
        foreach (GameObject obj in CardManager.Instance.drawPile)
        {
            totalCards++;
            Card.CardType cardType = obj.GetComponent<Card>().myType;

            if      (cardType == Card.CardType.Dig) digCards++;
            else if (cardType == Card.CardType.Dynamite) dynamiteCards++;
            else if (cardType == Card.CardType.Attack) attackCards++;
            else if (cardType == Card.CardType.Attack2x) attackTwiceCards++;
            else if (cardType == Card.CardType.DiggerBot) diggerBotCards++;
            else if (cardType == Card.CardType.FighterBot) fighterBotCards++;
        }
    }

    public void ResetCount()
    {
        digCards = 0;
        dynamiteCards = 0;
        attackCards = 0;
        attackTwiceCards = 0;
        fighterBotCards = 0;
        diggerBotCards = 0;

        totalCards = 0;
    }

    public void UpdateDrawDisplay()
    {
        CountCardsInDraw();

        for (var i = gameObject.transform.childCount - 1; i >= 0; i--)
            Destroy(gameObject.transform.GetChild(i).gameObject);

        if (digCards > 0)
        {
            GameObject digCard = Instantiate(CardManager.Instance.dig, gameObject.transform);
            GameObject digCounter = Instantiate(counterCircle, digCard.transform);
            digCounter.transform.localScale = Vector3.one * 0.2f;
            GameObject digCountText = Instantiate(counterText, digCard.transform);
            digCountText.transform.localScale = Vector3.one * 0.4f;
            digCountText.GetComponent<TextMeshProUGUI>().text = digCards.ToString();

        }
        if (attackCards > 0)
        {
            GameObject attackCard = Instantiate(CardManager.Instance.attack, gameObject.transform);
            GameObject attackCounter = Instantiate(counterCircle, attackCard.transform);
            attackCounter.transform.localScale = Vector3.one * 0.2f;
            GameObject attackCountText = Instantiate(counterText, attackCard.transform);
            attackCountText.transform.localScale = Vector3.one * 0.4f;
            attackCountText.GetComponent<TextMeshProUGUI>().text = attackCards.ToString();
        }
        if (attackTwiceCards > 0)
        {
            GameObject attackTwiceCard = Instantiate(CardManager.Instance.attack2x, gameObject.transform);
            GameObject Counter = Instantiate(counterCircle, attackTwiceCard.transform);
            Counter.transform.localScale = Vector3.one * 0.2f;
            GameObject CountText = Instantiate(counterText, attackTwiceCard.transform);
            CountText.transform.localScale = Vector3.one * 0.4f;
            CountText.GetComponent<TextMeshProUGUI>().text = attackTwiceCards.ToString();
        }
        if (dynamiteCards > 0)
        {
            GameObject dynamiteCard = Instantiate(CardManager.Instance.dynamite, gameObject.transform);
            GameObject Counter = Instantiate(counterCircle, dynamiteCard.transform);
            Counter.transform.localScale = Vector3.one * 0.2f;
            GameObject CountText = Instantiate(counterText, dynamiteCard.transform);
            CountText.transform.localScale = Vector3.one * 0.4f;
            CountText.GetComponent<TextMeshProUGUI>().text = dynamiteCards.ToString();
        }
        if (diggerBotCards > 0)
        {
            GameObject diggerCard = Instantiate(CardManager.Instance.diggerBot, gameObject.transform);
            GameObject Counter = Instantiate(counterCircle, diggerCard.transform);
            Counter.transform.localScale = Vector3.one * 0.2f;
            GameObject CountText = Instantiate(counterText, diggerCard.transform);
            CountText.transform.localScale = Vector3.one * 0.4f;
            CountText.GetComponent<TextMeshProUGUI>().text = diggerBotCards.ToString();
        }
        if (fighterBotCards > 0)
        {
            GameObject fighterCard = Instantiate(CardManager.Instance.fighterBot, gameObject.transform);
            GameObject Counter = Instantiate(counterCircle, fighterCard.transform);
            Counter.transform.localScale = Vector3.one * 0.2f;
            GameObject CountText = Instantiate(counterText, fighterCard.transform);
            CountText.transform.localScale = Vector3.one * 0.4f;
            CountText.GetComponent<TextMeshProUGUI>().text = fighterBotCards.ToString();
        }

    }



}

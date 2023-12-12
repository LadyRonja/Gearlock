using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardVisualSetup : MonoBehaviour
{

    [Header("Regions for display")]
    [SerializeField]
    public TMP_Text nameRegion;
    public TMP_Text nameShadow;
    public Image frameImage;
    public Image illustrationImage;
    public TMP_Text descriptionRegion;
    public TMP_Text descriptionShadow;
    public TMP_Text rangeRegion;
    public TMP_Text rangeShadow;

    // Start is called before the first frame update
    void Start()
    {
        SetUpCard();
    }

    private void SetUpCard()
    {
        PlayCard cardInfo = GetComponent<PlayCard>();
        if (cardInfo == null)
        {
            Debug.LogError("Card Missing Card Data");
            return;
        }

        nameRegion.text = cardInfo.cardName;
        nameShadow.text = cardInfo.cardName;
        frameImage.sprite = cardInfo.cardFrame;
        frameImage.color = cardInfo.frameColor;
        illustrationImage.sprite = cardInfo.illustration;
        descriptionRegion.text = cardInfo.cardDescription;
        descriptionShadow.text = cardInfo.cardDescription;
        rangeRegion.text = cardInfo.range.ToString();
        rangeShadow.text = cardInfo.range.ToString();

    }
}

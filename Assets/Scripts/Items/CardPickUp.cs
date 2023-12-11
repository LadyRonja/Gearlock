using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardPickUp : MonoBehaviour
{
    public GameObject cardToAdd;
    public SpriteRenderer frameSR;
    public SpriteRenderer illustrationSR;
    public AnimationCurve myCurve;
    float yOffSet = 0f;
    bool fixedGFX = false;
    Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
       
    }

    void Update()
    {
        if(!fixedGFX)
            CardLayout();

        yOffSet = myCurve.Evaluate(Time.time % myCurve.length);
        transform.position = new Vector3(startPos.x, startPos.y + yOffSet, startPos.z);
    }

    public void CardLayout()
    {
        PlayCard cardLayout = cardToAdd.GetComponent<PlayCard>();
        if (cardLayout == null)
        {
            Debug.LogError("Card Missing Card Data");
            return;
        }

        
        frameSR.sprite = cardLayout.cardFrame;
        frameSR.color = cardLayout.frameColor;
        illustrationSR.sprite = cardLayout.illustration;

        fixedGFX = true;
    }
    
}

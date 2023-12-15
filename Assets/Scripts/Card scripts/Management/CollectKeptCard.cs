using System.Collections;
using System.Collections.Generic;
using events;
using config;
using UnityEngine;
using UnityEngine.EventSystems;

public class CollectKeptCard : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (GetComponent<CardWrapper>().kept)
        {
            GameObject KeptCard = gameObject;
            GameObject ReturnedCard = Instantiate(KeptCard, HandPanel.Instance.transform);
            ReturnedCard.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);

            for (int j = 0; j < HandPanel.Instance.transform.childCount; j++)
            {
                HandPanel.Instance.transform.GetChild(j).gameObject.GetComponent<CardWrapper>().enabled = true;
                HandPanel.Instance.transform.GetChild(j).gameObject.GetComponent<CardWrapper>().kept = false;
            }

            Destroy(KeptCard);
        }
        else
            return;
    }
}

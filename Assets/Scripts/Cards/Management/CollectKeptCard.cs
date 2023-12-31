using System.Collections;
using System.Collections.Generic;
using events;
using config;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CollectKeptCard : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (gameObject.transform.parent.gameObject != KeepCard.Instance.transform.gameObject)
            return;

        if (GetComponent<CardWrapper>().kept)
        {
            GameObject KeptCard = gameObject;
            GameObject ReturnedCard = Instantiate(KeptCard, HandPanel.Instance.transform);
            ReturnedCard.transform.position = CardManager.Instance.drawIcon.transform.position;
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

    public void OnDestroy()
    {
        transform.DOKill();
    }
}

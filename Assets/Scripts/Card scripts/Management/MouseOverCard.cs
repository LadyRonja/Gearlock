using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;

public class MouseOverCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    bool hovering = false;
    float cardX, cardY;
    bool isDragged = false;
    public bool inHand = true;
    public bool isBeingPlayed = false;
    Vector3 dragPos;
    //DiscardShow discardShow;

    public void Update()
    {
        if (Input.GetMouseButton(0) && hovering)
        {
            isDragged = true;
            dragPos.x = Input.mousePosition.x;
            dragPos.y = Input.mousePosition.y - 50;
            transform.position = dragPos;
        }

        if (Input.mousePosition.y > 300 && isDragged && !Input.GetMouseButton(0))
        {
            CardManager.Instance.ClearActiveCard();
            this.gameObject.GetComponent<PlayCard>().Play();
            transform.parent = ActiveCard.Instance.transform;
            inHand = false;
            isBeingPlayed = true;
        }

        if (isDragged && !Input.GetMouseButton(0))
        {
            transform.position = new Vector3(cardX, cardY, 0);
            hovering = false;
            isDragged = false;
        }

        if (isBeingPlayed)
        {
            if (Input.GetMouseButtonDown(1))
            {
                transform.parent = HandPanel.Instance.transform;
                transform.position = new Vector3(cardX, cardY, 0);
                inHand = true;
                isBeingPlayed= false;
            }
        }
    }

    //Detect if the Cursor starts to pass over the GameObject
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (!isDragged)
        {
            cardX = transform.position.x;
            cardY = transform.position.y;
        }

        //Output to console the GameObject's name and the following message
        Debug.Log("Cursor Entering " + name + " GameObject");
        if (!hovering && inHand)
        {
            transform.position = new Vector3(cardX,cardY + 100,0);
            hovering = true;
        }
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        //Output the following message with the GameObject's name
        Debug.Log("Cursor Exiting " + name + " GameObject");

        if (hovering && !isDragged)
        {
            transform.position = new Vector3(cardX, cardY, 0);
            hovering = false;
        }


    }

}
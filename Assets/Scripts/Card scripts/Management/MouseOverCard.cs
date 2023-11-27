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

    public void Update()
    {
        // If left mouse button is held when above a card in hand, the card is then dragged, following the mouse cursor.
        if (Input.GetMouseButton(0) && hovering)
        {
            isDragged = true;
            dragPos.x = Input.mousePosition.x;
            dragPos.y = Input.mousePosition.y - 50;
            transform.position = dragPos;
        }

        // If card is dropped in the play area, it is set as active. Any other card that was active returns to hand
        if (Input.mousePosition.y > 300 && isDragged && !Input.GetMouseButton(0))
        {
            CardManager.Instance.ClearActiveCard();
            //this.gameObject.GetComponent<PlayCard>().Play();
            transform.parent = ActiveCard.Instance.transform;
            inHand = false;
            isBeingPlayed = true;
        }

        // If card is dragged and dropped in the outside the play area, the card returns to hand
        if (isDragged && !Input.GetMouseButton(0))
        {
            transform.position = new Vector3(cardX, cardY, 0);
            hovering = false;
            isDragged = false;
        }


        // If a card is currently in the process of being played, right-clicking will return it to hand
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
        // Saves the cards coordinates, to position it correctly when it is raised.
        if (!isDragged)
        {
            cardX = transform.position.x;
            cardY = transform.position.y;
        }

        // Raises the card to display it more clearly.
        if (!hovering && inHand)
        {
            transform.position = new Vector3(cardX,cardY + 100,0);
            hovering = true;
        }
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        // returns the card to original position
        if (hovering && !isDragged)
        {
            transform.position = new Vector3(cardX, cardY, 0);
            hovering = false;
        }


    }

}
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    bool hovering = false;
    float cardX, cardY;
    bool isDragged = false;
    bool inHand = true;
    Vector3 dragPos;
    //CardManager cardManager;

    private void Start()
    {
        //cardManager = FindObjectOfType<CardManager>();

        
    }

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
            transform.parent = DiscardPile.Instance.transform;
            inHand = false;
        }

        if (isDragged && !Input.GetMouseButton(0))
        {
            transform.position = new Vector3(cardX, cardY, 0);
            hovering = false;
            isDragged = false;
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
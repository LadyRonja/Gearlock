using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    bool hovering = false;
    float cardX, cardY;
    bool isDragged = false;
    Vector3 dragStartPos;
    Vector3 dragPos;
    public Transform discardPile;


    public void Update()
    {
        if (Input.GetMouseButtonDown(0) && hovering)
        {
            dragStartPos = new Vector2(transform.localPosition.x, transform.localPosition.y);
        }

        if (Input.GetMouseButton(0) && hovering)
        {
            isDragged = true;
            dragPos.x = Input.mousePosition.x;
            dragPos.y = Input.mousePosition.y - 50;
            transform.position = dragPos;
        }

        if (Input.mousePosition.y > 300 && isDragged && !Input.GetMouseButton(0))
        {
            Destroy(gameObject);
            Instantiate(gameObject, new Vector3(80f, 50f, 0f), Quaternion.identity);
        }

        if (isDragged && !Input.GetMouseButton(0))
        {
            transform.position = new Vector3(cardX, cardY, 0);
            hovering = false;
            isDragged = false;
        }


        Debug.Log(Input.mousePosition.y);
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
        if (!hovering)
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
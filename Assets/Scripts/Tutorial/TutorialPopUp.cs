using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPopUp : MonoBehaviour
{
    public bool firstTutorial = true;
    public Image myBackground;
    public TMP_Text myText;

    private bool cameraMoved = false;

    private void Update()
    {
        if (firstTutorial)
        {
            if (ActiveCard.Instance.cardBeingPlayed != null)
                Destroy(this.gameObject);
        }
        else
        {
            if(!cameraMoved) 
            {
                if (Input.GetKeyDown(KeyCode.W) ||
                    Input.GetKeyDown(KeyCode.A) ||
                    Input.GetKeyDown(KeyCode.S) ||
                    Input.GetKeyDown(KeyCode.D) ||
                    UnitSelector.Instance.selectedUnit != null || ActiveCard.Instance.cardBeingPlayed != null)
                {
                    cameraMoved = true;
                    myBackground.color = new Color(0, 0, 0, 0);
                    myText.color = new Color(0, 0, 0, 0);
                    myText.text = "You can also de-select Cards and Robots with Right Click";
                }
            }
            else
            {
                if(UnitSelector.Instance.selectedUnit != null || ActiveCard.Instance.cardBeingPlayed != null)
                {
                    myBackground.color = Color.white;
                    myText.color = Color.white;
                }

                if (Input.GetMouseButtonDown((int)MouseButton.Right))
                    Destroy(this.gameObject);
            }


        }

    }
}

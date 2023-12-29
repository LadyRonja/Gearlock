using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DrawShow : MonoBehaviour
{
    public bool displayingDraw;
    public GameObject discardPile;
    public GameObject drawPile;
    public GameObject keepPanel;
    public bool hasBeenClosedOnce = false;

    public static DrawShow Instance;


    private void Awake()
    {
        if (Instance == null || Instance == this)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    private void Start()
    {
        drawPile.SetActive(false);
    }

    public void ToggleDisplay()
    {
        discardPile.SetActive(false);
        drawPile.SetActive(!drawPile.activeSelf);
        if (TurnManager.Instance.keepPhase)
            keepPanel.SetActive(!keepPanel.activeSelf);

        // TODO: Fix tutorial coupling
        if (!drawPile.activeSelf)
            hasBeenClosedOnce = true;
        else if (TutorialBasic.Instance.IsInTutorial)
        {
            if (!hasBeenClosedOnce)
            {
                TutorialBasic.Instance.CloseSpecificPage(11);
            }
        }
    }
}

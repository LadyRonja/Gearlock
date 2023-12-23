using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DiscardShow : MonoBehaviour
{
    public bool displayingDiscard;
    public GameObject discardPile;
    public GameObject keepPanel;
    public bool hasBeenClosedOnce = false;

    public static DiscardShow Instance;


    private void Awake()
    {
        if (Instance == null || Instance == this)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    private void Start()
    {
        discardPile.SetActive(false);
    }

    public void ToggleDisplay()
    {
        discardPile.SetActive(!discardPile.activeSelf);
        if (TurnManager.Instance.keepPhase)
            keepPanel.SetActive(!keepPanel.activeSelf);

        // TODO: Fix tutorial coupling
        if(!discardPile.activeSelf)
            hasBeenClosedOnce = true;
        else if (TutorialBasic.Instance.IsInTutorial)
        {
            if(!hasBeenClosedOnce)
            {
                TutorialBasic.Instance.CloseSpecificPage(11);
            }
        }
    }
}

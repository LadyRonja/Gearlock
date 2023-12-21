using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DiscardShow : MonoBehaviour
{
    public bool displayingDiscard;
    public GameObject discardPile;
    public GameObject keepPanel;

    // Start is called before the first frame update

    private void Start()
    {
        discardPile.SetActive(false);
    }

    public void ToggleDisplay()
    {
        discardPile.SetActive(!discardPile.activeSelf);
        if (TurnManager.Instance.keepPhase)
            keepPanel.SetActive(!keepPanel.activeSelf);
    }
}

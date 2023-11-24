using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour // classen blir en singleton
{
    public static TurnManager Instance;
    public bool isPlayerTurn = true;

    [SerializeField] TMP_Text tempTurnText;

    public void Awake()
    {
        #region Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
        #endregion

        UpdateUI();

    }
    public void EndTurn() //when clicked on End Turn Button
    {
        if (isPlayerTurn)
        {
            isPlayerTurn = false;

            // Hurts my soul to write a nullcheck on a singleton
            if(CardManager.Instance != null)
                CardManager.Instance.EndTurnDiscardHand();

            MovementManager.Instance.takingMoveAction = false;
            UnitSelector.Instance.playerCanSelectNewUnit = false;

            UpdateUI();
            AIManager.Instance.StartAITurn();

            // Hurts my soul to write a nullcheck on a singleton
            if (CardManager.Instance != null)
                CardManager.Instance.DealHand();

            //TODO disable player interaction
        }
        
    }

    public void GoToPlayerTurn()
    {
        if (isPlayerTurn)
        {
            Debug.LogError("Something tried to pass to the player, during the players turn");
            return;
        }



        isPlayerTurn = true;
        MovementManager.Instance.takingMoveAction = true; // Change later
        UnitSelector.Instance.playerCanSelectNewUnit = true;
        foreach (Unit u in UnitStorage.Instance.playerUnits)
        {
            u.movePointsCur = u.movePointsMax;
        }
        UpdateUI();
    }


    public void UpdateUI()
    {
        if (tempTurnText == null) return;

        if (isPlayerTurn) tempTurnText.text = "Player Turn";
        else tempTurnText.text = "AI Turn";
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour // classen blir en singleton
{
    public static TurnManager Instance;
    public bool isPlayerTurn = true;
    public bool TurnEnd = false;
    public GameObject KeepCardScreen;

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

            CardManager.Instance.ClearActiveCard();
            CardManager.Instance.EndTurnDiscardHand();

            MovementManager.Instance.takingMoveAction = false;
            UnitSelector.Instance.playerCanSelectNewUnit = false;

            UpdateUI();

            CameraController.Instance.playerCanMove = false;
            CameraController.Instance.playerHasMoved = false;

            GameoverManager.Instance.CheckGameOver();
            AIManager.Instance.StartAITurn();
        }      
    }

    public void GoToPlayerTurn()
    {
        if (isPlayerTurn)
        {
            Debug.LogError("Something tried to pass to the player, during the players turn");
            return;
        }


        CardManager.Instance.DealHand();

        isPlayerTurn = true;
        MovementManager.Instance.takingMoveAction = true; // Change later
        UnitSelector.Instance.playerCanSelectNewUnit = true;
        CameraController.Instance.playerCanMove = true;
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


    public void toggleEnd()
    {
        if (TurnEnd)
        {
            KeepCardScreen.SetActive(false);
            EndTurn();
            CardManager.Instance.RetrieveKeptCards();
            TurnEnd = !TurnEnd;
        }

        else
        {
            if (HandPanel.Instance.transform.childCount > 0)
            {
                TurnEnd = !TurnEnd;
                KeepCardScreen.SetActive(true);
            }
            else
                EndTurn();
            
        }
    }   
}

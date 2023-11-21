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
            MovementManager.Instance.takingMoveAction = false;

            UpdateUI();
            StartCoroutine(AITurn());//Ändra sen

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
        foreach (Unit u in UnitStorage.Instance.playerUnits)
        {
            u.movePointsCur = u.movePointsMax;
        }
        UpdateUI();
    }

    public System.Collections.IEnumerator AITurn()
    {
        yield return new WaitForSeconds(2.0f); //justera sen 

        //kalla på scriptet för AI movement 

        // Switch back to player's turn
        GoToPlayerTurn();
    }

    public void UpdateUI()
    {
        if (tempTurnText == null) return;

        if (isPlayerTurn) tempTurnText.text = "Player Turn";
        else tempTurnText.text = "AI Turn";
    }
}

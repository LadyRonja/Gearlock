using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class TurnManager : MonoBehaviour // classen blir en singleton
{
    public static TurnManager Instance;
    public bool isPlayerTurn = true;
    public bool TurnEnd = false;
    public GameObject KeepCardScreen;
    public TMP_Text endTurnText;

    public Ease currentEase;
    Vector3 startPos;

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
        endTurnText.text = "End Turn";
        UpdateUI();
    }


    public void UpdateUI()
    {
        

        if (tempTurnText == null) return;

        if (isPlayerTurn)
        {
            tempTurnText.text = "Player Turn";
            TextAnimationTurn();
        }
        else 
        {
            tempTurnText.text = "AI Turn";
            TextAnimationTurn();
        }
        
            
            
    }


    public void toggleEnd()
    {
        CardManager.Instance.ClearActiveCard();

        if (TurnEnd)
        {
            KeepCardScreen.SetActive(false);
            EndTurn();
            CardManager.Instance.RetrieveKeptCards();
            TurnEnd = !TurnEnd;
            endTurnText.text = "--";
        }
        else
        {
            if (HandPanel.Instance.transform.childCount > 0)
            {
                TurnEnd = !TurnEnd;
                KeepCardScreen.SetActive(true);
                endTurnText.text = "Confirm";
            }
            else
                EndTurn();
            
        }
    }   

    public void TextAnimationTurn()
    {
        
        tempTurnText.rectTransform.anchoredPosition = new Vector2(1430, 0);
        tempTurnText.transform.DOMoveX(600, 0.5f).SetEase(currentEase);
    }

    public void OnDisable()
    {
        transform.DOKill();
    }
}

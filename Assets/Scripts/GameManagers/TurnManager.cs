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
    public bool keepPhase = false;
    public GameObject KeepCardScreen;
    public TMP_Text endTurnText;
    public GameObject discard;

    public AudioClip playerTurnStartSound;
    public AudioClip enemyTurnStartSound;

    // Tutorial
    public bool hasEndedTurnOnce = false;
    public bool canEndTurn = true;

    //DOTween Player and AI turn text
    public GameObject underlineRightRed; //turn text line enemy
    public GameObject underlineLeftRed;//turn text line enemy
    public GameObject underlineRightBlue; //turn text line player
    public GameObject underlineLeftBlue;//turn text line player
    public Ease currentEase;

    // public GameObject canvas;//ref to canvas for position 
    public Canvas canvasPos;

    private Unit enableLights;

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

    public void Start()
    {
         enableLights = GetComponent<Unit>();   
    }


    public void EndTurn() //when clicked on End Turn Button
    {
        if (!canEndTurn)
            return;

        if (isPlayerTurn)
        {
            isPlayerTurn = false;
            endTurnText.text = "--";
            if(!hasEndedTurnOnce)
                hasEndedTurnOnce= true;

            CardManager.Instance.ClearActiveCard();
            CardManager.Instance.EndTurnDiscardHand();

            MovementManager.Instance.takingMoveAction = false;
            UnitSelector.Instance.playerCanSelectNewUnit = false;
            

            UpdateUI();
            tempTurnText.DOFade(100, 2);
            underlineRightRed.GetComponent<UnityEngine.UI.Image>().DOFade(100, 2);
            underlineLeftRed.GetComponent<UnityEngine.UI.Image>().DOFade(100, 2);
            

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

        GameStats.Instance.IncreaseTurnsTaken();
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
        tempTurnText.DOFade(100, 2);
        underlineRightBlue.GetComponent<UnityEngine.UI.Image>().DOFade(100, 2);
        underlineLeftBlue.GetComponent<UnityEngine.UI.Image>().DOFade(100, 2);

        // TODO: Make this!
        //UnitSelector.Instance.UpdateSelectedUnit(UnitStorage.Instance.playerUnits[0], true);
    }


    public void UpdateUI()
    {
        if (tempTurnText == null) return;

        if (isPlayerTurn)
        {
            tempTurnText.text = "Player Turn";
            ChangeColorOnTurn();
            TexAnimationTurnPlayer();        
        }
        else 
        {
            tempTurnText.text = "AI Turn";
            ChangeColorOnTurn();
            TextAnimationTurnAI();        
        }

    }


    public void toggleEnd()
    {
        if (!canEndTurn)
            return;

        CardManager.Instance.ClearActiveCard();
        DEBUGCardStateUI.Instance.DEBUGUpdateUI(CardState.Inactive, "--");

        if (TurnEnd)
        {
            keepPhase = false;
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
                if (TutorialBasic.Instance.IsInTutorial)
                    TutorialBasic.Instance.StartBonusTutorial();

                else if (TutorialAdvanced.Instance.IsInTutorial)
                    TutorialAdvanced.Instance.StartBonusTutorial();

                keepPhase = true;
                TurnEnd = !TurnEnd;
                KeepCardScreen.SetActive(true);
                discard.SetActive(false);
                endTurnText.text = "Confirm";
            }
            else
            {
                keepPhase = false;
                EndTurn();
            }
            
        }
    }   

    public void TextAnimationTurnAI()
    {
        
        if (canvasPos != null)
        {

            AudioHandler.PlaySoundEffect(enemyTurnStartSound);

            RectTransform canvasRectTransform = canvasPos.GetComponent<RectTransform>();
            // Calculate the middle position of the canvas
            Vector3 middlePosition = canvasRectTransform.position;


            tempTurnText.rectTransform.anchoredPosition = new Vector2(1430, 15);
            underlineRightRed.transform.localPosition = new Vector3(2000, -80, 0);
            underlineLeftRed.transform.localPosition = new Vector3(-2000, 90, 0);

            DG.Tweening.Sequence textSequence = DOTween.Sequence();
            textSequence.Append(tempTurnText.transform.DOMoveX(middlePosition.x, 0.5f).SetEase(currentEase));
            textSequence.Join(underlineRightRed.transform.DOMoveX(middlePosition.x, 0.2f).SetEase(currentEase));
            textSequence.Join(underlineLeftRed.transform.DOMoveX(middlePosition.x, 0.2f).SetEase(currentEase));
            textSequence.OnComplete(FadeTextTurn);
        }

    }

    public void TexAnimationTurnPlayer()
    {
        
        if (canvasPos != null)
        {

            AudioHandler.PlaySoundEffect(playerTurnStartSound);

            RectTransform canvasRectTransform = canvasPos.GetComponent<RectTransform>();
            // Calculate the middle position of the canvas
            Vector3 middlePosition = canvasRectTransform.position;


            tempTurnText.rectTransform.anchoredPosition = new Vector2(1430, 15);
            underlineRightBlue.transform.localPosition = new Vector3(2000, -80, 0);
            underlineLeftBlue.transform.localPosition = new Vector3(-2000, 90, 0);

            DG.Tweening.Sequence textSequence = DOTween.Sequence();
            textSequence.Append(tempTurnText.transform.DOMoveX(middlePosition.x, 0.5f).SetEase(currentEase));
            textSequence.Join(underlineRightBlue.transform.DOMoveX(middlePosition.x, 0.2f).SetEase(currentEase));
            textSequence.Join(underlineLeftBlue.transform.DOMoveX(middlePosition.x, 0.2f).SetEase(currentEase));
            textSequence.OnComplete(FadeTextTurn);
        }
    }



    public void FadeTextTurn()
    {
        tempTurnText.DOFade(0, 2).SetDelay(0.7f);
        underlineRightRed.GetComponent<UnityEngine.UI.Image>().DOFade(0, 2).SetDelay(0.7f);
        underlineLeftRed.GetComponent<UnityEngine.UI.Image>().DOFade(0, 2).SetDelay(0.7f);

        underlineRightBlue.GetComponent<UnityEngine.UI.Image>().DOFade(0, 2).SetDelay(0.7f);
        underlineLeftBlue.GetComponent<UnityEngine.UI.Image>().DOFade(0, 2).SetDelay(0.7f);

        // Move the text and lines back to their original positions after a delay
       /* DG.Tweening.Sequence returnSequence = DOTween.Sequence();
        returnSequence.Append(tempTurnText.rectTransform.DOAnchorPos(new Vector2(1430, 15), 0.5f).SetEase(currentEase).SetDelay(3f));
        returnSequence.Join(underlineRightBlue.transform.DOLocalMove(new Vector3(2000, -80, 0), 0.5f).SetEase(currentEase).SetDelay(3f));
        returnSequence.Join(underlineLeftBlue.transform.DOLocalMove(new Vector3(-2000, 90, 0), 0.5f).SetEase(currentEase).SetDelay(3f));*/

    }

    public void ChangeColorOnTurn()
    {
   
        if (isPlayerTurn)
        {
            tempTurnText.color = new Color(38f / 255f, 202f / 255f, 227f / 255f, 1f);

        }
        else
        {
            tempTurnText.color = new Color(180f / 255f, 13f / 255f, 8f / 255f, 1f);
        }
    }

    public void OnDisable()
    {
        transform.DOKill();
    }

    //test elin 
    /*private void EnableMovePointLights()
    {
        // Iterate through all units in the MovePointLight list and enable their lights
        foreach (Unit unit in UnitStorage.Instance.playerUnits)
        {
          
                unit.EnableMovePointLights();
            
        }
    }*/

    

}

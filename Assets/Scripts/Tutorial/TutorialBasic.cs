using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialBasic : MonoBehaviour
{
    [Header("Singleton")]
    private static TutorialBasic instance;
    public static TutorialBasic Instance { get => GetInstance(); private set => instance = value; }

    //[Header("Determine if in tutorial")]
    public bool IsInTutorial { get => isInTutorial; private set => isInTutorial = value; }
    public int BasicTutorialIndex { get => basicTutorialIndex; private set => basicTutorialIndex = value; }
    public int[] BasicIndexesToPreventRaycastingOn { get => basicIndexesToPreventRaycastingOn; private set => basicIndexesToPreventRaycastingOn = value; }

    private bool isInTutorial = true;

    [Header("Tutorial Pages")]
    [SerializeField] List<GameObject> basicTutorialPages = new();
    [SerializeField] List<GameObject> bonusTutorialPages = new();
    int basicTutorialIndex = 0;
    int[] basicIndexesToPreventRaycastingOn = { 0, 1, 2, 3, 4, 5, /*6, 7, 8, 9, */
                                                10, 11, 12, 13, 14, 15, 16, 17, 18, /*19,
                                                20,*/ 21, 22, /*23,*/ 24, 25, 26/*, 27*/ };
    int bonusTutorialIndex = 0;
    bool bonusTutorialDone = false;
    bool bonusTutorialActive= false;


    private void Awake()
    {
        if(instance == null || instance == this)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    private void Start()
    {
        if (isInTutorial)
        {
            GoToTutorialPage(1);
            CameraController.Instance.playerCanMove = false;
            TurnManager.Instance.canEndTurn = false;
        }
    }

    private void Update()
    {
        if (!isInTutorial)
            return;

        CheckForTutorialTriggers();
    }

    public void GoToTutorialPage(int page)
    {
        if(basicTutorialIndex != page - 1)
        {
            Debug.Log("Got called to go to: " + page + " while on: " + basicTutorialIndex);
            return;
        }

        basicTutorialIndex = page;
        foreach(GameObject go in basicTutorialPages)
        {
            go.SetActive(false);
        }

        foreach (GameObject go in bonusTutorialPages)
        {
            go.SetActive(false);
        }

        if (basicTutorialIndex <= basicTutorialPages.Count)
            basicTutorialPages[basicTutorialIndex - 1].SetActive(true);
    }

    private void CheckForTutorialTriggers()
    {
        if (bonusTutorialActive)
            return;

        // Play a card
        if(basicTutorialIndex == 5)
        {
            if (ActiveCard.Instance.cardBeingPlayed != null)
                GoToTutorialPage(6);
        }
        else if (basicTutorialIndex == 6)
        {
            if (UnitSelector.Instance.selectedUnit != null)
            {
                GoToTutorialPage(7);
                GoToTutorialPage(8);
            }
        }

        // Select a robot
        else if (basicTutorialIndex == 7)
        {
            if (UnitSelector.Instance.selectedUnit != null)
            {
                GoToTutorialPage(8);
            }
        }

        // Mine a rock
        else if(basicTutorialIndex == 8)
        {
            if (!GridManager.Instance.tiles[1, 0].containsDirt)
            {
                GoToTutorialPage(9);
            }
        }

        // Move
        else if(basicTutorialIndex == 9)
        {
            if (!GridManager.Instance.tiles[0, 0].occupied)
            {
                GoToTutorialPage(10);
            }
        }

        // Open Discard pile
        else if(basicTutorialIndex == 10)
        {
            if (DiscardShow.Instance.hasBeenClosedOnce)
            {
                GoToTutorialPage(11);
                GoToTutorialPage(12);
            }
        }

        // Discard pile has been closed, talk about UI
        else if(basicTutorialIndex == 11)
        {
            if (DiscardShow.Instance.hasBeenClosedOnce)
            {
                GoToTutorialPage(12);
            }
        }

        else if (basicTutorialIndex >= 14 && basicTutorialIndex <= 17)
        {
            if(UnitSelector.Instance.selectedUnit == null)
                UnitSelector.Instance.UpdateSelectedUnit(UnitStorage.Instance.playerUnits[0], false, true);
        }

        // Once all movepoints are done, explain end of turn
        else if(basicTutorialIndex == 19)
        {
            if (UnitStorage.Instance.playerUnits[0].movePointsCur == 0)
            {
                TurnManager.Instance.canEndTurn = true;
                GoToTutorialPage(20);
            }
        }

        // End Turn
        else if(basicTutorialIndex == 20)
        {
            if (!TurnManager.Instance.isPlayerTurn)
            {
                CloseSpecificPage(20);
            }
            else
            {
                if (TurnManager.Instance.isPlayerTurn && TurnManager.Instance.hasEndedTurnOnce)
                {
                    GoToTutorialPage(21);
                }
            }
                
        }

        // Move Camera
        else if (basicTutorialIndex == 22)
        {
            if (CameraController.Instance.playerHasMoved)
            {
                GoToTutorialPage(23);
            }
        }

        // Select Enemy
        else if (basicTutorialIndex == 23)
        {
            if (UnitSelector.Instance.selectedUnit != null)
            {
                if (!UnitSelector.Instance.selectedUnit.playerBot)
                { 
                    GoToTutorialPage(24);
                }
            }
        }

        // Un select Enemy
        else if (basicTutorialIndex == 24)
        {
            if (UnitSelector.Instance.selectedUnit == null)
            {
                GoToTutorialPage(25);
            }
        }
    } 

    public void CloseSpecificPage(int page)
    {
        int[] allowedPagesToClose = { 11, 19, 20, 27 };

        if (!allowedPagesToClose.Contains(page))
        {
            Debug.LogError("CloseSpecificPage called incorrectly");
            return;
        }

        basicTutorialPages[basicTutorialIndex - 1].SetActive(false);
    }

    public void GoToNextBonusTutorial()
    {
        foreach (GameObject go in basicTutorialPages)
        {
            go.SetActive(false);
        }

        foreach (GameObject go in bonusTutorialPages)
        {
            go.SetActive(false);
        }

        if(bonusTutorialIndex < bonusTutorialPages.Count)
        {
            bonusTutorialPages[bonusTutorialIndex].SetActive(true);
            bonusTutorialIndex++;
        }
        else
        {
            bonusTutorialDone = true;
            bonusTutorialActive = false;
            foreach (GameObject go in bonusTutorialPages)
            {
                go.SetActive(false);
            }

            if (basicTutorialIndex <= basicTutorialPages.Count)
                basicTutorialPages[basicTutorialIndex - 1].SetActive(true);
        }

    }

    public void StartBonusTutorial()
    {
        if (!bonusTutorialDone)
        {
            if (!bonusTutorialActive)
            {
                bonusTutorialActive = true;
                GoToNextBonusTutorial();
            }
        }
    }

    public void DealTutorialHand()
    {
        CardManager.Instance.SetUpStartHand();
    }

    public static TutorialBasic GetInstance()
    {
        if (instance != null)
            return instance;

        GameObject newInstance = new GameObject("Basic Tutorial Instance");
        TutorialBasic tutorialScript = newInstance.AddComponent<TutorialBasic>();
        tutorialScript.isInTutorial = false;
        return tutorialScript;

    }
}

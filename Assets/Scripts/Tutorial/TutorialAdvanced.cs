using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TutorialAdvanced : MonoBehaviour
{
    [Header("Singleton")]
    private static TutorialAdvanced instance;
    public static TutorialAdvanced Instance { get => GetInstance(); private set => instance = value; }

    //[Header("Determine if in tutorial")]
    public bool IsInTutorial { get => isInTutorial; private set => isInTutorial = value; }
    public int AdvancedTutorialIndex { get => advancedTutorialIndex; private set => advancedTutorialIndex = value; }
    public int[] AdvancedIndexesToPreventRaycastingOn { get => advancedIndexesToPreventRaycastingOn; private set => advancedIndexesToPreventRaycastingOn = value; }

    private bool isInTutorial = true;

    [Header("Tutorial Pages")]
    [SerializeField] List<GameObject> advancedTutorialPages = new();
    [SerializeField] List<GameObject> bonusTutorialPages = new();
    int advancedTutorialIndex = 0;
    int[] advancedIndexesToPreventRaycastingOn = { 0, 1, 2, 3, 4, /*5,*/ 6, 7, 8, 9,
                                                10, 11, /*12,*/ 13, 14, 15, 16, 17/*, 18*/ };
    int bonusTutorialIndex = 0;
    public static bool bonusTutorialDone = false;
    public bool bonusTutorialActive = false; // should only be public getter
    bool savedCanMoveCam = false;

    bool playerHasEndedTurnOnce = false;
    bool attackCardsGiven = false;


    private void Awake()
    {
        if (instance == null || instance == this)
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
        if (advancedTutorialIndex != page - 1)
        {
            Debug.Log("Got called to go to: " + page + " while on: " + advancedTutorialIndex);
            return;
        }

        advancedTutorialIndex = page;
        foreach (GameObject go in advancedTutorialPages)
        {
            go.SetActive(false);
        }

        foreach (GameObject go in bonusTutorialPages)
        {
            go.SetActive(false);
        }

        if (advancedTutorialIndex <= advancedTutorialPages.Count)
            advancedTutorialPages[advancedTutorialIndex - 1].SetActive(true);
    }

    private void CheckForTutorialTriggers()
    {
        if (bonusTutorialActive)
            return;

        // Pick up another the Fight Bot
        if (advancedTutorialIndex == 5)
        {
            if (GridManager.Instance.tiles[1, 1].myPickUp == null)
            {
                GoToTutorialPage(6);
                TurnManager.Instance.canEndTurn = true;
            }
        }

        // End the players turn to draw the fight bot
        if (advancedTutorialIndex == 7)
        {
            if (!playerHasEndedTurnOnce)
            {
                if (!TurnManager.Instance.isPlayerTurn)
                {
                    CloseSpecificPage(7);
                    playerHasEndedTurnOnce = true;
                }
            }
            else
            {
                if (TurnManager.Instance.isPlayerTurn)
                {
                    GoToTutorialPage(8);
                    CameraController.Instance.playerCanMove = false;
                }
            }
        }

        // Highlight ghost
        if (advancedTutorialIndex == 9)
        {
            UnitSelector.Instance.UpdateSelectedUnit(UnitStorage.Instance.enemyUnits[0], false, true);
        }

        // Highlight big bot
        if (advancedTutorialIndex == 10)
        {
            UnitSelector.Instance.UpdateSelectedUnit(UnitStorage.Instance.enemyUnits[1], false, true);
        }

        // Highlight the dig bot
        if (advancedTutorialIndex == 11)
        {
            UnitSelector.Instance.UpdateSelectedUnit(UnitStorage.Instance.playerUnits[0], false, true);
        }

        // Assemble the Fight Bot
        if (advancedTutorialIndex == 12)
        {
            if (ActiveCard.Instance.cardBeingPlayed != null)
            {
                GoToTutorialPage(13);
            }
        }

        // Assemble the Fight Bot
        if (advancedTutorialIndex == 13)
        {
            if (UnitStorage.Instance.playerUnits.Count == 2)
            {
                GoToTutorialPage(14);
            }
        }

        // Give attack cards
        if (advancedTutorialIndex == 17)
        {
            if (!attackCardsGiven)
            {
                DealTutorialHand();
                CameraController.Instance.playerCanMove = true;
                attackCardsGiven = true;
            }
        }
    }

    public void CloseSpecificPage(int page)
    {
        int[] allowedPagesToClose = { 5, 7, 18 };

        if (!allowedPagesToClose.Contains(page))
        {
            Debug.LogError("CloseSpecificPage called incorrectly");
            return;
        }

        advancedTutorialPages[advancedTutorialIndex - 1].SetActive(false);
    }

    public void GoToNextBonusTutorial()
    {
        foreach (GameObject go in advancedTutorialPages)
        {
            go.SetActive(false);
        }

        foreach (GameObject go in bonusTutorialPages)
        {
            go.SetActive(false);
        }

        if (bonusTutorialIndex < bonusTutorialPages.Count)
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

            if (advancedTutorialIndex <= advancedTutorialPages.Count)
            {
                advancedTutorialPages[advancedTutorialIndex - 1].SetActive(true);
                CameraController.Instance.playerCanMove = savedCanMoveCam;
            }
        }

    }

    public void StartBonusTutorial()
    {
        if (!bonusTutorialDone)
        {
            if (!bonusTutorialActive)
            {
                savedCanMoveCam = CameraController.Instance.playerCanMove;
                bonusTutorialActive = true;
                TutorialBasic.bonusTutorialDone = true;
                GoToNextBonusTutorial();
            }
        }
    }

    public void DealTutorialHand()
    {
        CardManager.Instance.SetUpStartHand();
    }

    public static TutorialAdvanced GetInstance()
    {
        if (instance != null)
            return instance;

        GameObject newInstance = new GameObject("Basic Tutorial Instance");
        TutorialAdvanced tutorialScript = newInstance.AddComponent<TutorialAdvanced>();
        tutorialScript.isInTutorial = false;
        return tutorialScript;

    }
}

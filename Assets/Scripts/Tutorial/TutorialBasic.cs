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
    private bool isInTutorial = true;

    [Header("Tutorial Pages")]
    [SerializeField] List<GameObject> basicTutorialPages = new();
    [SerializeField] List<GameObject> bonusTutorialPages = new();
    int basicTutorialIndex = 0;
    int bonusTutorialIndex = 0;

    private void Awake()
    {
        if(instance == null || instance == this)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    private void Start()
    {
        if(isInTutorial)
            GoToTutorialPage(1);
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

        if(basicTutorialIndex < basicTutorialPages.Count - 1)
            basicTutorialPages[basicTutorialIndex - 1].SetActive(true);
    }

    private void CheckForTutorialTriggers()
    {
        // Play a card
        if(basicTutorialIndex == 5)
        {
            if (ActiveCard.Instance.cardBeingPlayed != null)
                GoToTutorialPage(6);
        }

        // Select a robot
        if (basicTutorialIndex == 7)
        {
            if (UnitSelector.Instance.selectedUnit != null)
            {
                GoToTutorialPage(8);
            }
        }

        // Mine a rock
        if(basicTutorialIndex == 8)
        {
            if (!GridManager.Instance.tiles[1, 0].containsDirt)
            {
                GoToTutorialPage(9);
            }
        }

        // Move
        if (basicTutorialIndex == 9)
        {
            if (!GridManager.Instance.tiles[0, 0].occupied)
            {
                GoToTutorialPage(10);
            }
        }

        // Open Discard pile
        if (basicTutorialIndex == 10)
        {
            if (DiscardShow.Instance.hasBeenClosedOnce)
            {
                GoToTutorialPage(11);
                GoToTutorialPage(12);
            }
        }

        // Discard pile has been closed, talk about UI
        if (basicTutorialIndex == 11)
        {
            if (DiscardShow.Instance.hasBeenClosedOnce)
            {
                GoToTutorialPage(12);
            }
        }

        // Once all movepoints are done, explain end of turn
        if(basicTutorialIndex == 19)
        {
            if (UnitStorage.Instance.playerUnits[0].movePointsCur == 0)
            {
                GoToTutorialPage(20);
            }
        }
    } 

    public void CloseSpecificPage(int page)
    {
        int[] allowedPagesToClose = { 11, 19 };

        if (!allowedPagesToClose.Contains(page))
        {
            Debug.LogError("CloseSpecificPage called incorrectly");
            return;
        }

        basicTutorialPages[basicTutorialIndex - 1].SetActive(false);
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

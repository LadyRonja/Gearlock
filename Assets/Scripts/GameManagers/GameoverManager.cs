using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameoverManager : MonoBehaviour
{
    public static GameoverManager Instance;
    [SerializeField] public GameObject gameOverScreen;
    [SerializeField] TMP_Text gameOverText;
    [SerializeField] string firstTutorialSceneName = "Tutorial First";
    public bool isFirstTutorial = false;
    [SerializeField] string secondTutorialSceneName = "Tutorial Second";
    public bool isSecondTutorial = false;
    [SerializeField] string levelOneName = "Last Stand _small";

    GameObject gameOverFade;

    private void Awake()
    {
        #region Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
        #endregion

        gameOverScreen.SetActive(false);
    }


    public void CheckGameOver()
    {
        if(UnitStorage.Instance.enemyUnits.Count <= 0)
        {
            GameIsOver(true); 
            return;
        }

        if (UnitStorage.Instance.playerUnits.Count <= 0)
        {   
            GameIsOver(false);
            return;
        }
    }


    private void GameIsOver(bool playerWon)
    {
        if(isFirstTutorial)
        {
            if (playerWon)
                Scenehandler.Instance.GoToScene(secondTutorialSceneName);
            else
                Scenehandler.Instance.GoToScene(firstTutorialSceneName);
        }
        else if(isSecondTutorial)
        {
            if (playerWon)
                Scenehandler.Instance.GoToScene(levelOneName);
            else
                Scenehandler.Instance.GoToScene(secondTutorialSceneName);
        }
        else
        {
            if (playerWon)
                gameOverText.text = "You won!";
            else
                gameOverText.text = "You lost!";

            PlayGameOverAnimation();
        }
    }

    private void PlayGameOverAnimation()
    {
        gameOverScreen.SetActive(true);
    }

    private IEnumerator GameOverAnimator()
    {

        yield return null;
    }

    private void GenerateGameOverScreen()
    {

    }
}

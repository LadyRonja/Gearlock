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
    [SerializeField] string levelOneName = "DevRonja";

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
                SceneManager.LoadScene(secondTutorialSceneName);
            else
                SceneManager.LoadScene(firstTutorialSceneName);
        }
        else if(isSecondTutorial)
        {
            if (playerWon)
                SceneManager.LoadScene(levelOneName);
            else
                SceneManager.LoadScene(secondTutorialSceneName);
        }
        else
        {
            if (playerWon)
                gameOverText.text = "You won!";
            else
                gameOverText.text = "You lost!";

            gameOverScreen.SetActive(true);
        }

    }
}

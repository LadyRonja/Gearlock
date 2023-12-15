using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameoverManager : MonoBehaviour
{
    public static GameoverManager Instance;
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] TMP_Text gameOverText;
    public bool isTutorial = false;

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
        if(isTutorial)
        {
            if (playerWon)
                SceneManager.LoadScene("GameTest2");
            else
                SceneManager.LoadScene("Tutorial 1");
        }

        if(playerWon)
            gameOverText.text = "You won!";
        else
            gameOverText.text = "You lost!";


        gameOverScreen.SetActive(true);
    }
}

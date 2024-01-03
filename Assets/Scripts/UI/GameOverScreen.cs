using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    public TMP_Text winStatusText;
    public TMP_Text statsText;
    [Space]
    public Button replayButton;
    public Button menuButton;
    public Button quitButton;
    [Space]
    public Image buttonPanel;

    public void PlayAgain() 
    {
        Scenehandler.Instance.GoToScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMain()
    {
        Scenehandler.Instance.GoToScene("Main Menu");
    }

    public void Quit()
    { 
        Scenehandler.Instance.QuitGame();
    }

}

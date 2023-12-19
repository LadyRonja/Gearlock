using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scenehandler : MonoBehaviour
{
    public GameObject menuButtons;
    public GameObject tutorialCheckPanel;
    public GameObject optionsBar;

    public void TutorialCheck()
    {
        menuButtons.SetActive(false);
        tutorialCheckPanel.SetActive(true);
    }

    public void OptionsToggle()
    {
        menuButtons.SetActive(!menuButtons.activeSelf);
        optionsBar.SetActive(!optionsBar.activeSelf);
    }
    public void GameStart()
    {
        SceneManager.LoadScene("DevLamberth");
    }

    public void TutorialStart()
    {
        SceneManager.LoadScene("Tutorial 1");
    }

    public void menu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    //Click the cell in the menu to trigger the easter egg

}

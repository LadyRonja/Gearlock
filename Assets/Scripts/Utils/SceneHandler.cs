using System.Collections;
using System.Collections.Generic;
using config;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scenehandler : MonoBehaviour
{
    public static Scenehandler Instance;

    public GameObject menuButtons;
    public GameObject tutorialCheckPanel;
    public GameObject optionsBar;

    public bool toggleZoomOnHover = false;
    public bool toggleClickToDrag = false;
    public bool toggleCardReposition = false;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float effectVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);

        AudioHandler musicInitializer = AudioHandler.Instance;
    }
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

    public void ToggleZoomOnHover()
    {
        toggleZoomOnHover = !toggleZoomOnHover;
    }

    public void ToggleClickToDrag()
    {
        toggleClickToDrag= !toggleClickToDrag;
    }

    public void ToggleCardReposition()
    {
        toggleCardReposition= !toggleCardReposition;
    }
}

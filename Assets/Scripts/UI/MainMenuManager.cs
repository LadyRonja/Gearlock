using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject menuButtons;
    public GameObject tutorialCheckPanel;
    public GameObject optionsBar;
    public GameObject creditsPanel;

    public Slider musicSlider;
    public Slider effectSlider;

    // Main menu buttons
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

    public void ToggleCredits()
    {
        menuButtons.SetActive(!menuButtons.activeSelf);
        creditsPanel.SetActive(!creditsPanel.activeSelf);
    }


    public void QuitGame()
    {
        Scenehandler.Instance.QuitGame();
    }

    // Go To Scenes
    public void TutorialStart()
    {
        Scenehandler.Instance.GoToScene("Basic Tutorial");
    }

    public void GameStart()
    {
        Scenehandler.Instance.GoToScene("Last Stand _Small");
    }


    // Options
    public void ToggleZoomOnHover()
    {
        Scenehandler.Instance.toggleZoomOnHover = !Scenehandler.Instance.toggleZoomOnHover;
    }

    public void ToggleClickToDrag()
    {
        Scenehandler.Instance.toggleClickToDrag = !Scenehandler.Instance.toggleClickToDrag;
    }

    public void ToggleCardReposition()
    {
        Scenehandler.Instance.toggleCardReposition = !Scenehandler.Instance.toggleCardReposition;
    }
    public void MusicVolume()
    {
        Scenehandler.Instance.MusicVolume(musicSlider);
    }

    public void EffectVolume()
    {
        Scenehandler.Instance.EffectVolume(effectSlider);
    }

}

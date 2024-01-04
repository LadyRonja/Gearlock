using System.Collections;
using System.Collections.Generic;
using config;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class PauseHandler : MonoBehaviour
{
    private static PauseHandler instance;

    public GameObject pauseButtons;
    public GameObject optionsBar;
    public GameObject hand;

    public bool toggleZoomOnHover = false;
    public bool toggleClickToDrag = false;
    public bool toggleCardReposition = false;
    private bool pauseMenu = false;
    private bool optionsMenu = false;

    public Toggle zoom;
    public Toggle click;
    public Toggle reposition;

    public Slider musicSlider;
    public Slider effectSlider;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float effectVolume = 1f;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

    }

    private void Start()
    {
        musicSlider.value = Scenehandler.Instance.musicVolume * 100;
        effectSlider.value = Scenehandler.Instance.effectVolume * 100;

        zoom.isOn = DataHandler.Instance.toggleZoom;
        click.isOn = DataHandler.Instance.toggleClick;
        reposition.isOn = DataHandler.Instance.toggleInverseCamera;
    }

    public void OptionsToggle()
    {
        pauseButtons.SetActive(!pauseButtons.activeSelf);
        optionsBar.SetActive(!optionsBar.activeSelf);
        pauseMenu = !pauseMenu;
        optionsMenu = !optionsMenu;
    }

    public void PauseToggle()
    {
        pauseMenu = !pauseMenu;


        if (pauseMenu)
        {
            hand.SetActive(false);
            pauseButtons.SetActive(true);
            if (optionsMenu)
            {
                pauseButtons.SetActive(false);
                hand.SetActive(true);
                optionsMenu = false;
                pauseMenu = false;
            }
        }
        else
        {
            hand.SetActive(true);
            pauseButtons.SetActive(false);
            if (optionsMenu)
            {
                pauseButtons.SetActive(false);
                optionsMenu = false;
            }
        }

        optionsBar.SetActive(false);
    }


    public void GameStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void menu()
    {
        SceneManager.LoadScene("Main Menu");
    }



    //    public void QuitGame()
    //    {
    //#if UNITY_EDITOR
    //        UnityEditor.EditorApplication.isPlaying = false;
    //#endif
    //        Application.Quit();
    //    }

    public void ToggleZoomOnHover()
    {
        DataHandler.Instance.toggleZoom = !DataHandler.Instance.toggleZoom;
    }

    public void ToggleClickToDrag()
    {
        DataHandler.Instance.toggleClick = !DataHandler.Instance.toggleClick;
    }

    public void ToggleCardReposition()
    {
        DataHandler.Instance.toggleInverseCamera = !DataHandler.Instance.toggleInverseCamera;
    }


    public void MusicVolume()
    {
        musicVolume = musicSlider.value / 100;
        AudioHandler.Instance.UpdateMusicVolume(musicVolume);
    }

    public void EffectVolume()
    {
        effectVolume = effectSlider.value / 100;
        AudioHandler.Instance.UpdateEffectVolume(effectVolume);
    }
}

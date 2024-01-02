using System.Collections;
using System.Collections.Generic;
using config;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

        AudioHandler musicInitializer = AudioHandler.Instance;
    }

    private void Start()
    {
        musicSlider = Scenehandler.Instance.musicSlider;
        effectSlider = Scenehandler.Instance.effectSlider;
        toggleZoomOnHover = Scenehandler.Instance.toggleZoomOnHover;
        toggleClickToDrag = Scenehandler.Instance.toggleClickToDrag;
        toggleCardReposition = Scenehandler.Instance.toggleCardReposition;
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
        SceneManager.LoadScene("Last Stand _Small");
    }


    public void menu()
    {
        SceneManager.LoadScene("MainMenu");
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
        musicVolume = musicSlider.value / 100;
        AudioHandler.Instance.UpdateMusicVolume(musicVolume);
    }

    public void EffectVolume()
    {
        effectVolume = effectSlider.value / 100;
        AudioHandler.Instance.UpdateEffectVolume(effectVolume);
    }
}

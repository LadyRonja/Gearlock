using System.Collections;
using System.Collections.Generic;
using config;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Scenehandler : MonoBehaviour
{
    private static Scenehandler instance;

    Transform transitionImage;

    public GameObject menuButtons;
    public GameObject tutorialCheckPanel;
    public GameObject optionsBar;

    public bool toggleZoomOnHover = false;
    public bool toggleClickToDrag = false;
    public bool toggleCardReposition = false;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float effectVolume = 1f;

    bool changingScene = false;

    public static Scenehandler Instance { get => GetInstance(); private set => instance = value; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += delegate { Detransition(); };

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
        SceneManager.LoadScene("GameTest4");
    }

    public void TutorialStart()
    {
        SceneManager.LoadScene("Tutorial First");
    }

    public void menu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void GoToScene(string toScene)
    {
        if(!changingScene)
        {
            changingScene = true;
            StartCoroutine(Transition(toScene));
        }
    }

    private void Detransition()
    {
        GenerateTransitionImage();

        transitionImage.GetComponent<RectTransform>().localScale = new Vector3(30, 30, 30);

        StartCoroutine(ScaleTransition(Vector3.zero));
        changingScene = false;
    }

    private void GenerateTransitionImage()
    {
        if (transitionImage != null)
            return;

        GraphicsRayCastAssistance attemptedMainCanvas = FindObjectOfType<GraphicsRayCastAssistance>();
        Canvas gameCanvas = null;

        if (attemptedMainCanvas != null)
            gameCanvas = attemptedMainCanvas.GetComponent<Canvas>();

        if (gameCanvas == null)
            gameCanvas = FindObjectOfType<Canvas>();

        if (gameCanvas == null)
            gameCanvas = new GameObject("Canvas").AddComponent<Canvas>();

        GameObject newTransitionScreen = new GameObject("Transition Image");
        newTransitionScreen.transform.parent = gameCanvas.transform;
        RectTransform rectTransform = newTransitionScreen.AddComponent<RectTransform>();
        rectTransform.localPosition = Vector3.zero;
        rectTransform.localScale = Vector3.zero;
        Canvas layerCanvas = newTransitionScreen.AddComponent<Canvas>();
        layerCanvas.overrideSorting = true;
        layerCanvas.sortingOrder = 50;
        Image newTransitionImage = newTransitionScreen.AddComponent<Image>();
        newTransitionImage.sprite = Resources.Load<Sprite>("Graphics/transition_Image");
        newTransitionImage.color = Color.black;

        transitionImage = newTransitionScreen.transform;     
        DontDestroyOnLoad(transitionImage);
    }

    private IEnumerator Transition(string toScene)
    {
        GenerateTransitionImage();

        if (transitionImage == null)
            SceneManager.LoadScene(toScene);

        transitionImage.GetComponent<RectTransform>().localScale = Vector3.zero;

        yield return StartCoroutine(ScaleTransition(new Vector3(30, 30, 30)));

        SceneManager.LoadScene(toScene);
        yield return null;
    }

    private IEnumerator ScaleTransition(Vector3 toScale)
    {
        float secondsToTransition = 1f;
        float timePassed = 0;

        Vector3 startScale = transitionImage.localScale;
        Vector3 endScale = toScale;
        RectTransform transitionRect = transitionImage.GetComponent<RectTransform>();
        float rotationSpeed = 45f;

        while (timePassed < secondsToTransition)
        {
            transitionRect.localScale = Vector3.Lerp(startScale, endScale, (timePassed / secondsToTransition));
            transitionRect.eulerAngles += new Vector3(0, 0, rotationSpeed * Time.deltaTime);

            timePassed += Time.deltaTime;
            yield return null;
        }
        transitionRect.localScale = endScale;

        yield return null;
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

    private static Scenehandler GetInstance()
    {
        if(instance != null)
            return instance;

        if (!Application.isPlaying)
            return null;

        GameObject go = new GameObject("SceneHandler");
        return go.AddComponent<Scenehandler>();
    }
}

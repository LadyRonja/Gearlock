using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;

public class GameoverManager : MonoBehaviour
{
    public static GameoverManager Instance;
    [SerializeField] public GameOverScreen gameOverScreen;
    [SerializeField] string firstTutorialSceneName = "Tutorial First";
    public bool isFirstTutorial = false;
    [SerializeField] string secondTutorialSceneName = "Tutorial Second";
    public bool isSecondTutorial = false;
    [SerializeField] string levelOneName = "Last Stand _small";

    [SerializeField] GameObject gameOverFade;

    public bool gameIsOver = false;

    private void Awake()
    {
        #region Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
        #endregion

        gameOverScreen.gameObject.SetActive(false);
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
        if (gameIsOver)
            return;

        gameIsOver = true;

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
            PlayGameOverAnimation(playerWon);
            AIManager.Instance.StopAllCoroutines();
        }
    }

    private void PlayGameOverAnimation(bool playerWon)
    {
        //gameOverScreen.SetActive(true);
        AudioHandler.Instance.TuneOutMusic();
        StartCoroutine(GameOverAnimator(playerWon));
    }

    private IEnumerator GameOverAnimator(bool playerWon)
    {
        GenerateGameOverScreen();

        // Buttons
        Image replayImage = gameOverScreen.replayButton.GetComponent<Image>();
        replayImage.raycastTarget = true;
        replayImage.color = new Color(1, 1, 1, 0);
        TMP_Text replayText = gameOverScreen.replayButton.GetComponentInChildren<TMP_Text>();
        replayText.color = new Color(1, 1, 1, 0);

        Image menuImage = gameOverScreen.menuButton.GetComponent<Image>();
        menuImage.raycastTarget = true;
        menuImage.color = new Color(1, 1, 1, 0);
        TMP_Text menuText = gameOverScreen.menuButton.GetComponentInChildren<TMP_Text>();
        menuText.color = new Color(1, 1, 1, 0);

        Image quitImage = gameOverScreen.quitButton.GetComponent<Image>();
        quitImage.raycastTarget = true;
        quitImage.color = new Color(1, 1, 1, 0);
        TMP_Text quitText = gameOverScreen.quitButton.GetComponentInChildren<TMP_Text>();
        quitText.color = new Color(1, 1, 1, 0);

        // Background
        Image fadeImage = gameOverFade.GetComponent<Image>();
        Color fadeColor = Color.white;
        if (!playerWon)
            fadeColor = Color.black;

        fadeColor.a = 0;
        fadeImage.color = fadeColor;

        // Text
        gameOverScreen.winStatusText.color = new Color(1, 1, 1, 0);
        gameOverScreen.statsText.color = new Color(1, 1, 1, 0);


        // Audio
        AudioSource mySource = new GameObject("Game Over Noise").AddComponent<AudioSource>();
        mySource.volume = Scenehandler.Instance.effectVolume;
        if(playerWon)
            mySource.clip = Resources.Load<AudioClip>("Music/Game Over/victory noise");
        else
            mySource.clip = Resources.Load<AudioClip>("Music/Game Over/defeat noise");

        mySource.Play();

        float timePassed = 0;
        float timeToAnimate = 5f;

        this.StartCoroutine(FadeInAlpha(timeToAnimate, fadeImage, 0));
        while (timePassed < timeToAnimate)
        {
            mySource.volume = Mathf.Lerp(0, Scenehandler.Instance.effectVolume * 0.5f, (timePassed / timeToAnimate));

            timePassed += Time.deltaTime;
            yield return null;
        }
        mySource.volume = Scenehandler.Instance.effectVolume * 0.5f;
        Image gameOverScreenBackground = gameOverScreen.GetComponent<Image>();
        gameOverScreenBackground.color = new Color(1,1,1,0);

        if (playerWon)
            gameOverScreen.winStatusText.text = "Victory!";
        else
            gameOverScreen.winStatusText.text = "Defeat!";

        gameOverScreen.winStatusText.color = new Color(1,1,1,0);
        gameOverScreen.statsText.color = new Color(1, 1, 1, 0);
        gameOverScreen.buttonPanel.color = new Color(1,1,1,0);

        gameOverScreen.gameObject.SetActive(true);
        this.StartCoroutine(FadeInAlpha(timeToAnimate/2f, gameOverScreenBackground, 0));
        this.StartCoroutine(FadeInAlpha(timeToAnimate / 2f, gameOverScreen.winStatusText, timeToAnimate / 4)); 
        this.StartCoroutine(FadeInAlpha(timeToAnimate / 2f, gameOverScreen.statsText, timeToAnimate / 4));

        this.StartCoroutine(FadeInAlpha(timeToAnimate / 2f, gameOverScreen.buttonPanel, timeToAnimate / 2f));
        this.StartCoroutine(FadeInAlpha(timeToAnimate / 2f, replayImage, timeToAnimate / 2f));
        this.StartCoroutine(FadeInAlpha(timeToAnimate / 2f, replayText, timeToAnimate / 2f));
        this.StartCoroutine(FadeInAlpha(timeToAnimate / 2f, menuImage, timeToAnimate / 2f));
        this.StartCoroutine(FadeInAlpha(timeToAnimate / 2f, menuText, timeToAnimate / 2f));
        this.StartCoroutine(FadeInAlpha(timeToAnimate / 2f, quitImage, timeToAnimate / 2f));
        this.StartCoroutine(FadeInAlpha(timeToAnimate / 2f, quitText, timeToAnimate / 2f));



        yield return null;
    }

    private IEnumerator FadeInAlpha(float secToFade, Image imageToFadeIn, float startDelay) 
    {
        yield return new WaitForSeconds(startDelay);
        Color startColor = imageToFadeIn.color;
        Color targetColor = startColor;
        targetColor.a = 1;

        float timePassed = 0;
        while (timePassed < secToFade)
        {
            imageToFadeIn.color = Color.Lerp(startColor, targetColor, (timePassed / secToFade));

            timePassed += Time.deltaTime;
            yield return null;
        }
        imageToFadeIn.color = targetColor;
        yield return null;
    }

    private IEnumerator FadeInAlpha(float secToFade, TMP_Text textToFadeIn, float startDelay)
    {
        yield return new WaitForSeconds(startDelay);
        Color startColor = textToFadeIn.color;
        Color targetColor = startColor;
        targetColor.a = 1;

        float timePassed = 0;
        while (timePassed < secToFade)
        {
            textToFadeIn.color = Color.Lerp(startColor, targetColor, (timePassed / secToFade));

            timePassed += Time.deltaTime;
            yield return null;
        }
        textToFadeIn.color = targetColor;
        yield return null;
    }


    private void GenerateGameOverScreen()
    {
        if (gameOverFade != null)
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
        rectTransform.localScale = new Vector3(50, 50, 50);
        Canvas layerCanvas = newTransitionScreen.AddComponent<Canvas>();
        layerCanvas.overrideSorting = true;
        layerCanvas.sortingOrder = 40;
        Image newTransitionImage = newTransitionScreen.AddComponent<Image>();
        newTransitionImage.raycastTarget = false;
        newTransitionImage.color = new Color(0, 0, 0, 0);

        gameOverFade = newTransitionScreen;
    }
}

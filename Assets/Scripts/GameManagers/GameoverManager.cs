using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameoverManager : MonoBehaviour
{
    public static GameoverManager Instance;
    [SerializeField] public GameOverScreen gameOverScreen;
    [SerializeField] TMP_Text gameOverText;
    [SerializeField] string firstTutorialSceneName = "Tutorial First";
    public bool isFirstTutorial = false;
    [SerializeField] string secondTutorialSceneName = "Tutorial Second";
    public bool isSecondTutorial = false;
    [SerializeField] string levelOneName = "Last Stand _small";

    [SerializeField] GameObject gameOverFade;

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
            if (playerWon)
                gameOverText.text = "You won!";
            else
                gameOverText.text = "You lost!";

            PlayGameOverAnimation(playerWon);
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
        Image fadeImage = gameOverFade.GetComponent<Image>();
        Color fadeColor = Color.white;
        if (!playerWon)
            fadeColor = Color.black;

        fadeColor.a = 0;
        fadeImage.color = fadeColor;
        fadeColor.a = 1;
        Color startColor = fadeImage.color;

        AudioSource mySource = new GameObject("Game Over Noise").AddComponent<AudioSource>();
        mySource.volume = Scenehandler.Instance.effectVolume;
        if(playerWon)
            mySource.clip = Resources.Load<AudioClip>("Music/Game Over/victory noise");
        else
            mySource.clip = Resources.Load<AudioClip>("Music/Game Over/defeat noise");

        mySource.Play();

        float timePassed = 0;
        float timeToAnimate = 5f;

        while (timePassed < timeToAnimate)
        {
            fadeImage.color = Color.Lerp(startColor, fadeColor, (timePassed / timeToAnimate));
            mySource.volume = Mathf.Lerp(0, Scenehandler.Instance.effectVolume * 0.5f, (timePassed / timeToAnimate));

            timePassed += Time.deltaTime;
            yield return null;
        }
        fadeImage.color = fadeColor;
        mySource.volume = Scenehandler.Instance.effectVolume * 0.5f;

        gameOverScreen.gameObject.SetActive(true);

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
        newTransitionImage.color = new Color(0, 0, 0, 0);

        gameOverFade = newTransitionScreen;
    }
}

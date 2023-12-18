using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendlyFirePopUp : MonoBehaviour
{
    private static FriendlyFirePopUp instance;

    public Button CancelButton;
    public Button ConfirmButton;

    public static FriendlyFirePopUp Instance { get => GetInstance(); private set => instance = value; }

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    private void Start()
    {
        ClosePopUp();
    }

    public void OpenPopUp()
    {
        gameObject.SetActive(true);
    }

    public void ClosePopUp()
    {
        gameObject.SetActive(false);
        CancelButton.onClick.RemoveAllListeners();
        ConfirmButton.onClick.RemoveAllListeners();
    }

    private static FriendlyFirePopUp GetInstance()
    {
        if (instance != null)
            return instance;

        Debug.LogError("No FriendlyFirePopUp instance, attempting to create new");
        GameObject newManager = new GameObject("FriendlyFirePopUp");
        instance = newManager.AddComponent<FriendlyFirePopUp>();
        instance.CancelButton = new GameObject("ConfirmButton").AddComponent<Button>();
        instance.CancelButton = new GameObject("CancelButton").AddComponent<Button>();

        return instance;
    }
}

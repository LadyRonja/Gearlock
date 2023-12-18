using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DEBUGCardStateUI : MonoBehaviour
{
    public static DEBUGCardStateUI Instance;
    [SerializeField] GameObject debugTextObject;
    [SerializeField] TMP_Text debugCardStateText;

    private void Awake()
    {
        #region Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
        #endregion

        debugTextObject.SetActive(false);
    }

    /// <summary>
    /// This is bad, but there to help players without the console
    /// </summary>
    /// <param name="toState"></param>
    /// <param name="msg"></param>
    public void DEBUGUpdateUI(CardState toState, string msg)
    {
        if (debugCardStateText == null)
            return;

        switch (toState)
        {
            case CardState.Inactive:
                debugTextObject.SetActive(false);
                break;
            case CardState.VerifyUnitSelection:
                debugTextObject.SetActive(true);
                debugCardStateText.text = msg;
                break;
            case CardState.SelectingUnit:
                debugTextObject.SetActive(true);
                debugCardStateText.text = msg;
                break;
            case CardState.SelectingTile:
                debugTextObject.SetActive(true);
                debugCardStateText.text = msg;
                break;
            case CardState.VerifyTileSelection:
                debugTextObject.SetActive(true);
                debugCardStateText.text = msg;
                break;
            case CardState.Executing:
                debugTextObject.SetActive(true);
                debugCardStateText.text = msg;
                break;
            case CardState.Finished:
                debugTextObject.SetActive(false);
                break;
            default:
                break;
        }
    }
}

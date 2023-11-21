using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour // classen blir en singleton
{
    public static TurnManager Instance;
    public bool isPlayerTurn = true;

    public void Awake()
    {
        #region Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
        #endregion

    }
    public void EndTurn() //when clicked on End Turn Button
    {
        if (isPlayerTurn)
        {
            isPlayerTurn = false;
            StartCoroutine(AITurn());//Ändra sen

            //TODO disable player interaction
        }
        
    }

    public System.Collections.IEnumerator AITurn()
    {
        yield return new WaitForSeconds(2.0f); //justera sen 

        //kalla på scriptet för AI movement 

        // Switch back to player's turn
        isPlayerTurn = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugCommands : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // Player Wins
        if(Input.GetKeyDown(KeyCode.P))
        {
            UnitStorage.Instance.enemyUnits[0].TakeDamage(99);
        }

        // Enemies win
        if(Input.GetKeyDown(KeyCode.L))
        {
            UnitStorage.Instance.playerUnits[0].TakeDamage(99);
        }
    }
}

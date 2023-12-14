using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStorage : MonoBehaviour
{
    public static UnitStorage Instance;

    public List<Unit> playerUnits = new();
    public List<Unit> enemyUnits = new();
    public List<UnitMiniPanel> playerPanels = new();

    private void Awake()
    {
        #region Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
        #endregion
    }

    public void RemoveUnit(Unit unitToRemove)
    {
        if (playerUnits.Contains(unitToRemove))
        {
            playerUnits.Remove(unitToRemove);
        }
        else if (enemyUnits.Contains(unitToRemove))
        {
            enemyUnits.Remove(unitToRemove);
        }
    }
}

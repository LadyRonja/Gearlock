using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance;

    private void Awake()
    {
        #region Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
        #endregion
    }
    

    public void StartAITurn()
    {
        foreach (Unit u in UnitStorage.Instance.enemyUnits)
        {
            u.movePointsCur = u.movePointsMax;
        }
        StartCoroutine(TakeEnemyTurn());
    }

    private IEnumerator TakeEnemyTurn()
    {
        foreach (Unit u in UnitStorage.Instance.enemyUnits)
        {
            UnitSelector.Instance.UpdateSelectedUnit(u, true);
            Unit enemyTarget = u.FindTargetUnit();
            Debug.Log("enemy target found " + enemyTarget + " for " + u.unitName);
            List<Tile> path = u.CalculatePathToTarget(enemyTarget.standingOn);
            yield return StartCoroutine(u.MovePath(path));

            // TODO: Attack Here
        }

        TurnManager.Instance.GoToPlayerTurn();
        yield return null;
    }
}

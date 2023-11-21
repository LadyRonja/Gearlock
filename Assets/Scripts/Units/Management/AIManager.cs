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

    public void StartEnemeyTurn()
    {
        StartCoroutine(TakeEnemyTurn());
    }

    private IEnumerator TakeEnemyTurn()
    {
        foreach (Unit u in UnitStorage.Instance.enemyUnits)
        {
            Unit enemyTarget = u.FindTargetUnit();
            List<Tile> path = u.CalculatePathToTarget(enemyTarget.standingOn);
            yield return StartCoroutine(u.MovePath(path));

            // TODO: Attack Here
        }

        TurnManager.Instance.GoToPlayerTurn();
        yield return null;
    }
}

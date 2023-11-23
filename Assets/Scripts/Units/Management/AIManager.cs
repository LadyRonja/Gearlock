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
            // Find Target
            UnitSelector.Instance.UpdateSelectedUnit(u, true);
            Unit enemyTarget = u.FindTargetUnit();

            // Move Towards Target
            List<Tile> path = u.CalculatePathToTarget(enemyTarget.standingOn);
            yield return StartCoroutine(u.MovePath(path));


            // Attack Target if in range
            if (Pathfinding.GetDistance(u.standingOn, enemyTarget.standingOn) > u.attackRange)
                continue;

            UnitSelector.Instance.UpdateSelectedUnit(enemyTarget, true);
            // TODO:    
            // Play Animation Here

            enemyTarget.TakeDamage(u.power);

        }

        TurnManager.Instance.GoToPlayerTurn();
        yield return null;
    }
}

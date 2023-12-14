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

            yield return new WaitForSeconds(1.5f); 

            // Move Towards Target
            List<Tile> path = u.CalculatePathToTarget(enemyTarget.standingOn);
            yield return StartCoroutine(u.MovePath(path));


            // Attack Target if in range
            if (Pathfinding.GetDistance(u.standingOn, enemyTarget.standingOn) > u.attackRange)
                continue;

            UnitSelector.Instance.UpdateSelectedUnit(enemyTarget, true);
            // TODO:    
            // Play Animation Here
            yield return new WaitForSeconds(0.5f);
            enemyTarget.TakeDamage(u.power);
            yield return new WaitForSeconds(2f);

        }

        TurnManager.Instance.GoToPlayerTurn();
        UnitSelector.Instance.UpdateSelectedUnit(UnitStorage.Instance.playerUnits[0], true);
        yield return null;
    }
}

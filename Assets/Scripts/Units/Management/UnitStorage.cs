using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStorage : MonoBehaviour
{
    public static UnitStorage Instance;

    public List<Unit> playerUnits = new List<Unit>();
    public List<Unit> enemyUnits = new List<Unit>();

    private void Awake()
    {
        #region Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
        #endregion
    }
}

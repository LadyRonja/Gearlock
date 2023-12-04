using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public static UnitSpawner Instance;

    public Transform playerUnitParent;
    public Transform enemyUnitParent;
    [Space]
    [SerializeField] GameObject unitPrefabToSpawn;
    [SerializeField] Tile tileToSpawnOn;
    [Space]
    [SerializeField] Unit unitToRemove;

    private void Awake()
    {
        #region Singleton
        if(Instance == null) 
            Instance = this;
        else
            Destroy(this.gameObject);
        #endregion
    }

    [ContextMenu("Spawn unit on tile")]
    private void SpawnUnitFromEditor()
    {
        // Exit points
        if (unitPrefabToSpawn == null) return;
        if (tileToSpawnOn == null) return;
        if (tileToSpawnOn.occupant != null) return;

        // Spawn object
        GameObject unitObject = Instantiate(unitPrefabToSpawn);
        Unit unitScript = unitObject.GetComponent<Unit>();

        // Set Position
        Vector3 spawnPos = tileToSpawnOn.transform.position;
        spawnPos.y += (unitScript.mySR.bounds.size.y / 2f) * 0.85f;
        unitObject.transform.position = spawnPos;

        // Set Parent
        if (unitScript.playerBot)
            unitObject.transform.parent = playerUnitParent;
        else
            unitObject.transform.parent = enemyUnitParent;

        // Update tile data
        tileToSpawnOn.UpdateOccupant(unitScript);

        // Update unit data
        unitScript.standingOn = tileToSpawnOn;

        // Add unit to stoarage
        UnitStorage unitStorage = GameObject.FindFirstObjectByType<UnitStorage>();
        if(unitScript.playerBot)
            unitStorage.playerUnits.Add(unitScript);
        else
            unitStorage.enemyUnits.Add(unitScript);
    }

    [ContextMenu("Remove unit")]
    private void RemoveUnitFromEditor()
    {
        // Exit points
        if(unitToRemove == null) return;

        // Remove from storage
        UnitStorage unitStorage = GameObject.FindFirstObjectByType<UnitStorage>();
        unitStorage.RemoveUnit(unitToRemove);

        // Remove from tile
        unitToRemove.standingOn.UpdateOccupant(null);

        // Delete unit
        DestroyImmediate(unitToRemove.gameObject);

    }
}

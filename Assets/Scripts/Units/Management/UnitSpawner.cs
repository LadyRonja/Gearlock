using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public static UnitSpawner Instance;

    public Transform playerUnitParent;
    public Transform enemyUnitParent; 
    [Space]
    [SerializeField] List<GameObject> unitPrefabsToSpawn = new();
    [SerializeField] List<Tile> tilesToSpawnThemOn = new();

    [Header("Decripit")]
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

        SpawnAUnit(unitPrefabToSpawn, tileToSpawnOn);
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

    [ContextMenu("Set units on board (destroys old)")]
    private void SpawnMultipleUnitsOnBoard()
    {
        // Bare minimum mess-up prevention
        if(unitPrefabsToSpawn.Count != tilesToSpawnThemOn.Count)
        {
            Debug.LogError("Each Unit requires their own tile");
            return;
        }

        // Remove all old units from storage
        UnitStorage _unitStoreage = FindObjectOfType<UnitStorage>();
        _unitStoreage.playerUnits = new();
        _unitStoreage.enemyUnits = new();

        // Destroy
        Object[] tiles = FindObjectsOfTypeAll(typeof(Tile));
        foreach(Tile t in tiles)
        {
            if(t.occupant != null)
            {
                DestroyImmediate(t.occupant.gameObject);
                t.UpdateOccupant(null);
            }
        }

        for (int i = 0; i < unitPrefabsToSpawn.Count; i++)
        {
            SpawnAUnit(unitPrefabsToSpawn[i], tilesToSpawnThemOn[i]);
        }
    }

    private void SpawnAUnit(GameObject unitPrefab, Tile onTile)
    {
        // Spawn object
        GameObject unitObject = Instantiate(unitPrefab);
        Unit unitScript = unitObject.GetComponent<Unit>();

        // Set Position
        Vector3 spawnPos = onTile.transform.position;
        if (unitScript.myMR != null)
            spawnPos.y += (unitScript.myMR.bounds.size.y / 2f) * 0.01f;
        else
            spawnPos.y += (unitScript.mySR.bounds.size.y / 2f) * 0.90f;
        unitObject.transform.position = spawnPos;

        // Set Parent
        if (unitScript.playerBot)
            unitObject.transform.parent = playerUnitParent;
        else
            unitObject.transform.parent = enemyUnitParent;

        // Update tile data
        onTile.UpdateOccupant(unitScript);

        // Update unit data
        unitScript.standingOn = onTile;

        // Add unit to stoarage
        UnitStorage unitStorage = GameObject.FindFirstObjectByType<UnitStorage>();
        if (unitScript.playerBot)
            unitStorage.playerUnits.Add(unitScript);
        else
            unitStorage.enemyUnits.Add(unitScript);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnUnitCard : PlayCard
{
    public GameObject unitPrefabToSpawn;

    public override void ExecuteBehaivour(Tile onTile, Unit byUnit)
    {
        Vector3 spawnpoint = onTile.transform.position;
        spawnpoint.y += 6;
        GameObject botObject = Instantiate(unitPrefabToSpawn, spawnpoint, Quaternion.identity);

        Unit botScript = botObject.GetComponent<Unit>();
        onTile.UpdateOccupant(botScript);
        botScript.standingOn = onTile;
        UnitStorage.Instance.playerUnits.Add(botScript);
        botScript.unitName += " "  + UnitStorage.Instance.playerUnits.Count;
        UnitSelector.Instance.UpdateSelectedUnit(botScript);
        UnitSelector.Instance.UpdatePlayerUnitUI();
    }
}

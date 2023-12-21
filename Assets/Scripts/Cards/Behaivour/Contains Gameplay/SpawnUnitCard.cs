using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnUnitCard : Card
{
    public GameObject unitPrefabToSpawn;

    public override void ExecuteBehaivour(Tile onTile, Unit byUnit)
    {
        Vector3 spawnpoint = onTile.transform.position;
        GameObject botObject = Instantiate(unitPrefabToSpawn, spawnpoint, Quaternion.identity);

        Unit botScript = botObject.GetComponent<Unit>();
        onTile.UpdateOccupant(botScript);
        AudioHandler.PlayRandomEffectFromList(botScript.getSpawnedSound);
        botScript.standingOn = onTile;
        UnitStorage.Instance.playerUnits.Add(botScript);
        botScript.unitName += " "  + UnitStorage.Instance.playerUnits.Count;
        UnitSelector.Instance.UpdateSelectedUnit(botScript);
        UnitSelector.Instance.UpdatePlayerUnitUI();

        spawnpoint.y += botScript.mySR.bounds.size.y / 2f;
        botObject.transform.position = spawnpoint;

        ConfirmCardExecuted();
    }


    public override void ConfirmCardExecuted()
    {
        myState = CardState.Finished;
    }

}

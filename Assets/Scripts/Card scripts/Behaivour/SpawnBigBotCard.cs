using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBigBotCard : PlayCard
{
    public GameObject bigBot;

    public override void ExecuteBehaivour(Tile onTile, Unit byUnit)
    {
        Vector3 spawnpoint = onTile.transform.position;
        spawnpoint.y += 6;
        GameObject botObject = Instantiate(bigBot, spawnpoint, Quaternion.identity);
        Unit bigBotScript = botObject.GetComponent<Unit>();
        onTile.UpdateOccupant(bigBotScript);
        bigBotScript.standingOn = onTile;

        UnitStorage.Instance.enemyUnits.Add(bigBotScript);

    }
}

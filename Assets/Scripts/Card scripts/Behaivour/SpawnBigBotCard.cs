using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBigBotCard : PlayCard
{
    public GameObject bigBot;

    public override void ExecuteBehaivour(Tile onTile, Unit byUnit)
    {
        Vector3 spawnpoint = onTile.transform.position;
        GameObject botObject = Instantiate(bigBot, spawnpoint, Quaternion.identity);
        Unit botScript = botObject.GetComponent<Unit>();
        onTile.UpdateOccupant(botScript);
        botScript.standingOn = onTile;

    }
}

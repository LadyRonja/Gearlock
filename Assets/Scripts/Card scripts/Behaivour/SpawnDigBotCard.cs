using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDigBotCard : PlayCard
{
    //placera dig bot 
    //inte p� dirt
    //inte p� obsticale
    //inte p� upptagen ruta

    //is spawnTile (tile verification)

    public GameObject digBot;
    

    public override void ExecuteBehaivour(Tile onTile, Unit byUnit)
    {
        Vector3 spawnpoint = onTile.transform.position;
        GameObject botObject = Instantiate(digBot, spawnpoint, Quaternion.identity);
        Unit botScript = botObject.GetComponent<Unit>();
        onTile.UpdateOccupant(botScript);
        botScript.standingOn = onTile;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDigBotCard : PlayCard
{
    //placera dig bot 
    //inte på dirt
    //inte på obsticale
    //inte på upptagen ruta

    //is spawnTile (tile verification)

    public GameObject digBot;

    private Unit nameBot;
    

    public override void ExecuteBehaivour(Tile onTile, Unit byUnit)
    {
        Vector3 spawnpoint = onTile.transform.position;
        spawnpoint.y += 6;
        GameObject botObject = Instantiate(digBot, spawnpoint, Quaternion.identity);
        
        Unit botScript = botObject.GetComponent<Unit>();
        onTile.UpdateOccupant(botScript);
        botScript.standingOn = onTile;
        UnitStorage.Instance.playerUnits.Add(botScript);
        botScript.unitName = "Digger " + UnitStorage.Instance.playerUnits.Count;


        
    }

    
}


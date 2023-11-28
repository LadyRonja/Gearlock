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
    

    public override void ExecuteBehaivour(Tile onTile, Unit byUnit)
    {
        Vector3 spawnpoint = onTile.transform.position;
        spawnpoint.y += 4;
        GameObject botObject = Instantiate(digBot, spawnpoint, Quaternion.identity);
        Unit botScript = botObject.GetComponent<Unit>();

        // Set BotSpecialization to Digger
        botScript.mySpecialization = BotSpecialization.Digger;

        onTile.UpdateOccupant(botScript);
        botScript.standingOn = onTile;
        UnitStorage.Instance.playerUnits.Add(botScript);
        


        //offset so it does not spawn in the ground
    }

    
}


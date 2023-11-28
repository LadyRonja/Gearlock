using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFightBotCard : PlayCard
{
    //placera fight bot 
    //inte på dirt
    //inte på obsticale
    //inte på upptagen ruta

    public GameObject fightBot;
  

    public override void ExecuteBehaivour(Tile onTile, Unit byUnit)
    {
        Vector3 spawnpoint = onTile.transform.position;
        spawnpoint.y += 6;
        GameObject botObject = Instantiate(fightBot, spawnpoint, Quaternion.identity);
        Unit botScript = botObject.GetComponent<Unit>();
        onTile.UpdateOccupant(botScript);
        botScript.standingOn = onTile;
        UnitStorage.Instance.playerUnits.Add(botScript);

        botScript.unitName = "Fighter " + UnitStorage.Instance.playerUnits.Count;
    }

   
}

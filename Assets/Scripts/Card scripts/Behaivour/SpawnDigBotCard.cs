using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDigBotCard : PlayCard
{
    public GameObject digBot;

    public override void ExecuteBehaivour(Tile onTile, Unit byUnit)
    {
        Vector3 spawnpoint = onTile.transform.position;
        spawnpoint.y += 6;
        GameObject botObject = Instantiate(digBot, spawnpoint, Quaternion.identity);
        
        Unit botScript = botObject.GetComponent<Unit>();
        onTile.UpdateOccupant(botScript);
        botScript.standingOn = onTile;
        UnitStorage.Instance.playerUnits.Add(botScript);
        UnitSelector.Instance.UpdateSelectedUnit(botScript);
        botScript.unitName = "Digger " + UnitStorage.Instance.playerUnits.Count;
        
    }   
}


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
    public Transform spawnPosition; 

    public override void ExecuteBehaivour(Tile onTile, Unit byUnit)
    {
        //throw new System.NotImplementedException();
        //is spawnTile (tile verification)

        Instantiate(digBot, spawnPosition.position, Quaternion.identity);

    }
}

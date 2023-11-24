using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDigBotCard : PlayCard
{
    //placera dig bot 
    //inte på dirt
    //inte på obsticale
    //inte på upptagen ruta

    public GameObject digBot;

    private Tile placeDigBot;

    public void PlacementDigBot()
    {
        if (placeDigBot.occupied == false)
        {
            GameObject newDigBot = Instantiate(digBot);
            newDigBot.transform.position = placeDigBot.transform.position;
            newDigBot.transform.rotation = Quaternion.identity;

            placeDigBot.occupied = true;
        }
    }
}

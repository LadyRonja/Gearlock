using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFightBotCard : PlayCard
{
    //placera fight bot 
    //inte p� dirt
    //inte p� obsticale
    //inte p� upptagen ruta

    public GameObject fightBot;

    private Tile placeFightBot;

    public void PlacementFightBot()
    {
        if (placeFightBot.occupied == false)
        {
            GameObject newFightBot = Instantiate(fightBot);
            newFightBot.transform.position = placeFightBot.transform.position;
            newFightBot.transform.rotation = Quaternion.identity;

            placeFightBot.occupied = true;
        }
    }
}

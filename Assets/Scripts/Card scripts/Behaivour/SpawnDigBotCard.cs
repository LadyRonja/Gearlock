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

    /*public override void ExecuteBehaivour(Tile onTile, Unit byUnit)
    {
        Debug.LogError("Not implemented");
    }*/

    public void Update()
    {
        //PlacementDigBot();
    }

    public void PlacementDigBot()
    {   if (Input.GetMouseButtonDown(0))
        {
            // Get the mouse position in screen coordinates
            Vector3 mousePosition = Input.mousePosition;

            //if (placeDigBot.occupied == false)
            //  {
            GameObject newDigBot = Instantiate(digBot, mousePosition, Quaternion.identity);
                //newDigBot.transform.position = placeDigBot.transform.position;
                //newDigBot.transform.rotation = Quaternion.identity;

                //placeDigBot.occupied = true;

                Debug.Log("Placed Digbot");
           // }

        }
            
    }

    public override void ExecuteBehaivour(Tile onTile, Unit byUnit)
    {
        throw new System.NotImplementedException();
    }
}

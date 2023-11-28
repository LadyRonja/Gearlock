using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamiteCard : PlayCard
{
    //kolla vart robortarna är 
    //Man kan inte targeta dirt
    // allt botar och fiender tar skada i en radie av 2 rutor från impact
    //exploderar direkt
    //(om hinns med: en seprat detonation action)

    public GameObject dynamite;
    public Animator explosion;

    [SerializeField]
    private int multiplier = 4;
    private int explosionRange = 2;
    

    public override void ExecuteBehaivour(Tile onTile, Unit byUnit)
    {
                //playerAnimator.SetTrigger("Exploding"); 

         if (onTile.occupant != null)
         {
             Vector3 spawnpoint = onTile.transform.position;
             GameObject newDynamite = Instantiate(dynamite, spawnpoint, Quaternion.identity);
             Unit dynamiteScript = newDynamite.GetComponent<Unit>();
             onTile.UpdateOccupant(dynamiteScript);
             dynamiteScript.standingOn = onTile;

             onTile.occupant.TakeDamage(byUnit.power * multiplier);


            Destroy(dynamite);

            ApplyDamageRadius(onTile, byUnit);       

                    //kolla grannar och lägg till i listan i alla fyra håll, for loop med dig eller damge
                    //den kan INTE hamna på dirt men kan pränga dirt
         }

                // Apply damage to units in a 2-tile radius
    }


    public void ApplyDamageRadius(Tile centerTile, Unit byUnit)
    {
        Tile[] allTiles = FindObjectsOfType<Tile>();

        // Loop through all tiles to find neighbors within the explosion radius
        foreach (Tile otherTile in allTiles)
        {
            // Skip the center tile
            if (otherTile == centerTile)
                continue;

            // Calculate the distance between tiles
            int distanceX = Mathf.Abs(centerTile.x - otherTile.x);
            int distanceY = Mathf.Abs(centerTile.y - otherTile.y);

            // Check if the otherTile is within the explosion radius
            if (distanceX <= explosionRange && distanceY <= explosionRange)
            {
                // Check if there is an occupant on the neighboring tile
                if (otherTile.occupant != null)
                {
                    // Apply damage to the unit on the neighboring tile
                    otherTile.occupant.TakeDamage(byUnit.power * multiplier);
                }
            }
        }

    }
}

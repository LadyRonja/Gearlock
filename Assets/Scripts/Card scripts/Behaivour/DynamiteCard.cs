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

         Vector3 spawnpoint = onTile.transform.position;
         spawnpoint.y += 3;
         GameObject dynamiteObject = Instantiate(dynamite, spawnpoint, Quaternion.identity);
             
         Unit dynamiteScript = dynamiteObject.GetComponent<Unit>();
         onTile.UpdateOccupant(dynamiteScript);
         dynamiteScript.standingOn = onTile;
         UnitStorage.Instance.playerUnits.Add(dynamiteScript);
         onTile.occupant.TakeDamage(byUnit.power * multiplier);
         Debug.Log("spawned dynamite");

            

            ApplyDamageRadius(onTile, byUnit);
            Destroy(dynamite);

        //kolla grannar och lägg till i listan i alla fyra håll, for loop med dig eller damge
        //den kan INTE hamna på dirt men kan pränga dirt


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
                // Store the original color of the tiles
                Color originalColor = otherTile.myMR.material.color;

                // Change neighbouring tiles to the color to blue
                otherTile.myMR.material.color = Color.blue;

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamiteCard : PlayCard
{
    //kolla vart robortarna �r 
    //Man kan inte targeta dirt
    // allt botar och fiender tar skada i en radie av 2 rutor fr�n impact
    //exploderar direkt
    //(om hinns med: en seprat detonation action)

    public GameObject dynamite;
    public Animator explosion;

    [SerializeField]
    private int multiplier = 4;
    private int explosionRange;
    

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

                    //onTile.occupant.TakeDamage(byUnit.attackRange  ); //2 rutor �t alla h�ll

                    //kolla grannar och l�gg till i listan i alla fyra h�ll, for loop med dig eller damge
                    //den kan INTE hamna p� dirt men kan pr�nga dirt
         }


                // Apply damage to units in a 2-tile radius
            

        
    }
   


}

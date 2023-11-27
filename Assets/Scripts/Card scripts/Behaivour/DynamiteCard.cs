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
    private Tile placeDynamite;
    private int explosionRange;
    

    public override void ExecuteBehaivour(Tile onTile, Unit byUnit)
    {

                GameObject newDynamite = Instantiate(dynamite);
                newDynamite.transform.position = placeDynamite.transform.position;
                newDynamite.transform.rotation = Quaternion.identity;

                //playerAnimator.SetTrigger("Exploding"); 

                if (onTile.occupant != null)
                {
                    onTile.occupant.TakeDamage(byUnit.power * multiplier);

                    //onTile.occupant.TakeDamage(byUnit.attackRange  ); //2 rutor åt alla håll

                    //kolla grannar och lägg till i listan i alla fyra håll, for loop med dig eller damge
                    //den kan INTE hamna på dirt men kan pränga dirt
                }


                // Apply damage to units in a 2-tile radius
            

        
    }
   


}

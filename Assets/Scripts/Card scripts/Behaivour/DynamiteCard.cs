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
    private int multiplier = 1;

    private Unit hitPower;
    private Unit takeDamage;
    private Tile placeDynamite;
    private SpawnDigBotCard digBotPos;
    private SpawnFightBotCard fightBotPos;

    public override void ExecuteBehaivour(Tile onTile, Unit byUnit)
    {
        Debug.LogError("Not implemented");
    }
    public void PlacementDynamite()
    {
        if (placeDynamite.containesDirt == false) 
        {
            GameObject newDynamite = Instantiate(dynamite);
            newDynamite.transform.position = placeDynamite.transform.position;
            newDynamite.transform.rotation = Quaternion.identity;

            //playerAnimator.SetTrigger("Exploding"); 

            int dynamitePower = hitPower.power; //ref to power variabel i Unit
            takeDamage.TakeDamage(dynamitePower * multiplier * 4); // power x4

            // Apply damage to units in a 2-tile radius
           

        }
        
        
    }


}

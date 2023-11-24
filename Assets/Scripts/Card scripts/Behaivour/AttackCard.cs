using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCard : PlayCard
{
    [SerializeField]
    private int multiplier = 1;
    
    private Tile enemyPlaced; // ref till var enemy står
    private Unit hitPower;
    private Unit takeDamage;
    
    public void Attack()
    {
        //if targetable gör damage 
        if (enemyPlaced.targetable == true) //ref till Tile varibel occupied 
        {
           
            int botPower = hitPower.power; //ref to power variabel i Unit
            takeDamage.TakeDamage(botPower * multiplier); // ger damage = power, tex 5 power tar 5 hp av fienden
        }

    }

    public override void ExecuteBehaivour(Tile onTile, Unit byUnit)
    {
        if(onTile.occupant != null)
        {
            onTile.occupant.TakeDamage(byUnit.power * multiplier);
        }
    }
}

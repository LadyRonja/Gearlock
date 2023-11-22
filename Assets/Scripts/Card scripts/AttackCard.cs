using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCard : PlayCard
{
    private Tile enemyPlaced; // ref till var enemy står
    private Unit hitPower;
    private Unit takeDamage;
    
    public void Attack()
    {
        //if occupied gör damage 
        if (enemyPlaced.occupied == true) //ref till Tile varibel occupied 
        {
            int botPower = hitPower.power; //ref to power variabel i Unit
            takeDamage.TakeDamage(botPower); // ger damage = power, tex 5 power tar 5 hp av fienden
        }

    }
}

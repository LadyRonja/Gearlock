using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DoubleAttackCard : PlayCard
{
    [SerializeField]
    private int multiplier = 1;

    private Tile enemyPlaced; // ref till var enemy står
    private Unit hitPower;
    private Unit takeDamage;

    public void DoubleAttack()
    {
        //if targetable gör damage 
        if (enemyPlaced.targetable == true) //ref till Tile varibel occupied 
        {
            int botPower = hitPower.power; //ref to power variabel i Unit
            takeDamage.TakeDamage(botPower * multiplier * 2); // ger damage = power, tex 5 power tar 5 hp av fienden
        }

    }
}

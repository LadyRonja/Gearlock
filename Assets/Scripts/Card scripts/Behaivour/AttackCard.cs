using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCard : PlayCard
{
    [SerializeField]
    private int multiplier = 1;
    
   
    
    public override void ExecuteBehaivour(Tile onTile, Unit byUnit)
    {
        if(onTile.occupant != null)
        {
            onTile.occupant.TakeDamage(byUnit.power * multiplier);
        }
    }
}

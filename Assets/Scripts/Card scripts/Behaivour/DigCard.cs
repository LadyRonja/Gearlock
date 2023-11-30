using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigCard : PlayCard
{
  public Animator digAnimation;

    

    public override void ExecuteBehaivour(Tile onTile, Unit byUnit)
    {
        if (onTile.containsDirt == true)
        {
            onTile.RemoveDirt();

        }
    }


}

//om man kan klicka på tilen så kollar man efter  public bool containesDirt;






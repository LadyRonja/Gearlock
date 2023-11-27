using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigCard : PlayCard
{
  public Animator digAnimation;
  private Tile dirtPile;

    

    public override void ExecuteBehaivour(Tile onTile, Unit byUnit)
    {
        if (dirtPile.containesDirt == true)
        {
            dirtPile.RemoveDirt();

        }
    }


}

//om man kan klicka på tilen så kollar man efter  public bool containesDirt;






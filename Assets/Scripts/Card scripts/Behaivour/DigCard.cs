using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigCard : PlayCard
{
  public Animator digAnimation;
  private Tile dirtPile;

    public void Dig()
    {
        if (dirtPile.containesDirt == true)
        {
            dirtPile.RemoveDirt();
            
        }
        
       //playerAnimator.SetTrigger("Digging"); 
    }

    public override void ExecuteBehaivour(Tile onTile, Unit byUnit)
    {
        Debug.LogError("Not implemented");
    }


}

//om man kan klicka p� tilen s� kollar man efter  public bool containesDirt;






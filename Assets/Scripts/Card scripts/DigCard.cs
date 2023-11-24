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

    


}

//om man kan klicka på tilen så kollar man efter  public bool containesDirt;






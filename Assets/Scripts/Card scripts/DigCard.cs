using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigCard : PlayCard
{
  public Animator digAnimation;
  private Tile dirtPile;

    public void Dig()
    {
        if (dirtPile != null)
        {
            dirtPile.RemoveDirt();

            //playerAnimator.SetTrigger("Digging");

        }
    }







}

    




using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigCard : Card
{
    public override void ExecuteBehaivour(Tile onTile, Unit byUnit)
    {
        onTile.RemoveDirt();
        ConfirmCardExecuted();
    }
    public override void ConfirmCardExecuted()
    {
        myState = CardState.Finished;
    }
}







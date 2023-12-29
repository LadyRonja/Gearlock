using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigCard : Card
{
    public override void ExecuteBehaivour(Tile onTile, Unit byUnit)
    {
        onTile.RemoveDirt();
        ConfirmCardExecuted();
        FlipUnitBasedOnClickedTile(byUnit, onTile);
        byUnit.PlayActionAnimation();
    }

    private void FlipUnitBasedOnClickedTile(Unit unit, Tile clickedTile)
    {
        if (clickedTile != null)
        {
            // Call the corrected FlipOnXAxis method
            unit.FlipOnXAxis(clickedTile);
        }
    }

    public override void ConfirmCardExecuted()
    {
        myState = CardState.Finished;
    }
}







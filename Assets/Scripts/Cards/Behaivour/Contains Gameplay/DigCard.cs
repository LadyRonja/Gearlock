using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigCard : Card
{
    [SerializeField] List<AudioClip> miningSounds = new();

    public override void ExecuteBehaivour(Tile onTile, Unit byUnit)
    {
        Invoke("PlayPickSound", 0.2f);
        onTile.RemoveDirt();
        GameStats.Instance.IncreaseRocksMined();
        ConfirmCardExecuted();
        FlipUnitBasedOnClickedTile(byUnit, onTile);
        byUnit.PlayActionAnimation();
    }

    private void PlayPickSound()
    {
        AudioHandler.PlayRandomEffectFromList(miningSounds);
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







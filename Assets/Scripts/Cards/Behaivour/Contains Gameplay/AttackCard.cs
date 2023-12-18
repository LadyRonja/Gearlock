using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AttackCard : Card
{
    [SerializeField] private int multiplier = 1;

    public override void ExecuteBehaivour(Tile onTile, Unit byUnit)
    {
        if(byUnit.playerBot && onTile.occupant.playerBot)
        {
            FriendlyFirePopUp.Instance.OpenPopUp();
            FriendlyFirePopUp.Instance.ConfirmButton.onClick.AddListener(delegate() { ConfirmAttack(onTile, byUnit); });
            FriendlyFirePopUp.Instance.CancelButton.onClick.AddListener(delegate () { CancelAttack(); });
        }
    }

    private void ConfirmAttack(Tile onTile, Unit byUnit)
    {
        FriendlyFirePopUp.Instance.ClosePopUp();
        onTile.occupant.TakeDamage(byUnit.power * multiplier);
        ConfirmCardExecuted();
    }

    private void CancelAttack()
    {
        FriendlyFirePopUp.Instance.ClosePopUp();
        selectedTile = null;
        tilesHighligthed = false;
        cardExecutionCalled = false;
        myState = CardState.SelectingTile;
    }

    public override void ConfirmCardExecuted()
    {
        myState = CardState.Finished;
    }
}

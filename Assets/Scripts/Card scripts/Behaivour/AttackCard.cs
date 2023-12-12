using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class AttackCard : PlayCard
{
    [SerializeField]
    private int multiplier = 1;

    public GameObject verificationText;

    private Unit byUnit;
    private Tile onTile;
    

    public void Awake()
    {
        if (verificationText != null)
            verificationText.SetActive(false);

        
    }

    public override void ExecuteBehaivour(Tile onTile, Unit byUnit)
    {
        /*if(onTile.occupant != null)
        {
            onTile.occupant.TakeDamage(byUnit.power * multiplier);

            if (byUnit.playerBot)
            {
                Debug.LogError("do you want to hit your bot?");
                verificationText.SetActive(true);

                //SetVerificationPanelActive(true);
            }
            else
            {
                
            }
        }*/
        onTile.occupant.TakeDamage(byUnit.power * multiplier);

    }

   /* public void ClickedYes()
    {
        verificationText.SetActive(false);
        onTile.occupant.TakeDamage(byUnit.power * multiplier);
    }

    public void ClickedNo()
    {
        verificationText.SetActive(false);
    }*/
}

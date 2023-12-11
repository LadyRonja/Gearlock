using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackCard : PlayCard
{
    [SerializeField]
    private int multiplier = 1;

    //public GameObject verificationText;
    

    /*public void Awake()
    {
        verificationText.gameObject.SetActive(false);
    }*/

    public override void ExecuteBehaivour(Tile onTile, Unit byUnit)
    {
        if(onTile.occupant != null)
        {
            if(byUnit.playerBot)
            {
                Debug.LogError("do you want to hit your bot?");
                //verificationText.gameObject.SetActive(true);
                //SetVerificationPanelActive(true);
            }
            else
            {
                onTile.occupant.TakeDamage(byUnit.power * multiplier);
            }
        }

        
    }

    private void SetVerificationPanelActive(bool active)
    {
        //verificationText.SetActive(active);
    }
}

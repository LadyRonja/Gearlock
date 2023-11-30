using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : Unit
{
    public GameObject brokenFighter;
    public override void Die()
    {
        if (brokenFighter != null)
        {
            Vector3 breakPosition = new Vector3(transform.position.x, transform.position.y - 2, transform.position.z);
            GameObject deadFighter = Instantiate(brokenFighter, breakPosition, transform.rotation);
            CardPickUp cardPickUpScript = deadFighter.GetComponent<CardPickUp>();
            standingOn.myPickUp = cardPickUpScript;
        }
        else
        {
            Debug.LogError("brokenFighter GameObject is not assigned.");
        }

        base.Die();
    }
}

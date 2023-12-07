using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Digger : Unit
{
    public GameObject brokenDigger;

    public GameObject infoTextDigger;

    public void Start()
    {
        //infoTextDigger.SetActive(false);
    }
    public override void Die()
    {
        if (brokenDigger != null)
        {
            Vector3 breakPosition = new Vector3(transform.position.x, transform.position.y - 4, transform.position.z);
            GameObject deadRobot = Instantiate(brokenDigger, breakPosition, transform.rotation);
            CardPickUp cardPickUpScript = deadRobot.GetComponent<CardPickUp>();
            standingOn.myPickUp = cardPickUpScript;
        }
        else
        {
            Debug.LogError("brokenDigger GameObject is not assigned.");
        }

        base.Die();
    }

    public void OnMouseEnter()
    {
        //infoTextDigger.SetActive(true);
    }

    public void OnMouseExit()
    {
        //infoTextDigger.SetActive(false);
    }

}

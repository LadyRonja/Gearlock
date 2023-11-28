using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Digger : Unit
{
    public GameObject brokenDigger;
    public override void Die()
    {
        if (brokenDigger != null)
        {
            Vector3 position = new Vector3(transform.position.x, transform.position.y - 4, transform.position.z);
            GameObject instantiatedObject = Instantiate(brokenDigger, position, transform.rotation);
        }
        else
        {
            Debug.LogError("brokenDigger GameObject is not assigned.");
        }

        base.Die();
    }
}

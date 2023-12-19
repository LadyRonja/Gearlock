using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynomiteProjectile : Projectile
{

    public override void OnArrival()
    {
        Debug.Log("Dynomite has landed!");

        ConfirmArrival();
    }

    public override void ConfirmArrival()
    {
        activator.ConfirmCardExecuted();
    }

}

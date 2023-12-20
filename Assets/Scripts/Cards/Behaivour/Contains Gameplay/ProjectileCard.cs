using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCard : Card
{
    [Header("Projectile Stats")]
    public GameObject projectilePrefab;
    public float secondsToArrive = 1f;
    public float startDelay = 0.5f;

    public override void ExecuteBehaivour(Tile onTile, Unit byUnit)
    {
        GameObject projectileObj = Instantiate(projectilePrefab, byUnit.transform.position, Quaternion.identity);
        Projectile projectileScr = projectileObj.GetComponent<Projectile>();
        projectileScr.SetUpProjectile(byUnit, onTile, this, secondsToArrive, startDelay);
        projectileScr.StartMovement();

        StartCoroutine(SafetyFail());
    }

    private IEnumerator SafetyFail()
    {
        yield return new WaitForSeconds(secondsToArrive * 10f);
        Debug.LogError("Projectile did not call to finish card, force finishing");
        ConfirmCardExecuted();
        yield return null;

    }

    public override void ConfirmCardExecuted()
    {
        this.StopAllCoroutines();
        myState = CardState.Finished;
    }
}

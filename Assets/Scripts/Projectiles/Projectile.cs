using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [Header("Required information")]
    [HideInInspector] public Unit thrower;
    [HideInInspector] public Card activator;
    [HideInInspector] public Tile targetTile;
    public float secondsToArrive = 1f;
    public float maxYOffSet = 1f;
    public float startDelay = 0.5f;

    [Header("Components")]
    [SerializeField] protected GameObject gfx;

    public abstract void OnArrival();
    public abstract void ConfirmArrival();

    public void SetUpProjectile(Unit thrower, Tile targetTile, Card activator)
    {
        this.thrower = thrower;
        this.targetTile = targetTile;
        this.activator = activator;
    }

    public void SetUpProjectile(Unit thrower, Tile targetTile, Card activator, float secondsToArrive)
    {
        SetUpProjectile(thrower, targetTile, activator);
        this.secondsToArrive = secondsToArrive;
    }

    public void SetUpProjectile(Unit thrower, Tile targetTile, Card activator, float secondsToArrive, float startDelay)
    {
        SetUpProjectile(thrower, targetTile, activator, secondsToArrive);
        this.startDelay = startDelay;
    }

    public void StartMovement()
    {
        StartCoroutine(MoveToTarget(startDelay));
    }

    protected virtual IEnumerator MoveToTarget(float startDelay)
    {
        yield return new WaitForSeconds(startDelay);

        Vector3 startPos = this.transform.position;
        Vector3 endPos = targetTile.transform.position;

        float timeToMove = secondsToArrive;
        float timePassed = 0;

        while (timePassed < timeToMove)
        {
            transform.position = Vector3.Lerp(startPos, endPos, (timePassed / timeToMove));

            CameraController.Instance.MoveToTarget(this.transform.position, 0.01f);

            // Lobbing          
            float yOffSet = gfx.transform.localPosition.y;
            yOffSet = Mathf.Max(0, maxYOffSet * Mathf.Sin(timePassed / timeToMove * Mathf.PI));
            gfx.transform.localPosition = new Vector3(gfx.transform.localPosition.x, yOffSet, gfx.transform.localPosition.z);

            timePassed += Time.deltaTime;
            yield return null;
        }
        gfx.transform.position = transform.position;

        OnArrival();
        yield return null;
    }
}

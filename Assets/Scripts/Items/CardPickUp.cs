using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardPickUp : MonoBehaviour
{
    public GameObject cardToAdd;
    public AnimationCurve myCurve;
    float yOffSet = 0f;
    Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        yOffSet = myCurve.Evaluate(Time.time % myCurve.length);
        transform.position = new Vector3(startPos.x, startPos.y + yOffSet, startPos.z);
    }

    public void OnMouseEnter()
    {
        MouseControl.instance.Pickup();
    }

    public void OnMouseExit()
    {
        MouseControl.instance.Default();
    }
}

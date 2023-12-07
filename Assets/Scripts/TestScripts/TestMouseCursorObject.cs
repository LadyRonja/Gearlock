using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMouseCursorObject : MonoBehaviour
{
    public void OnMouseEnter()
    {
        MouseControl.instance.Fight();
    }

    public void OnMouseExit()
    {
        MouseControl.instance.Default();
    }
}

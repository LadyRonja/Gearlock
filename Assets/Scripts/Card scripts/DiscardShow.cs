using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardShow : MonoBehaviour
{
    // Start is called before the first frame update

    public void MoveToScreen()
    {
        transform.position = new Vector3(960, 540, 0);
    }

    public void MoveOffScreen()
    {
        transform.position = new Vector3(4000, 0, 0);
    }

}

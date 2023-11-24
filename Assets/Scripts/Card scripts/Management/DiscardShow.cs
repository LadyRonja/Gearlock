using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DiscardShow : MonoBehaviour
{
    public bool displayingDiscard;

    // Start is called before the first frame update

    public void MoveToScreen()
    {
        transform.position = new Vector3(960, 540, 0);
        displayingDiscard = true;
    }

    public void MoveOffScreen()
    {
        transform.position = new Vector3(4000, 0, 0);
        displayingDiscard = false;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddedToDiscard : MonoBehaviour
{

    public static AddedToDiscard Instance;

    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepCard : MonoBehaviour
{

    public static KeepCard Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);

        transform.parent.gameObject.SetActive(false);
    }
}

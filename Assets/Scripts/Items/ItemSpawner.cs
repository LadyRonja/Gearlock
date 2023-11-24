using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public static ItemSpawner Instance;


    private void Awake()
    {
        #region Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
        #endregion
    }
}

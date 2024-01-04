using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataHandler : MonoBehaviour
{
    private static DataHandler instance;

    public bool toggleZoom = false;
    public bool toggleClick = false;
    public bool toggleDrag = false;

    public int musicVolume = 60;
    public int effectVolume = 60;


    public static DataHandler Instance
    {
        get { return instance; }
        private set { instance = value; }
    }
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }
}

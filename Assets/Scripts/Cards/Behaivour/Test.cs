using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject testPrefab;

    public Transform testTransform;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(testPrefab, testTransform.position, Quaternion.identity);
            Debug.Log("testspawn");
        }
    }
}

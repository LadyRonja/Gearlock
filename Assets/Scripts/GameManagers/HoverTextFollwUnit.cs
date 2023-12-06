using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverTextFollwUnit : MonoBehaviour
{
    public Transform unit;

    private Vector3 originalLocalPosition;

    void Start()
    {
        // Store the original local position of the text object
        originalLocalPosition = transform.localPosition;
    }

    void Update()
    {
        
            // Set the global position of the text object to match the unit's position
            transform.position = unit.position;

            // Add the original local position as an offset
           transform.localPosition = originalLocalPosition;
       
        
    }
}

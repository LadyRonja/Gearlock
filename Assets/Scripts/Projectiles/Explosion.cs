using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] AnimationClip myClip;

    private void Start()
    {
        Destroy(this.gameObject, myClip.length);
    }
}

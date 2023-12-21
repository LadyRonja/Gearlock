using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] AnimationClip myClip;
    [SerializeField] AudioClip explosionSound;

    private void Start()
    {
        AudioHandler.PlaySoundEffect(explosionSound);
        Destroy(this.gameObject, myClip.length);
    }
}

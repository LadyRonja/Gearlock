using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] AnimationClip myClip;
    [SerializeField] List<AudioClip> explosionSounds;

    private void Start()
    {
        AudioHandler.PlayRandomEffectFromList(explosionSounds);
        Destroy(this.gameObject, myClip.length);
    }
}

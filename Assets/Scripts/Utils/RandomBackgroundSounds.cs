using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBackgroundSounds : MonoBehaviour
{
    [SerializeField] List<AudioClip> sounds;

    [SerializeField] float minDelaySec = 50f;
    [SerializeField] float maxDelaySec = 140f;

    private void Start()
    {
        float rand = Random.Range(minDelaySec, maxDelaySec);
        Invoke("PlayRandomSoundRecusrive", rand);
    }

    private void PlayRandomSoundRecusrive()
    {
        AudioHandler.PlayRandomEffectFromList(sounds);

        float rand = Random.Range(minDelaySec, maxDelaySec);
        Invoke("PlayRandomSoundRecusrive", rand);
    }
}

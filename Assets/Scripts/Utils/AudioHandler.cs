using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioHandler : MonoBehaviour
{
    private static AudioHandler instance;
    public static AudioHandler Instance { get => GetInstance(); private set => instance = value; }
    public static bool deleteOtherSources = true;

    [Header("Music")]
    [SerializeField] AudioClip music;
    AudioSource musicPlayer;

    [Header("Effects")]
    [SerializeField] int initialEffectSourceCount = 10;
    List<AudioSource> effectSources = new();

    private void Awake()
    {
        #region Singleton
        if (instance == null || instance == this)
            instance = this;
        else
        {
            Destroy(this.gameObject);
            return;
        }
        #endregion

        if(deleteOtherSources)
            DestroyAllOtherSources();

        SetUpMusicPlayer();
        ExandSourceCount(initialEffectSourceCount);
    }

    public static void PlaySoundEffect(AudioClip clipToPlay)
    {
        if (clipToPlay == null) return;

        // Loop through all effect sources in Instance until a free one plays
        // If no free source is available, make more sources

        // Potential optimization would be to just play from one of the new sources instantly, rather than looping through again.
        // The reason this isn't done, is because Unity has previously created/destroyed objects at end of frame while continuing the stack

        bool soundStartedPlaying = false;
        while (!soundStartedPlaying)
        {
            foreach (AudioSource s in Instance.effectSources)
            {
                if (s.isPlaying)
                    continue;

                if (Scenehandler.Instance != null)
                    s.volume = Scenehandler.Instance.effectVolume;
                else
                    s.volume = 1;

                s.PlayOneShot(clipToPlay);
                soundStartedPlaying = true;
                break;
            }

            if (!soundStartedPlaying)
                Instance.ExandSourceCount(10);
        }

    }

    public static void PlayRandomEffectFromList(List<AudioClip> possibleClips)
    {
        if(possibleClips == null) return;
        if(possibleClips.Count == 0) return;

        int rand = Random.Range(0, possibleClips.Count);
        AudioClip clipToPlay = possibleClips[rand];

        PlaySoundEffect(clipToPlay);
    }

    private void SetUpMusicPlayer()
    {
        if (music == null)
            music = Resources.Load<AudioClip>("Music/Music");

        if(musicPlayer == null)
            musicPlayer = this.gameObject.AddComponent<AudioSource>();

        musicPlayer.loop = true;
        if(Scenehandler.Instance != null)
            musicPlayer.volume = Scenehandler.Instance.musicVolume;
        else
            musicPlayer.volume = 1f;

        if (!musicPlayer.isPlaying)
        {
            musicPlayer.clip = music;
            musicPlayer.Play();
        }
    }

    private void ExandSourceCount(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject newSourceObject = new GameObject("Effect Source");
            newSourceObject.transform.parent = transform;
            AudioSource newSource = newSourceObject.AddComponent<AudioSource>();

            if(Scenehandler.Instance != null)
                newSource.volume = Scenehandler.Instance.effectVolume;
            else
                newSource.volume = 1f;

            effectSources.Add(newSource);
        }
    }

    private void DestroyAllOtherSources()
    {
        AudioSource[] oldSources = Object.FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
        foreach (AudioSource o in oldSources)
        {
            AudioHandler thisCheck = o.GetComponent<AudioHandler>();
            if (thisCheck != null)
                if (thisCheck == this)
                    return;

            Destroy(o.transform.gameObject);
        }
    }

    private static AudioHandler GetInstance()
    {
        if(instance != null)
            return instance;

        GameObject newManager = new GameObject("AudioManager");
        instance = newManager.AddComponent<AudioHandler>();
        return instance;
    }
}

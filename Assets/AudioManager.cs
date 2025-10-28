using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // Singleton

    [Header("Audio Sources")]
    public AudioSource musicSource; // For background music
    public AudioSource sfxSource;   // For sound effects

    [Header("Clips")]
    public List<AudioClip> soundClips; // Assign in Inspector

    private Dictionary<string, AudioClip> clipDict;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;

            // Convert list into dictionary for fast lookup
            clipDict = new Dictionary<string, AudioClip>();
            foreach (var clip in soundClips)
            {
                if (!clipDict.ContainsKey(clip.name))
                    clipDict.Add(clip.name, clip);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ====================
    // MUSIC
    // ====================
    public void PlayMusic(string clipName, bool loop = true)
    {
        if (clipDict.ContainsKey(clipName))
        {
            musicSource.clip = clipDict[clipName];
            musicSource.loop = loop;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("Music clip not found: " + clipName);
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    // ====================
    // SOUND EFFECTS
    // ====================
    public void PlaySFX(string clipName)
    {
        if (clipDict.ContainsKey(clipName))
        {
            sfxSource.PlayOneShot(clipDict[clipName]);
        }
        else
        {
            Debug.LogWarning("SFX clip not found: " + clipName);
        }
    }

    // ====================
    // CONTROLS
    // ====================
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}

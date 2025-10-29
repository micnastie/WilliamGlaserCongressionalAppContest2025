using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; 
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;  

    [Header("Clips")]
    public List<AudioClip> soundClips;

    private Dictionary<string, AudioClip> clipDict;

    private void Awake()
    {
       
        if (Instance == null)
        {
            Instance = this;

          
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

   
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}

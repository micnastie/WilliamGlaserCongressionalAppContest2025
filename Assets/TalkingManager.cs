using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class TalkingManager : MonoBehaviour
{
    [Header("Talking Settings")]
    public AudioClip talkingClip;
    public float loopOverlapSeconds = 0.05f; // Small overlap to hide gaps
    public float defaultVolume = 1f;
    public float reducedVollume;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = talkingClip;
        audioSource.loop = false;
        audioSource.volume = defaultVolume;
    }

    private void Start()
    {
        if (talkingClip != null)
        {
            StartCoroutine(LoopCoroutine());
        }
    }

    private IEnumerator LoopCoroutine()
    {
        double nextStartTime = AudioSettings.dspTime;

        while (true)
        {
            audioSource.PlayScheduled(nextStartTime);
            nextStartTime += talkingClip.length - loopOverlapSeconds;
            yield return new WaitForSecondsRealtime((float)(talkingClip.length - loopOverlapSeconds));
        }
    }

    /// <summary>
    /// Fade volume down over given seconds
    /// </summary>
    public void FadeOut(float duration)
    {
        StartCoroutine(FadeVolume(audioSource.volume, 0f, duration));
    }

    /// <summary>
    /// Fade volume up over given seconds
    /// </summary>
    public void FadeIn(float duration)
    {
        StartCoroutine(FadeVolume(audioSource.volume, defaultVolume, duration));
    }

    private IEnumerator FadeVolume(float start, float target, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, target, elapsed / duration);
            yield return null;
        }

        audioSource.volume = target;
    }


    public void SetTalkingVolume(bool StartingConvo)
    {
        if(StartingConvo) {
            StartCoroutine(FadeVolume(audioSource.volume, reducedVollume, .3f)); 
        }
        else
        {
            StartCoroutine(FadeVolume(audioSource.volume, defaultVolume, .3f));
        }
    }
}
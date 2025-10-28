using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFade : MonoBehaviour
{
    [Header("Fade Settings")]
    public Image fadeImage;        // Full-screen UI Image
    public float fadeDuration = 2f;
    public bool skipFadeIn = false;

    [Header("Audio Sources to Fade (optional)")]
    public AudioSource[] audioSources; // This will be auto-filled on Start

    private void Start()
    {
        // Automatically find all AudioSources in the scene
        if (audioSources == null || audioSources.Length == 0)
        {
            audioSources = FindObjectsOfType<AudioSource>();
        }

        if (skipFadeIn)
        {
            fadeImage.gameObject.SetActive(false);
            return;
        }

        fadeImage.gameObject.SetActive(true);
        fadeImage.color = new Color(0f, 0f, 0f, 1f);
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        yield return new WaitForSeconds(0.5f);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            fadeImage.color = new Color(0f, 0f, 0f, 1f - t);

            yield return null;
        }

        fadeImage.color = new Color(0f, 0f, 0f, 0f);
        fadeImage.gameObject.SetActive(false);
    }

    public void FadeOut()
    {
        fadeImage.gameObject.SetActive(true);
        StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        float elapsed = 0f;

        // Store initial volumes
        float[] initialVolumes = new float[audioSources.Length];
        for (int i = 0; i < audioSources.Length; i++)
        {
            if (audioSources[i] != null)
                initialVolumes[i] = audioSources[i].volume;
        }

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            // Fade screen
            fadeImage.color = new Color(0f, 0f, 0f, t);

            // Fade all audio sources
            for (int i = 0; i < audioSources.Length; i++)
            {
                if (audioSources[i] != null)
                    audioSources[i].volume = Mathf.Lerp(initialVolumes[i], 0f, t);
            }

            yield return null;
        }

        fadeImage.color = new Color(0f, 0f, 0f, 1f);

        // Ensure all audio is silent
        for (int i = 0; i < audioSources.Length; i++)
        {
            if (audioSources[i] != null)
                audioSources[i].volume = 0f;
        }
    }
}

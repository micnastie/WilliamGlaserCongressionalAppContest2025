using UnityEngine;

public class AudioScaler : MonoBehaviour
{
    [Header("Audio Clips (used for visualization)")]
    public AudioClip introClip;
    public AudioClip loopClip;

    [Header("UI / Object Settings")]
    public RectTransform targetUI;      // UI element to scale
    public float sensitivity = 5f;      // How much it reacts
    public float smoothSpeed = 5f;      // Smooth interpolation

    private Vector3 initialScale;
    private int introSamplesLength;
    private int loopSamplesLength;

    private float timer = 0f;
    private bool playingIntro = true;

    private float[] samples = new float[512];

    void Start()
    {
        if (targetUI == null)
            targetUI = GetComponent<RectTransform>();

        initialScale = targetUI.localScale;

        if (introClip != null)
            introSamplesLength = introClip.samples;

        if (loopClip != null)
            loopSamplesLength = loopClip.samples;
    }

    void Update()
    {
        // Determine which clip we are “playing”
        AudioClip currentClip = playingIntro ? introClip : loopClip;
        if (currentClip == null) return;

        // Calculate sample index based on timer
        int sampleIndex = (int)(timer * currentClip.frequency);
        sampleIndex = Mathf.Clamp(sampleIndex, 0, currentClip.samples - 1);

        // Read a small block of samples
        int blockSize = Mathf.Min(512, currentClip.samples - sampleIndex);
        currentClip.GetData(samples, sampleIndex);

        // Compute average amplitude
        float sum = 0f;
        for (int i = 0; i < blockSize; i++)
            sum += Mathf.Abs(samples[i]);

        float average = sum / blockSize;

        // Scale target
        float targetScale = Mathf.Clamp(1 + average * sensitivity, 1f, 1.25f);
        targetUI.localScale = Vector3.Lerp(targetUI.localScale, initialScale * targetScale, Time.deltaTime * smoothSpeed);

        // Advance timer
        timer += Time.deltaTime;

        // Switch from intro to loop if needed
        if (playingIntro && timer >= introClip.length)
        {
            playingIntro = false;
            timer = 0f;
        }

        // Loop the main clip
        if (!playingIntro && timer >= loopClip.length)
        {
            timer = 0f;
        }
    }
}

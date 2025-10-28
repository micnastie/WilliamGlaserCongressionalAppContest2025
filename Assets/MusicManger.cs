using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManger : MonoBehaviour
{
    public static MusicManger Instance;

    private AudioSource introSource;
    private AudioSource loopSource;

    [Header("Music Settings")]
    public AudioClip introTrack;  // Plays once
    public AudioClip loopTrack;   // Loops forever

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Create two AudioSources
        introSource = gameObject.AddComponent<AudioSource>();
        loopSource = gameObject.AddComponent<AudioSource>();

        introSource.loop = false;
        loopSource.loop = true;
    }

    void Start()
    {
        if (introTrack != null && loopTrack != null)
        {
            PlayIntroThenLoop();
        }
        else if (loopTrack != null)
        {
            loopSource.clip = loopTrack;
            loopSource.Play();
        }
    }

    void PlayIntroThenLoop()
    {
        introSource.clip = introTrack;
        introSource.Play();

        // Schedule loopSource to start exactly when intro ends
        double startTime = AudioSettings.dspTime + introTrack.length;
        loopSource.clip = loopTrack;
        loopSource.PlayScheduled(startTime);
    }

    public void SetVolume(float volume)
    {
        introSource.volume = Mathf.Clamp01(volume);
        loopSource.volume = Mathf.Clamp01(volume);
    }
}

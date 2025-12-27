using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Music Settings")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] [Range(0f, 1f)] private float defaultVolume = 0.5f;
    [SerializeField] private bool playOnStart = true;

    private AudioSource audioSource;

    void Awake()
    {
        // Singleton pattern - only one music manager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSource();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (playOnStart && backgroundMusic != null)
        {
            PlayMusic();
        }
    }

    private void InitializeAudioSource()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = defaultVolume;

        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
        }
    }

    public void PlayMusic()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void StopMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void PauseMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    public void UnpauseMusic()
    {
        if (audioSource != null)
        {
            audioSource.UnPause();
        }
    }

    public void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = Mathf.Clamp01(volume);
        }
    }

    public float GetVolume()
    {
        return audioSource != null ? audioSource.volume : 0f;
    }

    public void ToggleMute()
    {
        if (audioSource != null)
        {
            audioSource.mute = !audioSource.mute;
        }
    }

    public bool IsMuted()
    {
        return audioSource != null && audioSource.mute;
    }

    public void ChangeMusic(AudioClip newClip)
    {
        if (audioSource != null && newClip != null)
        {
            audioSource.Stop();
            audioSource.clip = newClip;
            audioSource.Play();
        }
    }
}

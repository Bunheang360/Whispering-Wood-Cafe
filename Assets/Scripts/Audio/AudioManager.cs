using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Background Music")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;

    [Header("Sound Effects")]
    public AudioClip buttonClickSFX;
    public AudioClip buttonHoverSFX;
    public AudioClip panelOpenSFX;
    public AudioClip panelCloseSFX;
    public AudioClip successSFX;
    public AudioClip errorSFX;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float musicVolume = 0.7f;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    [Header("Fade Settings")]
    public float fadeDuration = 1f;

    [Header("Auto Play")]
    public bool autoPlayOnStart = true;

    private float targetMusicVolume;
    private bool isFading = false;

    void Awake()
    {
        Debug.Log($"[AudioManager] Awake called at Time: {Time.time}");

        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize audio sources if not assigned
        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("MusicSource");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.loop = true;
        }

        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFXSource");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
        }

        // Configure music source for 2D audio
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.spatialBlend = 0f; // 2D sound
        musicSource.priority = 0; // Highest priority
        musicSource.bypassEffects = false;
        musicSource.bypassListenerEffects = false;
        musicSource.bypassReverbZones = false;

        // Configure SFX source for 2D audio
        sfxSource.spatialBlend = 0f; // 2D sound
        sfxSource.priority = 128; // Default priority

        // Set initial volumes immediately
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;

        // PLAY MUSIC IMMEDIATELY IN AWAKE (earliest possible moment)
        if (autoPlayOnStart && menuMusic != null)
        {
            Debug.Log($"[AudioManager] Playing in Awake at Time: {Time.time}");
            musicSource.clip = menuMusic;
            musicSource.Play();
            Debug.Log($"[AudioManager] Clip: {menuMusic.name}, LoadState: {menuMusic.loadState}, IsPlaying: {musicSource.isPlaying}");
        }
    }

    void Start()
    {
        Debug.Log($"[AudioManager] Start called at Time: {Time.time}, Music still playing: {musicSource.isPlaying}");
    }

    void Update()
    {
        // Handle music fading
        if (isFading)
        {
            musicSource.volume = Mathf.MoveTowards(
                musicSource.volume, 
                targetMusicVolume, 
                Time.deltaTime / fadeDuration
            );

            if (Mathf.Approximately(musicSource.volume, targetMusicVolume))
            {
                isFading = false;
                if (targetMusicVolume == 0f)
                {
                    musicSource.Stop();
                }
            }
        }
    }

    // ==================== MUSIC CONTROLS ====================

    public void PlayMusic(AudioClip clip, bool fade = false)
    {
        if (clip == null)
        {
            Debug.LogWarning("Trying to play null music clip!");
            return;
        }

        Debug.Log($"[AudioManager] PlayMusic called - Clip: {clip.name}, Length: {clip.length}s, Frequency: {clip.frequency}, Load State: {clip.loadState}");

        if (fade && musicSource.isPlaying)
        {
            StartCoroutine(CrossFadeMusic(clip));
        }
        else
        {
            musicSource.clip = clip;
            musicSource.volume = musicVolume;
            musicSource.Play();
            
            Debug.Log($"[AudioManager] Music.Play() executed at Time: {Time.time}");
            Debug.Log($"[AudioManager] IsPlaying: {musicSource.isPlaying}, Time: {musicSource.time}, Volume: {musicSource.volume}");
        }
    }

    public void StopMusic(bool fade = true)
    {
        if (fade)
        {
            FadeOutMusic();
        }
        else
        {
            musicSource.Stop();
        }
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        musicSource.UnPause();
    }

    public void FadeOutMusic()
    {
        targetMusicVolume = 0f;
        isFading = true;
    }

    public void FadeInMusic()
    {
        musicSource.volume = 0f;
        targetMusicVolume = musicVolume;
        isFading = true;
        
        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    private System.Collections.IEnumerator CrossFadeMusic(AudioClip newClip)
    {
        float startVolume = musicSource.volume;

        // Fade out
        while (musicSource.volume > 0)
        {
            musicSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();

        // Fade in
        while (musicSource.volume < musicVolume)
        {
            musicSource.volume += startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        musicSource.volume = musicVolume;
    }

    // ==================== SFX CONTROLS ====================

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlaySFX(AudioClip clip, float volumeScale)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume * volumeScale);
    }

    // Convenience methods for common SFX
    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSFX);
    }

    public void PlayButtonHover()
    {
        PlaySFX(buttonHoverSFX);
    }

    public void PlayPanelOpen()
    {
        PlaySFX(panelOpenSFX);
    }

    public void PlayPanelClose()
    {
        PlaySFX(panelCloseSFX);
    }

    public void PlaySuccess()
    {
        PlaySFX(successSFX);
    }

    public void PlayError()
    {
        PlaySFX(errorSFX);
    }

    // ==================== VOLUME CONTROLS ====================

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (!isFading)
        {
            musicSource.volume = musicVolume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxVolume;
    }

    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = Mathf.Clamp01(volume);
    }

    public void ToggleMusic()
    {
        if (musicSource.volume > 0)
        {
            musicSource.volume = 0;
        }
        else
        {
            musicSource.volume = musicVolume;
        }
    }

    public void ToggleSFX()
    {
        if (sfxSource.volume > 0)
        {
            sfxSource.volume = 0;
        }
        else
        {
            sfxSource.volume = sfxVolume;
        }
    }

    // ==================== UTILITY ====================

    public bool IsMusicPlaying()
    {
        return musicSource.isPlaying;
    }

    public float GetMusicVolume()
    {
        return musicVolume;
    }

    public float GetSFXVolume()
    {
        return sfxVolume;
    }
}

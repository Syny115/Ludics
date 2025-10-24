using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Singleton
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Sound Clips")]
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip correctAnswer;
    [SerializeField] private AudioClip wrongAnswer;
    [SerializeField] private AudioClip gameOver;
    [SerializeField] private AudioClip victory;

    [Header("Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float masterVolume = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.7f;
    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 1f;

    private void Awake()
    {
        // Implementar Singleton
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

        // Crear AudioSources si no existen
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }

        UpdateVolumes();
    }

    // Funciones para reproducir sonidos específicos
    public void PlayButtonClick()
    {
        PlaySFX(buttonClick);
    }

    public void PlayCorrectAnswer()
    {
        PlaySFX(correctAnswer);
    }

    public void PlayWrongAnswer()
    {
        PlaySFX(wrongAnswer);
    }

    public void PlayGameOver()
    {
        PlaySFX(gameOver);
    }

    public void PlayVictory()
    {
        PlaySFX(victory);
    }

    // Función genérica para reproducir cualquier clip
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
        }
    }

    // Función para reproducir sonido con volumen específico
    public void PlaySFX(AudioClip clip, float volume)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, volume * masterVolume);
        }
    }

    // Funciones para música de fondo
    public void PlayMusic(AudioClip musicClip)
    {
        if (musicClip != null && musicSource != null)
        {
            musicSource.clip = musicClip;
            musicSource.volume = musicVolume * masterVolume;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void PauseMusic()
    {
        if (musicSource != null)
        {
            musicSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (musicSource != null)
        {
            musicSource.UnPause();
        }
    }

    // Funciones para controlar volúmenes
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }

    private void UpdateVolumes()
    {
        if (musicSource != null)
        {
            musicSource.volume = musicVolume * masterVolume;
        }
    }

    // Funciones adicionales útiles
    public void PlayRandomPitch(AudioClip clip, float minPitch = 0.9f, float maxPitch = 1.1f)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.pitch = Random.Range(minPitch, maxPitch);
            sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
            sfxSource.pitch = 1f; // Resetear pitch
        }
    }
}
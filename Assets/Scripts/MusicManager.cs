using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip menuMusic;
    public AudioClip gameModeMusic;

    void Start()
    {
        // Reproduce la música del menú al iniciar
        PlayMenuMusic();
    }

    public void PlayMenuMusic()
    {
        audioSource.Stop();
        audioSource.clip = menuMusic;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void PlayGameModeMusic()
    {
        audioSource.Stop();
        audioSource.clip = gameModeMusic;
        audioSource.loop = true;
        audioSource.Play();
 
    }
}


using UnityEngine;
using UnityEngine.Audio;

public class BackgroundMusicManager : MonoBehaviour
{
    public AudioClip bgmClip; // Background music clip
    private AudioSource bgmSource;

    void Start()
    {
        // Create and configure the BGM AudioSource
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.clip = bgmClip;
        bgmSource.loop = true;
        bgmSource.outputAudioMixerGroup = AudioManager.instance.musicMixerGroup;

        // Set the initial BGM volume
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        bgmSource.volume = musicVolume;
        
        // Play the background music
        PlayBGM();
    }

    public void PlayBGM()
    {
        bgmSource.Play();
    }

    public void PauseBGM()
    {
        bgmSource.Pause();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }
}

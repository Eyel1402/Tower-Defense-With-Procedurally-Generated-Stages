using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioMixer audioMixer; // Attach the AudioMixer from the Inspector
    public AudioMixerGroup sfxMixerGroup;
    public AudioMixerGroup musicMixerGroup;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetMusicVolume(float volume)
    {
        float db = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20; // Convert linear to dB
        audioMixer.SetFloat("MusicVolume", db);
        //audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        float db = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20; // Convert linear to dB
        audioMixer.SetFloat("SFXVolume", db);
        //audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }
}

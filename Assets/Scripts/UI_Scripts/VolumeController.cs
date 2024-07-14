using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    public Button muteMusicButton;
    public Button muteSFXButton;
    public GameObject muteSFXImg;
    public GameObject muteMusicImg;

    private bool isMusicMuted = false;
    private bool isSFXMuted = false;

    private float lastMusicVolume = 0.75f;
    private float lastSFXVolume = 0.75f;

    void Start()
    {
        // Set initial volume values
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        lastMusicVolume = musicVolume;
        lastSFXVolume = sfxVolume;

        // Update sliders
        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;

        // Set audio volumes based on slider values
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);

        // Add listeners to handle value changes
        musicSlider.onValueChanged.AddListener(OnMusicSliderValueChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXSliderValueChanged);

        // Add listeners for mute buttons
        muteMusicButton.onClick.AddListener(ToggleMuteMusic);
        muteSFXButton.onClick.AddListener(ToggleMuteSFX);

        // Update icons based on the initial state
        UpdateMuteIcons();
    }

    private void OnMusicSliderValueChanged(float volume)
    {
        if (isMusicMuted && volume > 0)
        {
            isMusicMuted = false; // Unmute if slider is moved
            UpdateMuteIcons();
        }
        lastMusicVolume = volume;
        SetMusicVolume(volume);
    }

    private void OnSFXSliderValueChanged(float volume)
    {
        if (isSFXMuted && volume > 0)
        {
            isSFXMuted = false; // Unmute if slider is moved
            UpdateMuteIcons();
        }
        lastSFXVolume = volume;
        SetSFXVolume(volume);
    }

    public void SetMusicVolume(float volume)
    {
        if (!isMusicMuted)
        {
            AudioManager.instance.SetMusicVolume(volume);
            PlayerPrefs.SetFloat("MusicVolume", volume);
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (!isSFXMuted)
        {
            AudioManager.instance.SetSFXVolume(volume);
            PlayerPrefs.SetFloat("SFXVolume", volume);
        }
    }

    public void ToggleMuteMusic()
    {
        isMusicMuted = !isMusicMuted;
        if (isMusicMuted)
        {
            // Mute
            AudioManager.instance.SetMusicVolume(0);
        }
        else
        {
            // Unmute
            AudioManager.instance.SetMusicVolume(lastMusicVolume);
        }
        UpdateMuteIcons();
    }

    public void ToggleMuteSFX()
    {
        isSFXMuted = !isSFXMuted;
        if (isSFXMuted)
        {
            // Mute
            AudioManager.instance.SetSFXVolume(0);
        }
        else
        {
            // Unmute
            AudioManager.instance.SetSFXVolume(lastSFXVolume);
        }
        UpdateMuteIcons();
    }

    private void UpdateMuteIcons()
    {
        // Update mute icon for music
        muteMusicImg.SetActive(isMusicMuted);

        // Update mute icon for SFX
        muteSFXImg.SetActive(isSFXMuted);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHitEffect : MonoBehaviour
{
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private float startTime = 0f;
    [SerializeField] private float volume = 1f;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource != null && hitSound != null)
        {
            audioSource.outputAudioMixerGroup = AudioManager.instance.sfxMixerGroup;
            audioSource.clip = hitSound;
            audioSource.volume = volume;
            audioSource.time = startTime;
            audioSource.Play();
            
            Destroy(gameObject, hitSound.length - startTime);
        }
        else
        {
            Debug.LogWarning("AudioSource or hitSound is not assigned. Destroying object immediately.");
            Destroy(gameObject);
        }
    }
}

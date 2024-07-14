using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireExplodeEffecft : MonoBehaviour
{
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private float startTime = 0f; // Start time in seconds
    private AudioSource audioSource;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource != null && hitSound != null)
        {
            audioSource.outputAudioMixerGroup = AudioManager.instance.sfxMixerGroup;
            audioSource.clip = hitSound;
            audioSource.time = startTime; // Set the start time
            audioSource.Play();
            
            // Ensure the game object is destroyed after the sound has finished playing from the start time
            Destroy(gameObject, hitSound.length - startTime);
        }
        else
        {
            Debug.LogWarning("AudioSource or hitSound is not assigned. Destroying object immediately.");
            Destroy(gameObject);
        }
    }
}

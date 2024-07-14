using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DestroyedSoundEffect : MonoBehaviour
{
    [SerializeField] private AudioClip deathSound;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
            // Destroy the object after the sound finishes
            Destroy(gameObject, deathSound.length);
        }
        else
        {
            Debug.LogWarning("AudioSource or deathSound is not assigned. Destroying object immediately.");
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class RestartButtons : MonoBehaviour
{
    public GameObject menuPanel; // Reference to the panel containing the menu buttons
    public AudioSource buttonSound;
    public AudioClip buttonClickClip;
    void Start()
    {
        CloseMenu();
    }

    public void ToggleMenu()
    {
        PlayButtonSound();
        menuPanel.SetActive(!menuPanel.activeSelf);
    }

    public void RestartScene()
    {
        PlayButtonSound();
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void CloseMenu()
    {
        PlayButtonSound();
        menuPanel.SetActive(false);
    }
    private void PlayButtonSound()
    {
        if (buttonSound != null && buttonClickClip != null)
        {
            buttonSound.PlayOneShot(buttonClickClip);
        }
    }
}

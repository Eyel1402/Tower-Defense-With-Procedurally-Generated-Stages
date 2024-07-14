using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu_Script : MonoBehaviour
{
    public static bool GameMode_option = false;
    public GameObject GameMode_UI;

    // Add an AudioSource component to the GameObject that has this script
    public AudioSource buttonSound;

    // Add a reference to the button sound clip in the Inspector
    public AudioClip buttonClickClip;

    // The specific duration (in seconds) to start playing the audio clip from
    public float startFromTime = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the button sound clip is not null
        if (buttonClickClip == null)
        {
            Debug.LogWarning("Button click sound clip is not assigned.");
        }
    }
    void Update()
    {
        // when the Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(GameMode_option)
            {
                close_bttn();
            }
            else
            {
                // nothing
            }
        }
    }

    public void StartGame(int gameMode)
    {
        // Play the button click sound starting from a specific duration
        PlayButtonSound(startFromTime);

        PlayerPrefs.SetInt("GameMode", gameMode); // Save the selected game mode
        SceneManager.LoadScene("GameScene"); // Load the game scene
    }

    public void close_bttn()
    {
        // Play the button click sound starting from a specific duration
        PlayButtonSound(startFromTime);

        GameMode_UI.SetActive(false);
        //Time.timeScale = 1f;    
        GameMode_option = false;
    }

    public void Play_bttn()
    {
        // Play the button click sound starting from a specific duration
        PlayButtonSound(startFromTime);

        GameMode_UI.SetActive(true);
        //Time.timeScale = 0f;    
        GameMode_option = true;
    }

    public void QuitGame() //Quit Button
    {
        // Play the button click sound starting from a specific duration
        PlayButtonSound(startFromTime);

        Debug.Log("QUIT");
        Application.Quit();
    }

    // Function to play the button click sound
    private void PlayButtonSound(float startTime)
    {
        if (buttonSound != null && buttonClickClip != null)
        {
            buttonSound.time = startTime; // Set the start time
            buttonSound.PlayOneShot(buttonClickClip);
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class Options_Script : MonoBehaviour
{
    public static bool GameOption = false;
    public GameObject SettingMenuUi;

    public AudioSource buttonSound;

    public AudioClip buttonClickClip;

    void Start()
    {
        if (buttonClickClip == null)
        {
            Debug.LogWarning("Button click sound clip is not assigned.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Play the button click sound
            PlayButtonSound();

            if (GameOption)
            {
                Resume();
            }
            else
            {
                Settings();
            }
        }
    }

    public void Resume()
    {
        PlayButtonSound();
        SettingMenuUi.SetActive(false);
        Time.timeScale = 1f;    
        GameOption = false;
    }

    public void Settings()
    {
        PlayButtonSound();
        SettingMenuUi.SetActive(true);
        Time.timeScale = 0f;    
        GameOption = true;
    }

    public void ChangeScene(string Main_Menu)
    {
        // Play the button click sound
        PlayButtonSound();

        SceneManager.LoadScene(Main_Menu);
    }

    // Function to play the button click sound
    private void PlayButtonSound()
    {
        if (buttonSound != null && buttonClickClip != null)
        {
            buttonSound.PlayOneShot(buttonClickClip);
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
public class Menu_Options_Script : MonoBehaviour
{
    public static bool ClosedPanel = false;
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
            if(!ClosedPanel)
            {
                ClosePanel();
            }
            else
            {
                //nothing
            }
        }
    }

    public void ClosePanel()
    {
        PlayButtonSound();
        SettingMenuUi.SetActive(false);
        Time.timeScale = 1f;    
        ClosedPanel = true;
    }

    public void Settings()
    {
        PlayButtonSound();
        SettingMenuUi.SetActive(true);
        Time.timeScale = 0f;    
        ClosedPanel = false;
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

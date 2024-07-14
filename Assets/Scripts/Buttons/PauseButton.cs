using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    public GameObject pauseImage;
    public GameObject playImage;
    public GameObject normalSpdImage;
    public GameObject fastFwdImage;

    public AudioSource buttonSound;
    public AudioClip buttonClickClip;


    private bool isPaused = false;
    private bool isFastForward = false;

    void Start()
    {
        pauseImage.SetActive(false);
        playImage.SetActive(true);
        normalSpdImage.SetActive(true);
        fastFwdImage.SetActive(false);
    }
    
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
            //PlayButtonSound();
            //TogglePause();
        //}
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        
        if (isPaused)
        {
            PauseGame();

            pauseImage.SetActive(true);
            playImage.SetActive(false);
            normalSpdImage.SetActive(true);
            fastFwdImage.SetActive(false);

            isFastForward = false;
            isPaused = true;
        }
        else
        {
            if(isFastForward)
            {
                FastForward();

                pauseImage.SetActive(false);
                playImage.SetActive(true);
                normalSpdImage.SetActive(false);
                fastFwdImage.SetActive(true);

                isFastForward = true;
                isPaused = false;
            }
            else
            {
                ResumeGame();

                pauseImage.SetActive(false);
                playImage.SetActive(true);
                normalSpdImage.SetActive(true);
                fastFwdImage.SetActive(false);

                isFastForward = false;
                isPaused = false;
            }
        }
    }

    public void ToggleFastForward()
    {
        isFastForward = !isFastForward;

        if (isFastForward && !isPaused)
        {
            FastForward();
            pauseImage.SetActive(false);
            playImage.SetActive(true);
            normalSpdImage.SetActive(false);
            fastFwdImage.SetActive(true);

            isFastForward = true;
            isPaused = false;
        }
        else
        {
            ResumeGame();
            pauseImage.SetActive(false);
            playImage.SetActive(true);
            normalSpdImage.SetActive(true);
            fastFwdImage.SetActive(false);

            isFastForward = false;
            isPaused = false;
        }
    }
    void PauseGame()
    {
        PlayButtonSound();
        Time.timeScale = 0f;
    }

    void ResumeGame()
    {
        PlayButtonSound();
        Time.timeScale = 1f;
    }

    void FastForward()
    {
        PlayButtonSound();
        Time.timeScale = 2f;
    }

    private void PlayButtonSound()
    {
        if (buttonSound != null && buttonClickClip != null)
        {
            buttonSound.PlayOneShot(buttonClickClip);
        }
    }
}

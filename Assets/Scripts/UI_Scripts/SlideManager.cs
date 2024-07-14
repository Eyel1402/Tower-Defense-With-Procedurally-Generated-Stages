using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;

public class SlideManager : MonoBehaviour
{
    public GameObject[] slides;
    public float slideDuration = 5f; // Duration for each slide in seconds
    public float transitionDuration = 1f; // Duration for the transition in seconds
    public AudioClip bgmClip; // Background music clip

    private int currentSlideIndex = 0;
    private bool isTransitioning = false;
    private AudioSource bgmSource;
    private Coroutine slideShowCoroutine;

    void Start()
    {
        // Create and configure the BGM AudioSource
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.volume = 0.5f;
        bgmSource.clip = bgmClip;
        bgmSource.outputAudioMixerGroup = AudioManager.instance.musicMixerGroup;
        bgmSource.loop = true;
        bgmSource.Play();

        // Set the initial BGM volume
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        AudioManager.instance.SetMusicVolume(musicVolume);

        ShowSlide(currentSlideIndex);
        slideShowCoroutine = StartCoroutine(SlideShow());
    }


    void Update()
    {
        if (Input.anyKeyDown)
        {
            LoadNextScene();
        }
    }

    private IEnumerator SlideShow()
    {
        while (currentSlideIndex < slides.Length)
        {
            yield return new WaitForSeconds(slideDuration);
            ShowNextSlide();
        }

        // After showing all slides, wait for the transition of the last slide to finish
        while (isTransitioning)
        {
            yield return null;
        }

        // Load the next scene after the last slide
        LoadNextScene();
    }

    public void ShowNextSlide()
    {
        if (isTransitioning)
            return;

        isTransitioning = true;

        var currentSlide = slides[currentSlideIndex];

        if (currentSlideIndex == slides.Length - 1)
        {
            // Animate fade out the current slide
            CanvasGroup currentCanvasGroup = currentSlide.GetComponent<CanvasGroup>();

            if (currentCanvasGroup == null)
            {
                currentCanvasGroup = currentSlide.AddComponent<CanvasGroup>();
            }

            currentCanvasGroup.DOFade(0, transitionDuration).OnComplete(() =>
            {
                currentSlide.SetActive(false);
                isTransitioning = false;
                // Load the next scene after the last slide
                LoadNextScene();
            });
        }
        else
        {
            var nextSlide = slides[++currentSlideIndex];

            // Animate fade out the current slide
            CanvasGroup currentCanvasGroup = currentSlide.GetComponent<CanvasGroup>();
            CanvasGroup nextCanvasGroup = nextSlide.GetComponent<CanvasGroup>();

            if (currentCanvasGroup == null)
            {
                currentCanvasGroup = currentSlide.AddComponent<CanvasGroup>();
            }

            if (nextCanvasGroup == null)
            {
                nextCanvasGroup = nextSlide.AddComponent<CanvasGroup>();
            }

            nextCanvasGroup.alpha = 0;

            currentCanvasGroup.DOFade(0, transitionDuration).OnComplete(() =>
            {
                currentSlide.SetActive(false);
                nextSlide.SetActive(true);
                // Animate fade in the next slide
                nextCanvasGroup.DOFade(1, transitionDuration).OnComplete(() =>
                {
                    isTransitioning = false;
                });
            });
        }
    }

    private void ShowSlide(int index)
    {
        if (index < 0 || index >= slides.Length)
        {
            Debug.LogError("Invalid slide index!");
            return;
        }

        slides[index].SetActive(true);
        var canvasGroup = slides[index].GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = slides[index].AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 1;
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene("Main_Menu"); // Replace with your main menu scene name
    }

    private void OnDestroy()
    {
        if (slideShowCoroutine != null)
            StopCoroutine(slideShowCoroutine);
    }
}

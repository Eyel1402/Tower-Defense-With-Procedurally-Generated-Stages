using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class List_UI_Script : MonoBehaviour
{
    public static bool GameList = false;
    public GameObject ListMenuUi;
    public AudioClip buttonClickClip;
    public AudioSource buttonSound;

    void Start()
    {
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
            if(GameList)
            {
                Close_List();
            }
            else
            {
                // nothing
            }
        }
    }
    public void ListMenu() //Active List Menu Popup
    {
        PlayButtonSound();
        ListMenuUi.SetActive(true);
        //Time.timeScale = 0f;    
        GameList = true;
    }

    public void TowerGallery()
    {
        PlayButtonSound();
        SceneManager.LoadScene("Tower_Gallery_Menu");
    }

    public void SlimeGallery()
    {
        PlayButtonSound();
        SceneManager.LoadScene("Slime_Gallery_Menu");
    }

    public void Close_List()
    {
        PlayButtonSound();
        ListMenuUi.SetActive(false);
        //Time.timeScale = 0f;    
        GameList = false;
    }

    private void PlayButtonSound()
    {
        if (buttonSound != null && buttonClickClip != null)
        {
            buttonSound.PlayOneShot(buttonClickClip);
        }
    }

}

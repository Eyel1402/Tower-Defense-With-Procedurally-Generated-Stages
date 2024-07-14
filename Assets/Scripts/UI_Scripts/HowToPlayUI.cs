using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowToPlayUI : MonoBehaviour
{
    public GameObject tutorialScreen;

    void Start()
    {
        // Set the timescale to 0 to pause the game
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if any key or mouse button is pressed
        if (Input.anyKeyDown)
        {
            // Make the target object disappear
            if (tutorialScreen != null)
            {
                tutorialScreen.SetActive(false);
            }

            // Set the timescale back to 1 to resume the game
            Time.timeScale = 1;
        }
    }
}


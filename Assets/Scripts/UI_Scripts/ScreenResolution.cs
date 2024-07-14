using UnityEngine;
using TMPro; // Import TextMeshPro namespace
using System.Collections.Generic;

public class ScreenResolutionManager : MonoBehaviour
{
    private TMP_Dropdown resolutionDropdown; // Reference to the TMP dropdown
    private List<Resolution> commonResolutions;
    public GameObject SettingMenuUi;

    void Start()
    {
        // Find the TMP dropdown component in the hierarchy based on a specific criteria
        resolutionDropdown = GameObject.FindWithTag("DropDownTag").GetComponent<TMP_Dropdown>();

        if (resolutionDropdown == null)
        {
            Debug.LogError("TMP_Dropdown component not found.");
            return;
        }

        // Define common resolutions, including a smaller one than 1280x720 (1024x576)
        commonResolutions = new List<Resolution>
        {
            new Resolution { width = 1024, height = 576 },
            new Resolution { width = 1280, height = 720 },  // Base resolution
            new Resolution { width = 1366, height = 768 },  // closest standard resolution
            new Resolution { width = 1600, height = 900 },
            new Resolution { width = 1920, height = 1080 },
            new Resolution { width = 2560, height = 1440 },
            new Resolution { width = 3840, height = 2160 }
        };

        // Initialize resolutions dropdown
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int savedResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 0);

        for (int i = 0; i < commonResolutions.Count; i++)
        {
            string option = commonResolutions[i].width + " x " + commonResolutions[i].height;
            options.Add(option);
        }

        resolutionDropdown.AddOptions(options);

        // Load the saved resolution
        if (savedResolutionIndex >= 0 && savedResolutionIndex < commonResolutions.Count)
        {
            resolutionDropdown.value = savedResolutionIndex;
            SetResolution(savedResolutionIndex); // Set the saved resolution
        }
        else
        {
            resolutionDropdown.value = 1; // Default to the 1280 x 720 resolution
            SetResolution(1); // Default to the second resolution (1280 x 720)
        }

        resolutionDropdown.RefreshShownValue();

        // Add listener
        resolutionDropdown.onValueChanged.AddListener(SetResolution);

        SettingMenuUi.SetActive(false);
    }

    // Function to set the resolution
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = commonResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        // Save the selected resolution index
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
        PlayerPrefs.Save();
    }
}

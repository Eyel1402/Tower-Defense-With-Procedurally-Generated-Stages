using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class WinUIManager : MonoBehaviour
{
    public GameObject winUI; // Reference to the Win UI GameObject
    public TextMeshProUGUI winText; // Reference to the TextMeshProUGUI component for the win message

    private void Start()
    {
        winUI.SetActive(false); // Ensure the win UI is initially inactive
    }

    public void ShowWinUI(int waveNumber)
    {
        winUI.SetActive(true); // Activate the win UI
        winText.text = "Congratulations! You've completed " + waveNumber + " waves!"; // Set the win message
        Time.timeScale = 0f;  
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Main_Menu"); // Load the main menu scene
    }
}

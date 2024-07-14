using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUIManager : MonoBehaviour
{
    public GameObject gameOverUI;

    void Start()
    {
        HideGameOverUI();
    }

    public void ShowGameOverUI()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = 0f;  
    }

    public void HideGameOverUI()
    {
        gameOverUI.SetActive(false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Main_Menu");
    }
}

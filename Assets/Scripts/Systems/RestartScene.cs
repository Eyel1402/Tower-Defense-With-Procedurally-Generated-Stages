using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class RestartSceneButton : MonoBehaviour
{
    public GameObject restartMenuPanel; // Reference to the panel containing the menu buttons
    public void OpenMenu()
    {
        restartMenuPanel.SetActive(true);
    }
}

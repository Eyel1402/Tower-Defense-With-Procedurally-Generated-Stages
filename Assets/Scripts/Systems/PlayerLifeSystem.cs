using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class LifeSystem : MonoBehaviour
{
    public int startingLifePoints = 10;
    private int currentLifePoints;
    public TMP_Text lifeCount;
    private const int maxLifePoints = 100; // Maximum limit for life points

    void Start()
    {
        currentLifePoints = startingLifePoints;
        UpdateLifePoints();
    }

    public void AddLifePoints(int amount)
    {
        currentLifePoints += amount;
        
        // Ensure currentLifePoints does not exceed maxLifePoints
        if (currentLifePoints > maxLifePoints)
        {
            currentLifePoints = maxLifePoints;
        }

        UpdateLifePoints();     
    }

    private void UpdateLifePoints()
    {
        lifeCount.text = currentLifePoints.ToString();
    }

    public bool LoseLifePoints(int amount)
    {
        currentLifePoints -= amount;
        
        // Ensure currentLifePoints does not go below zero
        if (currentLifePoints < 0)
        {
            currentLifePoints = 0;
        }

        UpdateLifePoints();
        CheckGameOver();

        // Return true to indicate life points were lost
        return true;
    }

    public int GetCurrentLifePoints()
    {
        return currentLifePoints;
    }

    private void CheckGameOver()
    {
        if (currentLifePoints <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");
        FindFirstObjectByType<GameOverUIManager>().ShowGameOverUI();
    }
}



/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class LifeSystem : MonoBehaviour
{
    public int startingLifePoints = 10;
    private int currentLifePoints;
    public TMP_Text lifeCount;

    void Start()
    {
        currentLifePoints = startingLifePoints;
        UpdateLifePoints();
    }

    public void AddLifePoints(int amount)
    {
        currentLifePoints += amount;
        UpdateLifePoints();     
    }

    private void UpdateLifePoints()
    {
        lifeCount.text = currentLifePoints.ToString();
    }

    public bool LoseLifePoints(int amount)
    {
        currentLifePoints -= amount;
        
        // Ensure currentLifePoints does not go below zero
        if (currentLifePoints < 0)
        {
            currentLifePoints = 0;
        }

        UpdateLifePoints();
        CheckGameOver();

        // Return true to indicate life points were lost
        return true;
    }

    public int GetCurrentLifePoints()
    {
        return currentLifePoints;
    }

    private void CheckGameOver()
    {
        if (currentLifePoints <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
{
    Debug.Log("Game Over!");
    FindFirstObjectByType<GameOverUIManager>().ShowGameOverUI();
}

}
*/
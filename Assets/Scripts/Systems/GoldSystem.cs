using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GoldSystem : MonoBehaviour
{
    public int startingGold = 10;
    public int currentGold;
    public TMP_Text goldCount;

    void Start()
    {
        currentGold = startingGold;
        UpdateGoldCount();      
    }
    public void AddGold(int amount)
    {
        currentGold += amount;
        UpdateGoldCount();
    }
    private void UpdateGoldCount()
    {
        goldCount.text = currentGold.ToString();
    }

    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            UpdateGoldCount();
            return true;
        }
        else
        {
            return false;
        }
    }

    public int GetCurrentGold()
    {
        return currentGold;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviousWaveStats : MonoBehaviour
{
    public static PreviousWaveStats instance;

    public int previousMaxHealth = 100;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
